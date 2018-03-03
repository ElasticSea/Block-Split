using UnityEngine;

namespace Assets.Scripts
{
    public class GameplayManager : MonoBehaviour
    {
        [SerializeField] private StackBuilder builder;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                builder.SpawnBlock();
            }
        }
    }
}