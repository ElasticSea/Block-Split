using Assets.Core.Extensions;
using Assets.Core.Scripts.Extensions;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class BlockRipple : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;

        [SerializeField] private RectTransform left;
        [SerializeField] private RectTransform right;
        [SerializeField] private RectTransform top;
        [SerializeField] private RectTransform bottom;

        private void Awake()
        {
            canvasGroup.alpha = 0;
            canvasGroup.DoFade(1, .3f).SetLoops(2, LoopType.Yoyo);
        }

        public void Animate(float width, float height, float thickens)
        {
            left.SetWidth(thickens);
            right.SetWidth(thickens);
            top.SetHeight(thickens);
            bottom.SetHeight(thickens);

            left.offsetMin = new Vector2(left.offsetMin.x, -thickens);
            left.offsetMax = new Vector2(left.offsetMax.x, -thickens);
            left.SetHeight(1 + 2 * thickens);

            right.offsetMin = new Vector2(right.offsetMin.x, -thickens);
            right.offsetMax = new Vector2(right.offsetMax.x, -thickens);
            right.SetHeight(1 + 2 * thickens);

            canvasGroup.GetComponent<RectTransform>().SetWidth(width);
            canvasGroup.GetComponent<RectTransform>().SetHeight(height);

            DOTween.To(() => canvasGroup.GetComponent<RectTransform>().GetWidth(), x => canvasGroup.GetComponent<RectTransform>().SetWidth(x), canvasGroup.GetComponent<RectTransform>().GetWidth() + .4f, .8f).SetEase(Ease.Linear);
            DOTween.To(() => canvasGroup.GetComponent<RectTransform>().GetHeight(), x => canvasGroup.GetComponent<RectTransform>().SetHeight(x), canvasGroup.GetComponent<RectTransform>().GetHeight() + .4f, .8f).SetEase(Ease.Linear);
        }
    }
}