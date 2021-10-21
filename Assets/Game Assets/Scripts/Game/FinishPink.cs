using Kaizen;
using UnityEngine;


namespace Slime
{
    public class FinishPink : Finish
    {
        [SerializeField] private PoolObject goalTouchObject = null;

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<PlayerPink>())
            {
                IsFinish = true;
                PoolManager.Instance.Spawn(goalTouchObject.id, transform.position, Quaternion.identity);
                AuraParticleSystem.Play();
                AudioManager.Instance.PlaySFX(PlayerEnterClip);
                onEnter?.Invoke();
            }
        }

        protected override void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.GetComponent<PlayerPink>())
            {
                AuraParticleSystem.Stop();
                IsFinish = false;
            }
        }

    }

}
