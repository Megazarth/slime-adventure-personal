using UnityEngine;
using UnityEngine.UI;

namespace Slime
{
    public class TransitionPiece : MonoBehaviour
    {
        private Image[] images;

        public RectTransform rectPiece;
        public RectTransform rectTail;
        public RectTransform rectTransform;

        private void Awake()
        {
            images = GetComponentsInChildren<Image>();
        }

        public void SetHeight(float size)
        {
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, size);
            rectPiece.sizeDelta = new Vector2(size, size);
            rectTail.sizeDelta = new Vector2(rectTail.sizeDelta.x, size);
        }

        public void SetTailLength(float length)
        {
            rectTail.sizeDelta = new Vector2(length, rectTail.sizeDelta.y);
            rectTransform.sizeDelta = new Vector2(rectPiece.sizeDelta.x + length, rectTransform.sizeDelta.y);
        }

        public void SetTransitionColor(Color color)
        {
            foreach (var image in images)
            {
                image.color = color;
            }
        }
    }
}