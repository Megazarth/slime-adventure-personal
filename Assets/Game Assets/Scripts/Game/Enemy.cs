using System;
using System.Collections.Generic;
using UnityEngine;

namespace Slime
{
    public class Enemy : MonoBehaviour
    {
        private const float ChaseResolution = 0.1f;

        [Serializable]
        public struct PatrolData
        {
            [Tooltip("Wait time before moving to next patrol position.")]
            public float waitTime;
            public Vector2 position;
        }
        private enum State
        {
            Idle,
            Patrol,
            Chase,
            Return,
            Knockback,
        }

        private State state = State.Idle;
        private Vector2 targetLastSeen;

        private int patrolIndex;
        private float patrolElapsedWaitTime;
        private Vector2 backtrackPosition;
        private bool isBacktracking;

        private List<Vector2> listChasePositions = new List<Vector2>();

        private int layerButton;

        public float speed;
        public PatrolData[] patrolDatas;

        public Rigidbody2D Rigidbody { get; private set; }
        public List<Player> AllPlayers { get; set; }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            var firstPoint = transform.position;

            for (int i = 0; i < patrolDatas.Length; i++)
            {
                Gizmos.DrawLine(firstPoint, patrolDatas[i].position);
                firstPoint = patrolDatas[i].position;
            }
        }

        private void OnDisable()
        {
            state = State.Idle;
            Rigidbody.velocity = Vector2.zero;
        }

        private void Start()
        {
            layerButton = 1 << LayerMask.NameToLayer("NoDetect");

            Rigidbody = GetComponent<Rigidbody2D>();

            if (patrolDatas.Length != 0)
            {
                state = State.Patrol;
            }
        }

        private bool IsPlayerSighted(Player player)
        {
            var hitResult = Physics2D.Linecast(transform.position, player.transform.position, ~layerButton, 0);

            if (hitResult.collider == null)
                return false;

            if (!hitResult.collider.CompareTag(Global.PlayerTag))
                return false;

            if (player.IsDead)
                return false;

            return true;
        }

        // Update is called once per frame
        private void Update()
        {
            if (AllPlayers == null)
                return;

            var minDistance = float.MaxValue;
            Player closestPlayer = null;

            foreach (var player in AllPlayers)
            {
                if (IsPlayerSighted(player))
                {
                    var distance = (player.transform.position - transform.position).sqrMagnitude;
                    if (distance < minDistance)
                    {
                        distance = minDistance;
                        closestPlayer = player;
                    }
                }
            }

            if (closestPlayer != null)
            {
                targetLastSeen = closestPlayer.transform.position;
                state = State.Chase;
            }
        }

        private void FixedUpdate()
        {
            switch (state)
            {
                case State.Idle:
                    Rigidbody.velocity = Vector2.zero;
                    break;
                case State.Patrol:
                    Patrol();
                    break;
                case State.Chase:
                    Chase();
                    RecordPath();
                    break;
                case State.Return:
                    ReturnToLastPos();
                    break;
                case State.Knockback:
                    Rigidbody.velocity = Vector2.Lerp(Rigidbody.velocity, Vector2.zero, Time.fixedDeltaTime);
                    if (Rigidbody.velocity.sqrMagnitude <= 0.01f)
                        state = State.Idle;
                    break;
                default:
                    break;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.TryGetComponent(out Player player))
            {
                state = State.Knockback;
                var direction = (transform.position - player.transform.position).normalized;
                Rigidbody.velocity = Vector2.zero;
                Rigidbody.AddForce(direction * 2f, ForceMode2D.Impulse);
            }
        }

        private void RecordPath()
        {
            var currentPosition = new Vector2(transform.position.x, transform.position.y);
            if (listChasePositions.Count == 0)
            {
                listChasePositions.Add(currentPosition);
            }
            else if ((listChasePositions[listChasePositions.Count - 1] - currentPosition).sqrMagnitude < ChaseResolution)
            {
                listChasePositions.Add(currentPosition);
            }
        }

        private void Chase()
        {
            //movement
            var direction = (new Vector3(targetLastSeen.x, targetLastSeen.y, 0f) - transform.position).normalized;
            Rigidbody.velocity = speed * direction * Time.fixedDeltaTime;

            //if already arrived, stop chasing
            if ((transform.position - new Vector3(targetLastSeen.x, targetLastSeen.y)).sqrMagnitude < (0.1f * 0.1f))
            {
                Rigidbody.velocity = Vector2.zero;

                if (patrolDatas.Length == 0)
                    state = State.Idle;
                else
                    state = State.Return;
            }
        }

        private void ReturnToLastPos()
        {
            if (listChasePositions.Count == 0)
            {
                state = State.Patrol;
            }
            else if (!isBacktracking)
            {
                backtrackPosition = listChasePositions[listChasePositions.Count - 1];
                listChasePositions.RemoveAt(listChasePositions.Count - 1);
                isBacktracking = true;
            }
            else
            {
                var currentPosition = new Vector2(transform.position.x, transform.position.y);
                var direction = (backtrackPosition - currentPosition).normalized;
                Rigidbody.velocity = speed * direction * Time.fixedDeltaTime;
                var sqrDistance = (backtrackPosition - currentPosition).sqrMagnitude;
                if (sqrDistance < 0.01f)
                {
                    listChasePositions.Remove(backtrackPosition);
                    isBacktracking = false;
                }
            }
        }

        private void Patrol()
        {
            var patrolData = patrolDatas[patrolIndex];
            var currentPosition = new Vector2(transform.position.x, transform.position.y);
            var sqrDistance = (patrolData.position - currentPosition).sqrMagnitude;
            if (sqrDistance > 0.01f)
            {
                var direction = (patrolDatas[patrolIndex].position - currentPosition).normalized;
                Rigidbody.velocity = speed * direction * Time.fixedDeltaTime;
            }
            else if (patrolElapsedWaitTime < patrolData.waitTime)
            {
                Rigidbody.velocity = Vector2.zero;
                patrolElapsedWaitTime += Time.deltaTime;
            }
            else
            {
                patrolElapsedWaitTime -= patrolData.waitTime;
                patrolIndex++;
                patrolIndex = patrolIndex == patrolDatas.Length ? 0 : patrolIndex;
            }
        }
    }

}
