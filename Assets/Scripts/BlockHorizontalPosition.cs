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
                if (result == StackBuilder.PlacementResult.Placed)
                {
                    var block = builder.Blocks.Last();
                    transform.position = Vector3.up *
                                         Mathf.Max(minHeight,
                                             block.transform.position.y + block.GetComponent<Collider>().bounds.size.y / 2);
                }
            };
        }
    }
}