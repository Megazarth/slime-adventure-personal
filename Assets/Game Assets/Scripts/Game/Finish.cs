﻿using DG.Tweening;
using System;
using UnityEngine;

namespace Slime
{
    public abstract class Finish : MonoBehaviour
    {
        [SerializeField] private AudioClip playerEnterClip = null;
        [SerializeField] private ParticleSystem auraParticleSystem = null;
        private SpriteRenderer spriteRenderer;

        private Sequence sequence;

        public Action onEnter;

        public bool IsFinish { get; internal set; }
        protected ParticleSystem AuraParticleSystem { get => auraParticleSystem; }
        protected AudioClip PlayerEnterClip { get => playerEnterClip; }

        protected virtual void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();

            sequence = DOTween.Sequence();
            sequence.SetLoops(-1);
            sequence.SetAutoKill(false);

            sequence.Insert(0f, spriteRenderer.DOFade(0f, 1f).From(1f).SetEase(Ease.InOutSine));
            sequence.Insert(1f, spriteRenderer.DOFade(1f, 1f).From(0f).SetEase(Ease.InOutSine));
        }

        protected virtual void Start()
        {
            auraParticleSystem.Stop();
        }

        protected virtual void OnDestroy()
        {
            sequence?.Kill();
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision) { }

        protected virtual void OnTriggerExit2D(Collider2D collision) { }
    }
}

