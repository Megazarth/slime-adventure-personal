using Kaizen;
using UnityEngine;

namespace Slime
{
    public class InputManager : SceneSingletonComponent<InputManager>
    {
        private Canvas canvas;

        public CustomJoystick joystickBlue;
        public CustomJoystick joystickPink;

        protected override void Awake()
        {
            base.Awake();
            canvas = GetComponentInChildren<Canvas>();
            HideControls();
        }

        public void HideControls()
        {
            canvas.enabled = false;
            Input.multiTouchEnabled = false;

            joystickBlue.enabled = false;
            joystickPink.enabled = false;
        }

        public void ShowControls()
        {
            canvas.enabled = true;
            Input.multiTouchEnabled = true;

            joystickBlue.enabled = true;
            joystickPink.enabled = true;
        }
    }

}
