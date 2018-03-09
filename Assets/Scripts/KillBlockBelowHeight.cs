using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts
{
    public class KillBlockBelowHeight : MonoBehaviour
    {
        [SerializeField] private float height;
        [SerializeField] private UnityAction<GameObject> onBelowHeight;

        public float Height
        {
            set { height = value; }
        }

        public UnityAction<GameObject> OnBelowHeight
        {
            set { onBelowHeight = value; }
        }

        private void Update()
        {
            if (transform.position.y < height)
            {
                onBelowHeight(gameObject);
            }
        }
    }
}