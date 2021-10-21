using DG.Tweening;
using Kaizen;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Slime
{
    public class TransitionAnimator : SceneSingletonComponent<TransitionAnimator>
    {
        private const float OffsetMultiplier = 1.0f;
        private const float TransitionDuration = 1.0f;
        private const float FadeOutDuration = 0.5f;
        private const float FadeInDuration = 0.5f;

        private const float LevelTextOffset = 100.0f;
        private const float LevelNumberTextOffset = 100.0f;

        private const float MinTailLengthMultiplier = 0.1f;
        private const int PieceCount = 10;

        private const Ease TransitionEase = Ease.InOutCirc;
        private const Ease FadeInEase = Ease.OutQuad;
        private const Ease FadeOutEase = Ease.InQuad;

        private Color colorDefault = new Color(0.8941177f, 0.8784314f, 0.8588235f);
        private Color colorCurrent;

        private Vector2 textLevelTitleOriginalPosition = Vector2.zero;
        private Vector2 textLevelNumberOriginalPosition = Vector2.zero;

        private Sequence sequence;

        private List<TransitionPiece> pieces = new List<TransitionPiece>();

        public Image imageBackground;
        public Canvas canvas;
        public TextMeshProUGUI textLevelTitle;
        public TextMeshProUGUI textLevelNumber;

        private RectTransform rectTransform;

        [Space]
        public TransitionPiece prefabTransitionPiece;


        protected override void Awake()
        {
            base.Awake();
            rectTransform = GetComponent<RectTransform>();
            SpawnPieces();

            textLevelTitle.gameObject.SetActive(false);
            textLevelNumber.gameObject.SetActive(false);

            colorCurrent = colorDefault;

            SetTransitionColor(Color.black);
        }

        private void Start()
        {
            textLevelTitleOriginalPosition = textLevelTitle.rectTransform.anchoredPosition;
            textLevelNumberOriginalPosition = textLevelNumber.rectTransform.anchoredPosition;
        }

        private void OnDestroy()
        {
            sequence?.Kill();
        }

        // If the resolution changes in runtime, call this to reset the pieces
        public void ResetPieces()
        {
            FlushPieces();
            SpawnPieces();
        }

        public void Show(Action onComplete = null, Color color = default)
        {
            InputBlocker.Instance.DisableInput();
            SetTransitionColor(color);
            RandomizeTailLength();

            textLevelTitle.gameObject.SetActive(false);
            textLevelNumber.gameObject.SetActive(false);

            imageBackground.rectTransform.localScale = Vector3.one;

            canvas.enabled = true;

            sequence?.Kill(true);
            sequence = DOTween.Sequence();

            sequence.Insert(0.0f, imageBackground.rectTransform.DOAnchorPos(Vector2.zero, TransitionDuration).From(new Vector2(imageBackground.rectTransform.rect.width * (1 + OffsetMultiplier), 0.0f)).SetEase(TransitionEase));
            sequence.OnKill(() =>
            {
                onComplete?.Invoke();
            });
        }

        public void Show(int levelNumber = 0, Action onComplete = null, Color color = default)
        {
            InputBlocker.Instance.DisableInput();
            SetTransitionColor(color);
            RandomizeTailLength();

            textLevelTitle.gameObject.SetActive(true);
            textLevelNumber.gameObject.SetActive(true);

            imageBackground.rectTransform.localScale = Vector3.one;
            textLevelNumber.text = levelNumber.ToString();

            canvas.enabled = true;

            sequence?.Kill(true);
            sequence = DOTween.Sequence();

            var totalAnimationDuration = 0f;

            sequence.Insert(totalAnimationDuration, imageBackground.rectTransform.DOAnchorPos(Vector2.zero, TransitionDuration).From(new Vector2(imageBackground.rectTransform.rect.width * (1 + OffsetMultiplier), 0.0f)).SetEase(TransitionEase));
            totalAnimationDuration += TransitionDuration * 0.5f;

            textLevelTitle.gameObject.SetActive(true);
            textLevelNumber.gameObject.SetActive(true);

            sequence.Insert(totalAnimationDuration, textLevelTitle.DOFade(1.0f, FadeInDuration).From(0.0f).SetEase(FadeInEase));

            var levelTextOffsetPosition = textLevelTitleOriginalPosition;
            levelTextOffsetPosition.x += LevelTextOffset;
            sequence.Insert(totalAnimationDuration, textLevelTitle.rectTransform.DOAnchorPosX(textLevelTitleOriginalPosition.x, FadeInDuration).From(levelTextOffsetPosition).SetEase(FadeInEase));
            //totalAnimationDuration += FadeInDuration;

            sequence.Insert(totalAnimationDuration, textLevelNumber.DOFade(1.0f, FadeInDuration).From(0.0f).SetEase(FadeInEase));
            var levelNumberTextOffsetPosition = textLevelNumberOriginalPosition;
            levelNumberTextOffsetPosition.x += LevelNumberTextOffset;
            sequence.Insert(totalAnimationDuration, textLevelNumber.rectTransform.DOAnchorPosX(textLevelNumberOriginalPosition.x, FadeInDuration).From(levelNumberTextOffsetPosition).SetEase(FadeInEase));
            //totalAnimationDuration += FadeInDuration;

            sequence.OnKill(() => onComplete?.Invoke());
        }

        public void Hide(Action onComplete = null, Color color = default)
        {
            SetTransitionColor(color);
            RandomizeTailLength();
            imageBackground.rectTransform.localScale = new Vector3(-1f, 1f, 1f);

            sequence = DOTween.Sequence();

            var totalAnimationDuration = 0.5f;

            if (textLevelTitle.gameObject.activeSelf)
            {
                sequence.Insert(totalAnimationDuration, textLevelTitle.DOFade(0.0f, FadeOutDuration).From(1.0f).SetEase(FadeOutEase));

                var levelTextOffsetPosition = textLevelTitleOriginalPosition;
                levelTextOffsetPosition.x -= LevelTextOffset;
                sequence.Insert(totalAnimationDuration, textLevelTitle.rectTransform.DOAnchorPosX(levelTextOffsetPosition.x, FadeOutDuration).From(textLevelTitleOriginalPosition).SetEase(FadeOutEase));
                //totalAnimationDuration += FadeOutDuration;

                sequence.Insert(totalAnimationDuration, textLevelNumber.DOFade(0.0f, FadeOutDuration).From(1.0f).SetEase(FadeOutEase));
                var levelNumberTextOffsetPosition = textLevelNumberOriginalPosition;
                levelNumberTextOffsetPosition.x -= LevelNumberTextOffset;
                sequence.Insert(totalAnimationDuration, textLevelNumber.rectTransform.DOAnchorPosX(levelNumberTextOffsetPosition.x, FadeOutDuration).From(textLevelNumberOriginalPosition).SetEase(FadeOutEase));
                //totalAnimationDuration += FadeOutDuration;
            }

            sequence.Insert(totalAnimationDuration, imageBackground.rectTransform.DOAnchorPos(new Vector2(-imageBackground.rectTransform.rect.width * (1 + OffsetMultiplier), 0.0f), TransitionDuration).From(Vector2.zero).SetEase(TransitionEase));
            totalAnimationDuration += TransitionDuration;

            sequence.OnKill(() =>
            {
                InputBlocker.Instance.EnableInput();
                canvas.enabled = false;
                onComplete?.Invoke();
            });
        }

        private void SetTransitionColor(Color color)
        {
            Color newColor = color;
            if (color == default)
                newColor = colorDefault;

            if (newColor == colorCurrent)
                return;

            foreach (var piece in pieces)
            {
                piece.SetTransitionColor(newColor);
            }
            imageBackground.color = newColor;
            colorCurrent = newColor;
        }

        private void FlushPieces()
        {
            foreach (var piece in pieces)
            {
                Destroy(piece.gameObject);
            }
            pieces.Clear();
        }

        private void RandomizeTailLength()
        {
            var pieceHeight = rectTransform.sizeDelta.y / PieceCount;
            var minLength = rectTransform.sizeDelta.x * MinTailLengthMultiplier;
            var maxLength = rectTransform.sizeDelta.x * OffsetMultiplier - pieceHeight;

            foreach (var piece in pieces)
            {
                piece.SetTailLength(UnityEngine.Random.Range(minLength, maxLength));
            }
        }

        private void SpawnPieces()
        {
            var pieceHeight = rectTransform.sizeDelta.y / PieceCount;
            for (int i = 0; i < PieceCount; i++)
            {
                var newPiece = Instantiate(prefabTransitionPiece, transform.position, Quaternion.identity, imageBackground.rectTransform);
                newPiece.SetHeight(pieceHeight);
                newPiece.rectTransform.anchoredPosition = new Vector2(0.0f, i * pieceHeight);

                pieces.Add(newPiece);
            }
        }
    }
}
