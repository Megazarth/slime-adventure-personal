using Kaizen;
using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

namespace Slime
{
    public class PanelFade : SingletonComponent<PanelFade>
    {
        private const float AnimationDuration = 0.25f;

        public CanvasGroup canvasGroup;
        public TextMeshProUGUI textLevel;

        public void Show(Action onComplete = null, bool hideText = false)
        {
            textLevel.gameObject.SetActive(!hideText);
            canvasGroup.DOFade(1.0f, AnimationDuration).From(0f).SetUpdate(true).SetEase(Ease.Linear).OnComplete(() => onComplete?.Invoke());
        }

        public void Hide(Action onComplete = null)
        {
            canvasGroup.DOFade(0.0f, AnimationDuration).From(1.0f).SetUpdate(true).SetEase(Ease.Linear).OnComplete(() => onComplete?.Invoke());
        }
    }
}

