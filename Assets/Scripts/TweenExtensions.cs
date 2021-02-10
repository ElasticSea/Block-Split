using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using ElasticSea.Framework.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace _Framework.Scripts.Extensions
{
    public static class TweenExtensions
    {
        public static TweenerCore<float, float, FloatOptions> DoFade(this CanvasGroup canvasGroup, float endValue, float duration)
        {
            return DOTween.To(() => canvasGroup.alpha, value => canvasGroup.alpha = value, endValue, duration);
        }

        public static TweenerCore<float, float, FloatOptions> DoWidth(this RectTransform rectTransform, float endValue, float duration)
        {
            return DOTween.To(rectTransform.GetWidth, rectTransform.SetWidth, endValue, duration);
        }

        public static TweenerCore<float, float, FloatOptions> DoHeight(this RectTransform rectTransform, float endValue, float duration)
        {
            return DOTween.To(rectTransform.GetHeight, rectTransform.SetHeight, endValue, duration);
        }

        public static TweenerCore<float, float, FloatOptions> DOAnchorPosX(this RectTransform rectTransform, float endValue, float duration)
        {
            return DOTween.To(() => rectTransform.anchoredPosition.x, pos => rectTransform.anchoredPosition = rectTransform.anchoredPosition.SetX(pos), endValue, duration);
        }

        public static Tweener DOFade(this IEnumerable<Graphic> target, float endValue, float duration)
        {
            return DOTween.ToAlpha((DOGetter<Color>) (() => target.First().color), (DOSetter<Color>) (x => target.Foreach(t => t.color = x)), endValue, duration);
        }
        
        public static Tweener DOFade(this IEnumerable<Material> materials, float endValue, float duration)
        {
            var mats = materials.Where(m => m.HasProperty("_Color")).ToArray();
            var value = mats.FirstOrDefault()?.color.a ?? 0;

            Action<float> SetMaterialAlpha = v =>
            {
                value = v;
                for (var i = 0; i < mats.Length; i++)
                {
                    
                    mats[i].color = mats[i].color.SetAlpha(value);
                }
            };

            return DOTween.To(() => value, v => SetMaterialAlpha(v), endValue, duration);
        }
    }
}