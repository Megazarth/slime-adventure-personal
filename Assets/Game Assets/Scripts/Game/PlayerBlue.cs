namespace Slime
{
    public class PlayerBlue : Player
    {
        private void Start()
        {
            joystick = InputManager.Instance.joystickBlue;
        }
    }

}
