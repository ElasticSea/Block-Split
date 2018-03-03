using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class BlockBuilder : MonoBehaviour
    {
        public IEnumerable<Collider> Blocks => Enumerable.Empty<Collider>();

        public event Action<Collider> OnBlockAdded = block => { };
    }
}
