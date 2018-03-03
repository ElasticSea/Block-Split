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

            // Only works for Z axis
            var ratio = maxDist.z > 0 ? (-maxDist.z + previous.size.z) / previous.size.z : 1 - (previous.size.z + maxDist.z) / previous.size.z;

            if (ratio <= 0 || ratio >= 1)
            {
                return new Result(null, current);
            }
            else
            {
                var minZ = vertsB.Min(c => c.z);
                var maxZ = vertsB.Max(c => c.z);
                var midZ = (maxZ - minZ) * ratio + minZ;
                var lastCurA = vertsB.Select(c => c.z > midZ ? c : c.SetZ(midZ));
                var lastCurB = vertsB.Select(c => c.z < midZ ? c : c.SetZ(midZ));

                var ba = lastCurA.ToArray().ToBounds();
                var bb = lastCurB.ToArray().ToBounds();

                return maxDist.z > 0 ? new Result(bb, ba) : new Result(ba, bb);
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