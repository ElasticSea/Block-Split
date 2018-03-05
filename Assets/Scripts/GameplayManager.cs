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

        private void Awake()
        {
            builder.BlockColor = Color;
            builder.CutoutColor = Color;
            builder.OnBlockPlaced += sucess =>
            {
                builder.BlockColor = Color;
                builder.CutoutColor = Color;

                if (sucess == false)
                {
                    record = 0;
                }
                else
                {
                    record++;
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
            if (Input.GetKeyDown(KeyCode.Space))
            {
                builder.SpawnBlock();
            }
        }
    }
}