using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Core.Extensions;
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

        private Collider block;
        private Tweener transition;

        public event Action<Collider> OnBlockAdded = block => { };

        private void Start()
        {
            var pos = Vector3.zero.SetY(-PedestalSize.y / 2);
            var block = CreateBlock(pos, pos, PedestalSize, 0);
            PlaceBlock(null, block);
        }

        public void SpawnBlock()
        {
            if (block) PlaceBlock(blocks.Last(), block);

            var lastblock = blocks.Last();
            var height = lastblock.transform.position.y + lastblock.bounds.extents.y + BlockHeight / 2;
            var size = lastblock.transform.localScale.SetY(BlockHeight);


            if (blocks.Count % 2 == 0)
            {
                block = CreateBlock(
                    (lastblock.transform.position + Vector3.forward).SetY(height),
                    (lastblock.transform.position + Vector3.back).SetY(height),
                    size, 1);
            }
            else
            {
                block = CreateBlock(
                    (lastblock.transform.position + Vector3.left).SetY(height),
                    (lastblock.transform.position + Vector3.right).SetY(height),
                    size, 1);
            }
        }

        private Collider CreateBlock(Vector3 from, Vector3 to, Vector3 size, float speed)
        {
            var block = GameObject.CreatePrimitive(PrimitiveType.Cube).GetComponent<Collider>();
            block.GetComponent<Renderer>().material.color = blockColor;
            block.transform.localScale = size;
            block.transform.position = from;
            transition = block.transform.DOMove(to, from.Distance(to) / speed).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
            return block;
        }

        private void PlaceBlock(Collider previous, Collider current)
        {
            if (previous != null)
            {
                previous.transform.position = previous.bounds.GetVertices().Select(v => v.Snap(SnapMarginOfError, 10000)).ToArray().ToBounds()
                    .center;
                current.transform.position = current.bounds.GetVertices().Select(v => v.Snap(SnapMarginOfError, 10000)).ToArray().ToBounds()
                    .center;

                Destroy(current.gameObject);
                var result = BoxCutter.Cut(previous.bounds, current.bounds);

                transition.Kill();

                if (result.Base != null)
                {
                    var block = createBox(result.Base.Value);
                    block.GetComponent<Renderer>().material.color = blockColor;
                    block.name = "Block";
                    block.gameObject.AddComponent<Rigidbody>().isKinematic = true;

                    var bb = block.GetComponent<Collider>();
                    blocks.Add(bb);
                    OnBlockAdded(bb);
                }
                else
                {
                    //Die
                }

                if (result.Cutout != null)
                {
                    var cutout = createBox(result.Cutout.Value);
                    cutout.GetComponent<Renderer>().material.color = cutoutColor;
                    cutout.name = "Cutout";
                    cutout.gameObject.AddComponent<Rigidbody>().isKinematic = false;
                }
                else
                {
                    //place the same
                }

            }
            else
            {
                transition.Kill();
                current.gameObject.AddComponent<Rigidbody>().isKinematic = true;
                blocks.Add(current);
                OnBlockAdded(current);
            }
        }

        private GameObject createBox(Bounds resultBase)
        {
            var box = GameObject.CreatePrimitive(PrimitiveType.Cube);
            box.transform.position = resultBase.center;
            box.transform.localScale = resultBase.size;
            return box;
        }
    }
}
