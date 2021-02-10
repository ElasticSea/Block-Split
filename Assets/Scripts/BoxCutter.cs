using System;
using System.Linq;
using ElasticSea.Framework.Extensions;
using UnityEngine;

namespace Assets.Scripts
{
    public class BoxCutter
    {
        public static Result Cut(Bounds blockBelow, Bounds blockToCut)
        {
            var vertsA = blockBelow.GetVertices();
            var vertsB = blockToCut.GetVertices();
            Vector3 offsetVec = vertsB[0].SetY(0) - vertsA[0].SetY(0);

            // Blocks are on top of each, they overlap each other completely other there is no cut.
            if (offsetVec == Vector3.zero) return new Result(blockToCut, null);

            var getAxis = createGetter(offsetVec);
            var setAxis = createSetter(offsetVec);

            var cutRatio = getAxis(offsetVec) > 0
                ? (getAxis(blockBelow.size) - getAxis(offsetVec)) / getAxis(blockBelow.size)
                : 1 - (getAxis(blockBelow.size) + getAxis(offsetVec)) / getAxis(blockBelow.size);

            // Blocks are outside of each bounds there is no cut
            if (cutRatio <= 0 || cutRatio >= 1) return new Result(null, blockToCut);

            var min = vertsB.Min(c => getAxis(c));
            var max = vertsB.Max(c => getAxis(c));
            var mid = (max - min) * cutRatio + min;
            var cutA = vertsB.Select(c => getAxis(c) > mid ? c : setAxis(c, mid)).ToArray().ToBounds();
            var cutB = vertsB.Select(c => getAxis(c) < mid ? c : setAxis(c, mid)).ToArray().ToBounds();

            // Blocks are cut where they overlap.
            return getAxis(offsetVec) > 0 ? new Result(cutB, cutA) : new Result(cutA, cutB);
        }

        private static Func<Vector3, float> createGetter(Vector3 maxDist)
        {
            if (maxDist.normalized == Vector3.forward || maxDist.normalized == Vector3.back)
            {
                return v => v.z;
            }
            if (maxDist.normalized == Vector3.right || maxDist.normalized == Vector3.left)
            {
                return v => v.x;
            }
            throw new InvalidOperationException("Axis not supported: " + maxDist);
        }

        private static Func<Vector3, float, Vector3> createSetter(Vector3 maxDist)
        {
            if (maxDist.normalized == Vector3.forward || maxDist.normalized == Vector3.back)
            {
                return (v, z) => v.SetZ(z);
            }
            if (maxDist.normalized == Vector3.right || maxDist.normalized == Vector3.left)
            {
                return (v, x) => v.SetX(x);
            }
            throw new InvalidOperationException("Axis not supported: " + maxDist);
        }

        public class Result
        {
            public Result(Bounds? @base, Bounds? cutout)
            {
                Base = @base;
                Cutout = cutout;
            }

            public Bounds? Base { get; }
            public Bounds? Cutout { get; }
        }
    }
}