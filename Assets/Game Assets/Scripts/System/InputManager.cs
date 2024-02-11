using Kaizen;
using UnityEngine;

namespace Slime
{
	public class InputManager : SceneSingletonComponent<InputManager>
	{
		private Canvas canvas;

		private bool enableInput;

		private PlayerBlue playerBlue;
		private PlayerPink playerPink;

		public CustomJoystick joystickBlue;
		public CustomJoystick joystickPink;

		protected override void Awake()
		{
			base.Awake();
			canvas = GetComponentInChildren<Canvas>();
			HideControls();
		}

		protected void Update()
		{
			if (!enableInput)
				return;

			if (playerBlue != null)
				playerBlue.UpdateInput(new Vector2(joystickBlue.Horizontal, joystickBlue.Vertical));

			if (playerPink != null)
				playerPink.UpdateInput(new Vector2(joystickPink.Horizontal, joystickPink.Vertical));
		}

		public void HideControls()
		{
			enableInput = false;

			canvas.enabled = false;
			Input.multiTouchEnabled = false;

			joystickBlue.enabled = false;
			joystickPink.enabled = false;
		}

		public void ShowControls()
		{
			enableInput = true;

			canvas.enabled = true;
			Input.multiTouchEnabled = true;

			joystickBlue.enabled = true;
			joystickPink.enabled = true;
		}

		public void AssignPlayer(Player player)
		{
			if (player is PlayerBlue playerBlue)
				this.playerBlue = playerBlue;
			if (player is PlayerPink playerPink)
				this.playerPink = playerPink;
		}

		public void RemoveAllPlayers()
		{
			playerBlue = null;
			playerPink = null;
		}

		public void SetSinglePlayerMode()
		{

		}

		public void SetMultiplayerModeHostIsPlayerBlue()
		{

		}

		public void SetMultiplayerModeHostIsPlayerPink()
		{

		}
	}

}
