using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class GameEffects : MonoBehaviour
    {
        [SerializeField] private StackBuilder builder;
        [SerializeField] private GameplayManager gameplay;
        [SerializeField] private BlockRipple ripplePrefab;
        [SerializeField] private AudioSource placeSound;

        private void Awake()
        {
            builder.OnBlockPlaced += result =>
            {
                if (result != StackBuilder.PlacementResult.Miss)
                {
                    if (result == StackBuilder.PlacementResult.Placed)
                    {
                        // Show visual cue
                        var block = builder.Blocks.Last();
                        var ripple = Instantiate(ripplePrefab);
                        ripple.transform.position = block.transform.position;
                        ripple.Animate(block.transform.localScale.x, block.transform.localScale.z, gameplay.Record * .01f);
                    }

                    // Play sound cue
                    placeSound.pitch = 1 + (gameplay.Record * .1f) % 1;
                    placeSound.Play();
                }
            };
        }
    }
}