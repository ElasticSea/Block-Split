using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class GameplayManager : MonoBehaviour
    {
        [SerializeField] private StackBuilder builder;
        [SerializeField] private int colorGradientLength;
        [SerializeField] private int expandThreshold;

        private Color color;
        private int record;
        private bool gameover;

        private void Awake()
        {
            builder.BlockColor = Color;
            builder.CutoutColor = Color;
            builder.OnBlockPlaced += result =>
            {
                builder.BlockColor = Color;
                builder.CutoutColor = Color;

                if (result == StackBuilder.PlacementResult.Partial)
                {
                    record = 0;
                }
                else if(result == StackBuilder.PlacementResult.Placed)
                {
                    record++;
                }else if (result == StackBuilder.PlacementResult.Miss)
                {
                    gameover = true;
                }

                if (record >= expandThreshold)
                {
                    builder.Extend();
                }
            };
        }

        private Color startingColor = Color.red.TransformHSV((float) (360 * new System.Random().NextDouble()), 1, 1);
        private Color Color => startingColor.TransformHSV(colorGradientLength * 1.6180339887f * builder.Blocks.Count, 1, 1);

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                builder.PlaceBlock();

                if (gameover == false)
                {
                    builder.SpawnBlock();
                }
            }
        }
    }
}