using UnityEngine;
using UnityEngine.EventSystems;

namespace Slime
{
    public class CustomJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        private Vector2 tapPosition;
        private Vector2 inputValue;

        private bool willReceiveInput;

        public RectTransform rectInputField;
        public RectTransform background;
        public RectTransform handle;
        [Space]
        public float radius;
        [Range(0f, 1f)]
        public float deadZone;

        public float Horizontal { get => inputValue.x; }
        public float Vertical { get => inputValue.y; }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 1f, 1f, 0.5f);
            Gizmos.DrawSphere(background.transform.position, radius * deadZone);
        }

        private void OnEnable()
        {
            ResetPosition();
            willReceiveInput = true;
        }

        private void OnDisable()
        {
            ResetPosition();
            willReceiveInput = false;
        }

        private void Awake()
        {
            background.gameObject.SetActive(false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!willReceiveInput)
                return;

            background.gameObject.SetActive(true);
            tapPosition = eventData.position;
            /*
            var minPosition = rectInputField.anchorMin * new Vector2(Screen.width, Screen.height);
            var position = eventData.position - Vector2.right * minPosition;
            */
            background.position = tapPosition;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!willReceiveInput)
                return;

            var position = eventData.position - tapPosition;

            position = Vector2.ClampMagnitude(position, radius);
            handle.anchoredPosition = position;

            var deadZoneValue = radius * deadZone;
            if (position.sqrMagnitude > (deadZoneValue * deadZoneValue))
                inputValue = position.normalized;
            else
                inputValue = Vector2.zero;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!willReceiveInput)
                return;

            background.gameObject.SetActive(false);
            handle.anchoredPosition = Vector2.zero;
            inputValue = Vector2.zero;
        }

        public void ResetPosition()
        {
            background.gameObject.SetActive(false);

            background.position = Vector3.zero;
            handle.anchoredPosition = Vector2.zero;
            inputValue = Vector2.zero;
        }
    }
}