using UnityEngine;

namespace Assets.Scripts
{
    public class DifficultyProgression : MonoBehaviour
    {
        [SerializeField] private StackBuilder builder;
        [SerializeField] private float speedIncreaseOverLevel;

        private void Awake()
        {
            builder.OnBlockPlaced += result =>
            {
                builder.Speed = 1 + builder.Count * speedIncreaseOverLevel;
            };
        }
    }
}