using UnityEngine;

namespace Assets.Scripts
{
    public class BlockHorizontalPosition : MonoBehaviour
    {
        [SerializeField] private StackBuilder builder;

        private void Awake()
        {
            builder.OnBlockAdded += block =>
            {
                transform.position += Vector3.up * block.bounds.size.y;
            };
        }
    }
}