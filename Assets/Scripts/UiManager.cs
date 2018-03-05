using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class UiManager : MonoBehaviour
    {
        [SerializeField] private StackBuilder stack;
        [SerializeField] private Text score;

        private void Awake()
        {
            stack.OnBlockAdded += result =>
            {
                score.text = stack.Blocks.Count.ToString();
            };
        }
    }
}