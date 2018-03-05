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

        private List<Collider> blocks = new List<Collider>();
        public List<Collider> Blocks => blocks.ToList();

        private Collider currentBlock;
        private Tweener transition;
        private Pool<GameObject> pool;
        private Bounds extendedForm;

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
            var block = CreateBlock(pos, pos, PedestalSize, 0);

            transition.Kill();
            extendedForm = new Bounds(block.transform.position, block.transform.localScale);
            blocks.Add(block);
            OnBlockPlaced(PlacementResult.Placed);
        }

        public void SpawnBlock()
        {
            if (currentBlock != null) return;

            var lastblock = blocks.Last();
            var position = extendedForm.center;
            var height = position.y + lastblock.bounds.extents.y + BlockHeight / 2;
            var size = extendedForm.size.SetY(BlockHeight);

            if (blocks.Count % 2 == 0)
            {
                currentBlock = CreateBlock(
                    (position + Vector3.forward).SetY(height),
                    (position + Vector3.back).SetY(height),
                    size, 1);
            }
            else
            {
                currentBlock = CreateBlock(
                    (position + Vector3.left).SetY(height),
                    (position + Vector3.right).SetY(height),
                    size, 1);
            }
        }

        public void PlaceBlock()
        {
            if (currentBlock == null) return;

            PlaceBlock(blocks.Last(), currentBlock);
            currentBlock = null;
        }

        private Collider CreateBlock(Vector3 from, Vector3 to, Vector3 size, float speed)
        {
            var block = pool.Get().GetComponent<Collider>();
            block.GetComponent<Renderer>().material.color = blockColor;
            block.transform.localScale = size;
            block.transform.position = from;
            transition = block.transform.DOMove(to, from.Distance(to) / speed).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
            return block;
        }

        public void Extend()
        {
            var last = blocks.Last().transform;

            var offsets = new[]
            {
                Tuple.Create(Vector3.left, .5f - Mathf.Abs(extendedForm.center.x - extendedForm.size.x / 2f)),
                Tuple.Create(Vector3.right,  .5f- Mathf.Abs(extendedForm.center.x + extendedForm.size.x / 2f)),
                Tuple.Create(Vector3.back,  .5f -Mathf.Abs(extendedForm.center.z - extendedForm.size.z / 2f)),
                Tuple.Create(Vector3.forward, .5f -Mathf.Abs( extendedForm.center.z + extendedForm.size.z / 2f))
            };

            var dir = offsets.OrderByDescending(it => it.Item2).FirstOrDefault(it => it.Item2 > 0.001f)?.Item1;
            if (dir != null)
            {
                extendedForm = new Bounds(
                    last.position + dir.Value * snapMarginOfError / 2f,
                    last.localScale + dir.Value.Abs() * snapMarginOfError
                );

                last.DOScale(extendedForm.size, .3f).SetEase(Ease.Linear);
                last.DOMove(extendedForm.center, .3f).SetEase(Ease.Linear);
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

            transition.Kill();

            GameObject baseBlock = null;
            if (result.Base != null)
            {
                baseBlock = CreateBox(result.Base.Value);
                baseBlock.GetComponent<Renderer>().material.color = blockColor;
                baseBlock.name = "Block";
                baseBlock.gameObject.GetOrAddComponent<Rigidbody>().isKinematic = true;

                blocks.Add(baseBlock.GetComponent<Collider>());
            }
            else
            {
                OnBlockPlaced(PlacementResult.Miss);
                return;
                //Die
            }

            if (result.Cutout != null)
            {
                var cutout = CreateBox(result.Cutout.Value);
                cutout.GetComponent<Renderer>().material.color = cutoutColor;
                cutout.name = "Cutout";
                cutout.gameObject.GetOrAddComponent<Rigidbody>().isKinematic = false;


                var kill = cutout.GetOrAddComponent<KillBlockBelowHeight>();
                kill.Height = blocks.Last().transform.position.y - 10;
                kill.Pool = pool;
            }
            else
            {
                //place the same
            }
            extendedForm = new Bounds(baseBlock.transform.position, baseBlock.transform.localScale);
            OnBlockPlaced(result.Cutout == null ? PlacementResult.Placed : PlacementResult.Partial);

        }

        private void ClampCollider(Collider previous)
        {
            previous.transform.position = previous.bounds.GetVertices().Select(v => v.Snap(SnapMarginOfError, 10000)).ToArray().ToBounds()
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
            Placed, Partial, Miss
        }
    }
}
