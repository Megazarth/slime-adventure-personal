using Kaizen;
using UnityEngine;

namespace Slime
{
    public class SpriteParticle : SingleAnimator
    {
        public AnimationClip clip;

        private void OnEnable()
        {
            PlayAnimation(clip, () => PoolManager.Instance.Unspawn(gameObject));
        }
    }
}