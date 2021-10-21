using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

namespace Kaizen
{
    public static class Extensions
    {
        public static Coroutine OnSmoothRewindCompleted(this Sequence sequence, MonoBehaviour monoBehaviour, bool autoKill, Action onComplete = null)
        {
            return monoBehaviour.StartCoroutine(OnSmoothRewindCompletedEnumerator(sequence, autoKill, onComplete));
        }

        public static IEnumerator OnSmoothRewindCompletedEnumerator(Sequence sequence, bool autoKill, Action onComplete = null)
        {
            while (sequence != null && sequence.IsPlaying())
                yield return null;

            if (autoKill)
                sequence?.Kill(true);
            onComplete?.Invoke();
        }
    }
}