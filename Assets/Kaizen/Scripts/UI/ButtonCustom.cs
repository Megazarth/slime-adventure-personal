using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Kaizen
{
    public class ButtonCustom : Button
    {
#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            this.transition = Transition.SpriteSwap;
        }
#endif
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (interactable)
                AppManager.Instance.onButtonCustomClicked.Invoke(this);
        }
    }
}