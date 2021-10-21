using Kaizen;
using UnityEngine;


namespace Slime
{
    public class RedButton : MonoBehaviour
    {
        private SingleAnimator singleAnimator;

#if UNITY_EDITOR
        private Color GateCubeColor = new Color(0.0f, 1.0f, 0.0f, 0.5f);
#endif // UNITY_EDITOR
        private int collisionCount;

        public Gate[] gates;
        [Header("Assets")]
        public PoolObject smokeObject;
        public AudioClip pressedClip;
        public AnimationClip clipRedButtonPress;
        public AnimationClip clipRedButtonRelease;

        private void Awake()
        {
            singleAnimator = GetComponent<SingleAnimator>();
        }

        private void SpawnSmoke()
        {
            PoolManager.Instance.Spawn(smokeObject.id, transform.position, Quaternion.identity);
        }

        private void ToggleGates()
        {
            if (gates.Length == 0) { return; }

            for (int i = 0; i < gates.Length; i++)
            {
                gates[i].Toggle();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            collisionCount++;
            if (collisionCount == 1)
            {
                singleAnimator.PlayAnimation(clipRedButtonPress);
                AudioManager.Instance.PlaySFX(pressedClip);
                ToggleGates();
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            collisionCount--;
            if (collisionCount < 1)
            {
                singleAnimator.PlayAnimation(clipRedButtonRelease);
                ToggleGates();
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = GateCubeColor;

            if (gates.Length == 0) { return; }

            for (int i = 0; i < gates.Length; i++)
            {
                if (!gates[i]) { continue; }

                Gizmos.DrawCube(gates[i].transform.position, gates[i].transform.localScale);
            }
        }
#endif // UNITY_EDITOR
    }

}
