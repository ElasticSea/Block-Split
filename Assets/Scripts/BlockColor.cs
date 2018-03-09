using System;
using UnityEngine;
using Random = System.Random;

namespace Assets.Scripts
{
    public class BlockColor : MonoBehaviour
    {
        private const float GOLDEN_RATIO = 1.6180339887f;

        [SerializeField] private StackBuilder builder;
        [SerializeField] private float colorMultiplier;

        private Color startingColor = Color.red.TransformHSV((float) (360 * new Random().NextDouble()), 1, 1);
        private Color Color => startingColor.TransformHSV(colorMultiplier * 360 * GOLDEN_RATIO * builder.Count, 1, 1);

        private void Awake()
        {
            builder.OnBlockPlaced += result =>
            {
                builder.BlockColor = Color;
                builder.CutoutColor = Color;
            };
            builder.BlockColor = Color;
            builder.CutoutColor = Color;
        }
    }
}