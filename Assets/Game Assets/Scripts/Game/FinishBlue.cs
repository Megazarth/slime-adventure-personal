using Kaizen;
using UnityEngine;


namespace Slime
{
	public class FinishBlue : Finish
	{
		[SerializeField] private PoolObject goalTouchObject = null;

		protected override void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.TryGetComponent(out PlayerBlue player))
			{
				IsFinish = true;
				PoolManager.Instance.Spawn(goalTouchObject.id, transform.position, Quaternion.identity);
				AuraParticleSystem.Play();
				onPlayerEnterEvent?.Invoke(player, this);
			}
		}

		protected override void OnTriggerExit2D(Collider2D collision)
		{
			if (collision.GetComponent<PlayerBlue>())
			{
				AuraParticleSystem.Stop();
				IsFinish = false;
			}
		}

	}


}
