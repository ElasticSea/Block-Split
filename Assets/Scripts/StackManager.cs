using UnityEngine;

namespace Assets.Scripts
{
    public class StackManager : MonoBehaviour
    {
        [SerializeField] private BlockBuilder builder;

        private void Start()
        {
            builder.SpawnBlock(Vector3.forward, Vector3.back, new Vector3(1, .2f, 1), 1);
        }
    }
}