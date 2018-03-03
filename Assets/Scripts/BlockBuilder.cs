using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Core.Extensions;
using DG.Tweening;
using UnityEngine;

namespace Assets.Scripts
{
    public class BlockBuilder : MonoBehaviour
    {
        [SerializeField] private Vector3 pedestalSize = new Vector3(1, .5f, 1);

        private List<Collider> blocks = new List<Collider>();
        public List<Collider> Blocks => blocks.ToList();

        public event Action<Collider> OnBlockAdded = block => { };

        private void Start()
        {
            var pedestal = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pedestal.AddComponent<Rigidbody>();
            pedestal.transform.localScale = pedestalSize;
            pedestal.transform.localPosition = Vector3.up * pedestal.GetComponent<Collider>().bounds.extents.y;

            PlaceBlock(pedestal.GetComponent<Collider>());
        }

        public void SpawnBlock(Vector3 from, Vector3 to, Vector3 size, float speed)
        {
            var block = GameObject.CreatePrimitive(PrimitiveType.Cube);
            block.transform.localScale = size;

            var lastBlock = blocks.Last();
            var height = lastBlock.transform.position.y + lastBlock.bounds.extents.y + block.GetComponent<Collider>().bounds.extents.y;
            block.transform.position = from.SetY(height);
            block.transform.DOMove(to.SetY(height), to.Distance(from) / speed).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        }

        private void PlaceBlock(Collider block)
        {
            block.GetComponent<Rigidbody>().isKinematic = true;
            blocks.Add(block);
        }
    }
}
