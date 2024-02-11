using UnityEngine;

namespace Kaizen
{
    [RequireComponent(typeof(Canvas))]
    public class InputBlocker : SingletonComponent<InputBlocker>
    {
        private Canvas canvas;

        public int SortingOrder { get => canvas.sortingOrder; }

        private void OnValidate()
        {
            if (canvas == null)
                canvas = GetComponent<Canvas>();

            canvas.sortingOrder = 10000;
        }

        protected override void Awake()
        {
            base.Awake();
            canvas = GetComponent<Canvas>();
            canvas.enabled = false;
        }

        public void DisableInput()
        {
            canvas.enabled = true;
        }

        public void EnableInput()
        {
            canvas.enabled = false;
        }
    }
}