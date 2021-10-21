using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Kaizen
{
    [RequireComponent(typeof(Animator))]
    public class SingleAnimator : MonoBehaviour
    {
        protected PlayableGraph playableGraph;
        protected AnimationPlayableOutput playableOutput;
        protected Dictionary<AnimationClip, AnimationClipPlayable> animationClipDict = new Dictionary<AnimationClip, AnimationClipPlayable>();

        protected Coroutine coroutineOnAnimationCompleted;

        protected virtual void Awake()
        {
            playableGraph = PlayableGraph.Create();

            playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

            playableOutput = AnimationPlayableOutput.Create(playableGraph, $"{gameObject.name}_Animator", GetComponent<Animator>());
        }

        protected virtual void OnDestroy()
        {
            playableGraph.Destroy();
        }

        public virtual void PlayAnimation(AnimationClip clip, Action onComplete = null)
        {
            if (!animationClipDict.TryGetValue(clip, out AnimationClipPlayable clipPlayable))
            {
                clipPlayable = AnimationClipPlayable.Create(playableGraph, clip);
                animationClipDict.Add(clip, clipPlayable);
            }

            clipPlayable.SetTime(0);
            clipPlayable.SetTime(0);
            playableOutput.SetSourcePlayable(clipPlayable);
            playableGraph.Play();

            OnAnimationComplete(clip, () =>
            {
                playableGraph.Stop();
                onComplete?.Invoke();
            });
        }

        protected virtual void OnAnimationComplete(AnimationClip clip, Action onComplete = null)
        {
            if (coroutineOnAnimationCompleted != null)
                StopCoroutine(coroutineOnAnimationCompleted);

            coroutineOnAnimationCompleted = StartCoroutine(OnAnimationCompleteCoroutine(clip, onComplete));
        }

        protected IEnumerator OnAnimationCompleteCoroutine(AnimationClip clip, Action onComplete = null)
        {
            while (playableOutput.GetSourcePlayable().GetTime() < clip.length)
            {
                yield return null;
            }
            onComplete?.Invoke();
        }
    }
}

