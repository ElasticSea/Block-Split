using System;
using System.Linq;
using Assets.Core.Extensions;
using Assets.Core.Scripts.Extensions;
using UnityEngine;

namespace Assets.Scripts
{
    public class BoxCutter : MonoBehaviour
    {
        public static Result Cut(Bounds previous, Bounds current)
        {
            var vertsA = previous.GetVertices();
            var vertsB = current.GetVertices();

            var maxDist = Vector3.zero;
            for (var i = 0; i < vertsA.Length; i++)
            {
                var newDist = vertsB[i].SetY(0) - vertsA[i].SetY(0);
                if (newDist.magnitude > maxDist.magnitude)
                    maxDist = newDist;
            }

            Func<Vector3, float> getAxis;
            Func<Vector3, float, Vector3> setAxis;

            if (maxDist == Vector3.zero)
            {
                return new Result(current, null);
            }

            if (maxDist.normalized == Vector3.forward || maxDist.normalized == Vector3.back)
            {
                getAxis = v => v.z;
                setAxis = (v, z) => v.SetZ(z);
            }
            else if (maxDist.normalized == Vector3.right || maxDist.normalized == Vector3.left)
            {

                getAxis = v => v.x;
                setAxis = (v, x) => v.SetX(x);
            }
            else
            {
                throw new InvalidOperationException("Axis not supported: "+ maxDist);
            }

            var ratio = getAxis(maxDist) > 0
                ? (-getAxis(maxDist) + getAxis(previous.size)) / getAxis(previous.size)
                : 1 - (getAxis(previous.size) + getAxis(maxDist)) / getAxis(previous.size);

            if (ratio <= 0 || ratio >= 1)
            {
                return new Result(null, current);
            }
            else
            {
                var min = vertsB.Min(c => getAxis(c));
                var max = vertsB.Max(c => getAxis(c));
                var mid = (max - min) * ratio + min;
                var lastCurA = vertsB.Select(c => getAxis(c) > mid ? c : setAxis(c,mid));
                var lastCurB = vertsB.Select(c => getAxis(c) < mid ? c : setAxis(c,mid));

                var ba = lastCurA.ToArray().ToBounds();
                var bb = lastCurB.ToArray().ToBounds();

                return getAxis(maxDist) > 0 ? new Result(bb, ba) : new Result(ba, bb);
            }
        }

        public class Result
        {
            public Bounds? Base { get; }
            public Bounds? Cutout { get; }

            public Result(Bounds? @base, Bounds? cutout)
            {
                Base = @base;
                Cutout = cutout;
            }
        }
    }
}