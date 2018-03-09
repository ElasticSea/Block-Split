using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Core.Extensions;
using Assets.Core.Scripts;
using Assets.Core.Scripts.Extensions;
using DG.Tweening;
using UnityEngine;

namespace Assets.Scripts
{
    public class StackBuilder : MonoBehaviour
    {
        [SerializeField] private Vector3 pedestalSize = new Vector3(1, .5f, 1);
        [SerializeField] private float blockHeight = .2f;
        [SerializeField] private float snapMarginOfError = .1f;
        [SerializeField] private Color blockColor = Color.blue;
        [SerializeField] private Color cutoutColor = Color.red;
        [SerializeField] private float speed = 1;

        private readonly List<Collider> blocks = new List<Collider>();

        private Collider currentBlock;
        private Pool<GameObject> pool;
        private Bounds blockBounds;

        public Vector3 PedestalSize
        {
            get { return pedestalSize; }
            set { pedestalSize = value; }
        }

        public float BlockHeight
        {
            get { return blockHeight; }
            set { blockHeight = value; }
        }

        public float SnapMarginOfError
        {
            get { return snapMarginOfError; }
            set { snapMarginOfError = value; }
        }

        public Color BlockColor
        {
            get { return blockColor; }
            set { blockColor = value; }
        }

        public Color CutoutColor
        {
            get { return cutoutColor; }
            set { cutoutColor = value; }
        }

        public float Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        public List<Collider> Blocks => blocks.ToList();
        public int Count => blocks.Count;

        public event Action<PlacementResult> OnBlockPlaced = result => { };

        private void Awake()
        {
            pool = new Pool<GameObject>(10, () => GameObject.CreatePrimitive(PrimitiveType.Cube), go =>
            {
                go.SetActive(true);
                go.GetOrAddComponent<Rigidbody>().isKinematic = true;
                go.transform.rotation = Quaternion.identity;
                go.transform.position = Vector3.zero;
                go.transform.localScale = Vector3.one;
            }, go => go.SetActive(false));
        }

        private void Start()
        {
            var pos = Vector3.zero.SetY(-PedestalSize.y / 2);
            var block = CreateBlock(pos, pos, PedestalSize, 0, false);
            blockBounds = new Bounds(block.transform.position, block.transform.localScale);
            blocks.Add(block);
            OnBlockPlaced(PlacementResult.Placed);
        }

        // Creates new block
        public void SpawnBlock()
        {
            if (currentBlock != null) return;

            var lastblock = blocks.Last();
            var position = blockBounds.center;
            var height = position.y + lastblock.bounds.extents.y + BlockHeight / 2;
            var size = blockBounds.size.SetY(BlockHeight);

            if (blocks.Count % 2 == 0)
            {
                currentBlock = CreateBlock(
                    (position + Vector3.forward).SetY(height),
                    (position + Vector3.back).SetY(height),
                    size, Speed);
            }
            else
            {
                currentBlock = CreateBlock(
                    (position + Vector3.left).SetY(height),
                    (position + Vector3.right).SetY(height),
                    size, Speed);
            }
        }

        // Places current block
        public void PlaceBlock()
        {
            if (currentBlock == null) return;

            PlaceBlock(blocks.Last(), currentBlock);
            currentBlock = null;
        }

        private Collider CreateBlock(Vector3 from, Vector3 to, Vector3 size, float speed, bool animate = true)
        {
            var block = pool.Get().GetComponent<Collider>();
            block.GetComponent<Renderer>().material.color = blockColor;
            block.transform.localScale = size;
            block.transform.position = from;

            if (animate)
                block.transform.DOMove(to, from.Distance(to) / speed).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);

            return block;
        }

        // Extends block in random direction by margin size
        public void Extend()
        {
            var offsets = new[]
            {
                Tuple.Create(Vector3.left, .5f - Mathf.Abs(blockBounds.center.x - blockBounds.size.x / 2f)),
                Tuple.Create(Vector3.right, .5f - Mathf.Abs(blockBounds.center.x + blockBounds.size.x / 2f)),
                Tuple.Create(Vector3.back, .5f - Mathf.Abs(blockBounds.center.z - blockBounds.size.z / 2f)),
                Tuple.Create(Vector3.forward, .5f - Mathf.Abs(blockBounds.center.z + blockBounds.size.z / 2f))
            };

            var dir = offsets.OrderByDescending(it => it.Item2).FirstOrDefault(it => it.Item2 > 0.001f)?.Item1;

            if (dir != null)
            {
                var last = blocks.Last().transform;
                blockBounds = new Bounds(
                    last.position + dir.Value * snapMarginOfError / 2f,
                    last.localScale + dir.Value.Abs() * snapMarginOfError
                );

                last.DOScale(blockBounds.size, .3f).SetEase(Ease.Linear);
                last.DOMove(blockBounds.center, .3f).SetEase(Ease.Linear);
            }
        }

        private void PlaceBlock(Collider previous, Collider current)
        {
            ClampCollider(previous);
            ClampCollider(current);

            // TODO put it in the pool
            // pool.Put(current.gameObject);
            Destroy(current.gameObject);

            var result = BoxCutter.Cut(previous.bounds, current.bounds);

            GameObject baseBlock = null;
            if (result.Base != null)
            {
                baseBlock = CreateBox(result.Base.Value);
                baseBlock.GetComponent<Renderer>().material.color = blockColor;
                baseBlock.name = "Block";
                baseBlock.gameObject.GetOrAddComponent<Rigidbody>().isKinematic = true;

                blocks.Add(baseBlock.GetComponent<Collider>());
            }

            if (result.Cutout != null)
            {
                var cutout = CreateBox(result.Cutout.Value);
                cutout.GetComponent<Renderer>().material.color = cutoutColor;
                cutout.name = "Cutout";
                cutout.gameObject.GetOrAddComponent<Rigidbody>().isKinematic = false;

                var kill = cutout.GetOrAddComponent<KillBlockBelowHeight>();
                kill.Height = blocks.Last().transform.position.y - 10;
                kill.OnBelowHeight = go => { pool.Put(go); };
            }

            if (baseBlock != null)
            {
                blockBounds = new Bounds(baseBlock.transform.position, baseBlock.transform.localScale);
                OnBlockPlaced(result.Cutout == null ? PlacementResult.Placed : PlacementResult.Partial);
            }
            else
            {
                OnBlockPlaced(PlacementResult.Miss);
            }
        }

        private void ClampCollider(Collider previous)
        {
            previous.transform.position = previous.bounds
                .GetVertices()
                .Select(v => v.Snap(SnapMarginOfError, int.MaxValue))
                .ToArray()
                .ToBounds()
                .center;
        }

        private GameObject CreateBox(Bounds resultBase)
        {
            var box = pool.Get();
            box.transform.position = resultBase.center;
            box.transform.localScale = resultBase.size;
            return box;
        }

        public enum PlacementResult
        {
            Placed,
            Partial,
            Miss
        }
    }
}