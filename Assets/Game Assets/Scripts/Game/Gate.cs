using Kaizen;
using UnityEngine;

namespace Slime
{
    public class Gate : MonoBehaviour
    {
        public enum GateState
        {
            Open,
            Close
        }

        private SingleAnimator animator;
        private BoxCollider2D boxCollider;

        public GateState gateState = GateState.Close;
        [Header("Animation Clips")]
        public AnimationClip clipGateOpen;
        public AnimationClip clipgateClose;
        [Space]
        public Vector3 smokeOpenOffset;
        public PoolObject smokeOpen;
        public Vector3 smokeCloseOffset;
        public PoolObject smokeClose;
        public AudioClip gateClip;

        private void Awake()
        {
            animator = GetComponent<SingleAnimator>();
            boxCollider = GetComponent<BoxCollider2D>();
        }

        private void Start()
        {
            if (gateState == GateState.Open)
                animator.PlayAnimation(clipGateOpen);
        }

        public void SpawnSmoke()
        {
            switch (gateState)
            {
                case GateState.Open:
                    PoolManager.Instance.Spawn(smokeOpen.id, transform.position + smokeOpenOffset, Quaternion.identity);
                    break;
                case GateState.Close:
                    PoolManager.Instance.Spawn(smokeClose.id, transform.position + smokeOpenOffset, Quaternion.identity);
                    break;
                default:
                    PoolManager.Instance.Spawn(smokeOpen.id, transform.position + smokeOpenOffset, Quaternion.identity);
                    break;
            }
        }

        public void Toggle()
        {
            if (gateState == GateState.Open)
                gateState = GateState.Close;
            else
                gateState = GateState.Open;

            AudioManager.Instance.PlaySFX(gateClip);
            switch (gateState)
            {
                case GateState.Open:
                    boxCollider.enabled = false;
                    animator.PlayAnimation(clipGateOpen);
                    break;
                case GateState.Close:
                    boxCollider.enabled = true;
                    animator.PlayAnimation(clipgateClose);
                    break;
                default:
                    animator.PlayAnimation(clipGateOpen);
                    break;
            }
        }
    }

}
