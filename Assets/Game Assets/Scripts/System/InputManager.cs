using Kaizen;
using UnityEngine;

namespace Slime
{
	public class InputManager : SceneSingletonComponent<InputManager>
	{
		private Canvas canvas;

		private bool enableInput;
		private bool multiplayerMode;

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
			if (multiplayerMode)
				return;

			if (!enableInput)
				return;

			if (playerBlue != null)
				playerBlue.UpdateInput(GetJoystickBlueDirection());

			if (playerPink != null)
				playerPink.UpdateInput(GetJoystickPinkDirection());
		}

		public Vector2 GetJoystickBlueDirection()
		{
			return new Vector2(joystickBlue.Horizontal, joystickBlue.Vertical);
		}

		public Vector2 GetJoystickPinkDirection()
		{
			return new Vector2(joystickPink.Horizontal, joystickPink.Vertical);
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

		public void SetMultiplayerMode(bool state)
		{
			multiplayerMode = state;
		}

		public void SetSinglePlayerMode()
		{
			joystickBlue.gameObject.SetActive(true);
			joystickPink.gameObject.SetActive(true);

			joystickBlue.ResetAnchor();
			joystickPink.ResetAnchor();
		}

		public void SetMultiplayerModeHostIsPlayerBlue()
		{
			joystickBlue.gameObject.SetActive(true);
			joystickPink.gameObject.SetActive(true);
			joystickPink.gameObject.SetActive(false);

			joystickBlue.SetAnchorForMultiplayer();
		}

		public void SetMultiplayerModeHostIsPlayerPink()
		{
			joystickBlue.gameObject.SetActive(true);
			joystickPink.gameObject.SetActive(true);
			joystickBlue.gameObject.SetActive(false);

			joystickPink.SetAnchorForMultiplayer();
		}
	}

}
