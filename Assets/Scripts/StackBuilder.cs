using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Core.Extensions;
using DG.Tweening;
using UnityEngine;

namespace Assets.Scripts
{
    public class StackBuilder : MonoBehaviour
    {
        [SerializeField] private Vector3 pedestalSize = new Vector3(1, .5f, 1);

        private List<Collider> blocks = new List<Collider>();
        public List<Collider> Blocks => blocks.ToList();

        private Collider block;
        private Tweener transition;

        public event Action<Collider> OnBlockAdded = block => { };

        private void Start()
        {
            var block = CreateBlock(Vector3.zero, Vector3.zero, pedestalSize, 0);
            PlaceBlock(null, block);
        }

        public void SpawnBlock()
        {
            if (block)
            {
                PlaceBlock(blocks.Last(), block);
            }
            block = CreateBlock(Vector3.forward, Vector3.back, new Vector3(1, .2f, 1), 1);
        }

        private Collider CreateBlock(Vector3 from, Vector3 to, Vector3 size, float speed)
        {
            var block = GameObject.CreatePrimitive(PrimitiveType.Cube).GetComponent<Collider>();
            block.transform.localScale = size;

            var lastBlock = blocks.LastOrDefault();
            var height = (lastBlock ? lastBlock.transform.position.y + lastBlock.bounds.extents.y  : 0) + block.bounds.extents.y;
            block.transform.position = from.SetY(height);
            transition = block.transform.DOMove(to.SetY(height), to.Distance(from) / speed).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
            return block;
        }

        private void PlaceBlock(Collider previous, Collider current)
        {
            transition.Kill();
            current.gameObject.AddComponent<Rigidbody>().isKinematic = true;
            blocks.Add(current);
        }
    }
}
