using UnityEngine;

namespace Assets.Scripts
{
    public class BlockHorizontalPosition : MonoBehaviour
    {
        [SerializeField] private StackBuilder builder;
        [SerializeField] private float minHeight;

        private void Awake()
        {
            builder.OnBlockAdded += result =>
            {
                if (result.Success)
                {
                    transform.position = Vector3.up *
                                         Mathf.Max(minHeight,
                                             result.Block.transform.position.y + result.Block.GetComponent<Collider>().bounds.size.y / 2);
                }
            };
        }
    }
}