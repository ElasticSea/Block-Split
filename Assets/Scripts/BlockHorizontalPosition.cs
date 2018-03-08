using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class BlockHorizontalPosition : MonoBehaviour
    {
        [SerializeField] private StackBuilder builder;
        [SerializeField] private float minHeight;

        private void Awake()
        {
            builder.OnBlockPlaced += result =>
            {
                var block = builder.Blocks.Last();
                var height = block.transform.position.y + block.GetComponent<Collider>().bounds.size.y / 2;
                transform.position = Vector3.up * Mathf.Max(minHeight, height);
            };
        }
    }
}