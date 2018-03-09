using UnityEngine;

namespace Assets.Scripts
{
    public class DifficultyProgression : MonoBehaviour
    {
        [SerializeField] private StackBuilder builder;
        [SerializeField] private float initialSpeed = .75f;
        [SerializeField] private float speedIncreaseOverLevel = .001f;

        private void Awake()
        {
            builder.OnBlockPlaced += result => builder.Speed = initialSpeed + builder.Count * speedIncreaseOverLevel;
        }
    }
}