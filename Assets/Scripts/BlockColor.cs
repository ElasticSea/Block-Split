using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class BlockColor : MonoBehaviour
    {
        private const float GOLDEN_RATIO = 1.6180339887f;

        [SerializeField] private StackBuilder builder;
        [SerializeField] private int colorMultiplier;

        private void Awake()
        {
            builder.BlockColor = Color;
            builder.CutoutColor = Color;
            builder.OnBlockPlaced += result =>
            {
                builder.BlockColor = Color;
                builder.CutoutColor = Color;
            };
        }

        private Color startingColor = Color.red.TransformHSV((float)(360 * new System.Random().NextDouble()), 1, 1);
        private Color Color => startingColor.TransformHSV(colorMultiplier * GOLDEN_RATIO * builder.Count, 1, 1);
    }
}