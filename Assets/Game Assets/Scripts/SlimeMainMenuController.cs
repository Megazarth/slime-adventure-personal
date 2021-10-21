using UnityEngine;

namespace Slime
{
    public class SlimeMainMenuController : Player
    {
        private enum AppearDirection
        {
            Left = 1,
            Right = -1,
        }

        private AppearDirection direction = AppearDirection.Left;

        private Vector3 worldMinPos;
        private Vector3 worldMaxPos;

        private float cruisingSpeed;

        private void Start()
        {
            boxCollider.enabled = false;

            //Bottom-Left
            worldMinPos = CameraManager.Instance.MainCamera.ViewportToWorldPoint(new Vector3(0f, 0f, CameraManager.Instance.MainCamera.nearClipPlane));
            worldMinPos.x -= spriteRenderer.bounds.extents.x * 2f;
            worldMinPos.y += spriteRenderer.bounds.extents.y * 2f;
            //Top-Right
            worldMaxPos = CameraManager.Instance.MainCamera.ViewportToWorldPoint(new Vector3(1f, 1f, CameraManager.Instance.MainCamera.nearClipPlane));
            worldMaxPos.x += spriteRenderer.bounds.extents.x * 2f;
            worldMaxPos.y -= spriteRenderer.bounds.extents.y * 2f;

            RandomizePositon();
        }

        private void RandomizePositon()
        {
            var yPos = Random.Range(worldMinPos.y, worldMaxPos.y);
            float xPos;
            if (direction == AppearDirection.Left)
            {
                xPos = worldMinPos.x;
                movement = Vector2.right;
            }
            else
            {
                xPos = worldMaxPos.x;
                movement = Vector2.left;
            }

            transform.position = new Vector3(xPos, yPos);
            cruisingSpeed = Random.Range(90f, Speed);
        }

        private void CheckPosition()
        {
            if (direction == AppearDirection.Left)
            {
                if (transform.position.x > worldMaxPos.x)
                {
                    direction = AppearDirection.Right;
                    RandomizePositon();
                }
            }
            else
            {
                if (transform.position.x < worldMinPos.x)
                {
                    direction = AppearDirection.Left;
                    RandomizePositon();
                }
            }
        }

        protected override void Update()
        {
            spriteRenderer.flipX = movement.x < -0.5f;
            anim.SetFloat("Horizontal", movement.x);
            anim.SetFloat("Vertical", movement.y);

            CheckPosition();
        }

        protected override void FixedUpdate()
        {
            rb.velocity = movement * cruisingSpeed * Time.fixedDeltaTime;
        }
    }
}