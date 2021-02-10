using _Framework.Scripts.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class UiManager : MonoBehaviour
    {
        [SerializeField] private StackBuilder stack;
        [SerializeField] private Text score;
        [SerializeField] private Text score2;
        [SerializeField] private Text gameOver;
        [SerializeField] private Text retry;

        private void Awake()
        {
            retry.gameObject
                 .AddComponent<EventTrigger>()
                 .PointerClick(d => { SceneManager.LoadScene(SceneManager.GetActiveScene().name); });

            stack.OnBlockPlaced += result =>
            {
                score.text = stack.Count.ToString();
                score2.text = stack.Count.ToString();

                if (result == StackBuilder.PlacementResult.Miss)
                {
                    score.gameObject.SetActive(false);
                    gameOver.gameObject.SetActive(true);
                }
                else
                {
                    score.gameObject.SetActive(true);
                    gameOver.gameObject.SetActive(false);
                }
            };
        }
    }
}