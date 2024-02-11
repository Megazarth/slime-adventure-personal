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

#if UNITY_EDITOR
		public int playerId;
#endif

		public delegate void onOtherEntityContact(Player player, Entity entity);
		public onOtherEntityContact onOtherEntityContactEvent;

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
			{
				return;
			}

#if UNITY_EDITOR
			movement.x += Input.GetAxisRaw("Horizontal" + playerId);
			movement.y += Input.GetAxisRaw("Vertical" + playerId);
#endif

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
			if (collision.gameObject.TryGetComponent(out Entity entity))
			{
				onOtherEntityContactEvent?.Invoke(this, entity);
			}
		}

		public void UpdateInput(Vector2 movement)
		{
			this.movement = movement;
		}

		public void Dead()
		{
			IsDead = true;
			boxCollider.enabled = false;
			rb.velocity = Vector3.zero;
			movement = Vector2.zero;
			anim.SetBool("isDead", true);
		}
	}

}