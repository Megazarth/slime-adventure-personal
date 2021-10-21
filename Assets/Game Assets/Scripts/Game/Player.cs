using System;
using UnityEngine;

namespace Slime
{
    public class Player : MonoBehaviour
    {
        protected const float Speed = 150;

        protected Rigidbody2D rb;
        protected Vector2 movement;
        protected Animator anim;
        protected SpriteRenderer spriteRenderer;
        protected BoxCollider2D boxCollider;

        protected CustomJoystick joystick;
#if UNITY_EDITOR
        public int playerId;
#endif
        public AudioClip audioDeath;
        public Action<Player> onDead;
        public bool IsDead { get; set; }

        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            boxCollider = GetComponent<BoxCollider2D>();
            IsDead = false;
        }

        protected virtual void Update()
        {
            if (IsDead)
                return;

            movement = Vector2.zero;
#if UNITY_EDITOR
            movement.x += Input.GetAxisRaw("Horizontal" + playerId);
            movement.y += Input.GetAxisRaw("Vertical" + playerId);
#endif
            movement.x += joystick.Horizontal;
            movement.y += joystick.Vertical;

            spriteRenderer.flipX = movement.x < -0.5f;
            anim.SetFloat("Horizontal", movement.x);
            anim.SetFloat("Vertical", movement.y);
        }

        protected virtual void FixedUpdate()
        {
            if (IsDead)
                return;

            rb.velocity = movement * Speed * Time.fixedDeltaTime;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                Dead();
            }
        }

        public void Dead()
        {
            IsDead = true;
            boxCollider.enabled = false;
            rb.velocity = Vector3.zero;
            onDead?.Invoke(this);
            anim.SetBool("isDead", true);

            AudioManager.Instance.PlaySFX(audioDeath);
        }
    }

}