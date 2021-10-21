using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Slime
{
    public class UILevelButton : MonoBehaviour
    {
        private static float AnimationDuration = 1.5f;
        private static float AnimationLoopDelay = 2.5f;

        private Sequence sequence;

        public TextMeshProUGUI numberText;
        public Button button;
        public RectTransform rectBadge;
        public RectTransform rectShiny;

        private void OnDestroy()
        {
            sequence?.Kill();
        }

        private void PlayShinyAnimation()
        {
            if (sequence == null)
            {
                sequence = DOTween.Sequence();
                sequence.SetAutoKill(false);
                sequence.SetLoops(-1);
                sequence.SetDelay(AnimationLoopDelay);

                sequence.Insert(0f, rectShiny.DOAnchorPosX(rectShiny.sizeDelta.x, AnimationDuration).From(rectShiny.sizeDelta.x * Vector2.left));
            }
            else
            {
                sequence.Restart();
            }
        }

        private void PauseShinyAnimation()
        {
            sequence?.Pause();
        }

        public void ToggleCompletionBadge(bool state)
        {
            rectBadge.gameObject.SetActive(state);
            if (state)
                PlayShinyAnimation();
            else
                PauseShinyAnimation();
        }
    }
}
