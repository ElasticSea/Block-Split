using ElasticSea.Framework.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private GameplayManager gameplayManager;
        [SerializeField] private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform.gameObject.AddComponent<EventTrigger>().Click(arg0 =>
            {
                gameplayManager.NextBlock();
            });
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                gameplayManager.NextBlock();
            }
        }
    }
}