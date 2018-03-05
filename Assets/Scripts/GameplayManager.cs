using System;
using Assets.Core.Scripts;
using UnityEngine;

namespace Assets.Scripts
{
    public class GameplayManager : MonoBehaviour
    {
        [SerializeField] private StackBuilder builder;
        [SerializeField] private int colorGradientLength;

        private int blockCount = 1;
        private Color color;
        private int record;

        private void Awake()
        {
            Color colorA = Utils.RandomColor() / 2f + Color.white * .5f;
            Color colorB = Utils.RandomColor() / 2f + Color.white * .5f;

            Colorize(colorA);
            builder.OnBlockAdded += result =>
            {
                if (result.Success == false)
                {
                    record = 0;
                    return;
                }

                if (blockCount > colorGradientLength)
                {
                    colorA = colorB;
                    colorB = Utils.RandomColor();
                    blockCount = 1;
                }
                Colorize(Color.Lerp(colorA, colorB, blockCount / (float) colorGradientLength));
                blockCount++;
                record++;

                if (record >= 2)
                {
                    builder.extendBlock(builder.Direction);
                }
            };
        }

        private void Colorize(Color color)
        {
            this.color = color;
            builder.BlockColor = color;
            builder.CutoutColor = color;
        }

        private void Update()
        {
            Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, color.TransformHSV(0, .4f, .3f), Time.deltaTime);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                builder.SpawnBlock();
            }
        }
    }
}