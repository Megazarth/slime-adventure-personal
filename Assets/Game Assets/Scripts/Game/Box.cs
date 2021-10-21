using UnityEngine;

namespace Slime
{
    public class Box : MonoBehaviour
    {
        private new Rigidbody2D rigidbody;
        private float elapsedTime;

        private bool isBeingPushed;

        public AudioClip audioBoxMoved;
        public float cooldown;

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            elapsedTime += Time.deltaTime;

            if (rigidbody.velocity.sqrMagnitude > 0f && elapsedTime >= cooldown && isBeingPushed)
            {
                elapsedTime = 0f;
                AudioManager.Instance.PlaySFX(audioBoxMoved);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            isBeingPushed = collision.gameObject.TryGetComponent(out Player _);
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.TryGetComponent(out Player _))
                isBeingPushed = false;
        }
    }
}