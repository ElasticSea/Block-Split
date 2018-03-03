using Assets.Core.Scripts;
using UnityEngine;

namespace Assets.Scripts
{
    public class KillBlockBelowHeight : MonoBehaviour
    {
        public float Height { get; set; }
        public Pool<GameObject> Pool { private get; set; }

        private void Update()
        {
            if (transform.position.y < Height)
            {
                Pool.Put(gameObject);
            }
        }
    }
}