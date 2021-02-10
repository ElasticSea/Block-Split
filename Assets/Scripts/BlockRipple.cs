using _Framework.Scripts.Extensions;
using DG.Tweening;
using ElasticSea.Framework.Extensions;
using UnityEngine;

namespace Assets.Scripts
{
    public class BlockRipple : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;

        [SerializeField] private RectTransform left;
        [SerializeField] private RectTransform right;
        [SerializeField] private RectTransform top;
        [SerializeField] private RectTransform bottom;

        public void Animate(float width, float height, float thickens)
        {
            canvasGroup.alpha = 0;
            canvasGroup.DoFade(1, .3f).SetEase(Ease.InQuad).SetLoops(2, LoopType.Yoyo);

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

            var canvasRect = canvasGroup.GetComponent<RectTransform>();
            canvasRect.SetWidth(width);
            canvasRect.SetHeight(height);

            canvasRect.DoWidth(canvasRect.GetWidth() + .4f, .8f);
            canvasRect.DoHeight(canvasRect.GetHeight() + .4f, .8f);
        }
    }
}