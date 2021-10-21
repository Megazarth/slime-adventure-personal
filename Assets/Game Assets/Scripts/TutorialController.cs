using DG.Tweening;
using Kaizen;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Slime
{
    public class TutorialController : MonoBehaviour
    {
        private const float AnimationDuration = 1.0f;

        private Sequence sequenceBlue;
        private Sequence sequencePink;

        private List<Finish> finishLines = new List<Finish>();

        private bool isBlueSlimeMoved;
        private bool isPinkSlimeMoved;

        public Image blueArea;
        public Image pinkArea;
        public TextMeshProUGUI textBlue;
        public TextMeshProUGUI textPink;

        private void Start()
        {
            if (EncryptedPrefs.GetBool(Global.KeyTutorialLevel1Completed))
            {
                gameObject.SetActive(false);
            }
            else
            {
                StartAnimation();
                var allGOs = gameObject.scene.GetRootGameObjects();

                foreach (var item in allGOs)
                {
                    if (item.TryGetComponent(out Finish finish))
                    {
                        finishLines.Add(finish);
                    }
                    if (item.TryGetComponent(out LevelController levelController))
                    {
                        levelController.buttonPause.gameObject.SetActive(false);
                    }
                }
            }
        }

        private void Update()
        {
            if (AllFinishReached())
                return;

            if (InputManager.Instance.joystickBlue.Horizontal != 0f && sequenceBlue != null && !isBlueSlimeMoved)
            {
                sequenceBlue?.Kill();
                textBlue.gameObject.SetActive(false);
                blueArea.DOFade(0f, AnimationDuration);
                isBlueSlimeMoved = true;
            }
            if (InputManager.Instance.joystickPink.Horizontal != 0f && sequencePink != null && !isPinkSlimeMoved)
            {
                sequencePink?.Kill();
                textPink.gameObject.SetActive(false);
                pinkArea.DOFade(0f, AnimationDuration);
                isPinkSlimeMoved = true;
            }
        }

        private void OnDestroy()
        {
            if (AllFinishReached())
            {
                if (!EncryptedPrefs.GetBool(Global.KeyTutorialLevel1Completed))
                {
                    EncryptedPrefs.SetBool(Global.KeyTutorialLevel1Completed, isBlueSlimeMoved && isPinkSlimeMoved);
                    GameAnalytics.Instance.TrackTutorialFinished();
                }
            }

            sequenceBlue?.Kill();
            sequencePink?.Kill();

            blueArea.DOKill();
            pinkArea.DOKill();
        }

        private bool AllFinishReached()
        {
            if (finishLines.Count == 0)
                return true;

            foreach (var finish in finishLines)
            {
                if (!finish.IsFinish)
                    return false;
            }
            return true;
        }

        private void StartAnimation()
        {
            sequenceBlue = DOTween.Sequence();
            sequenceBlue.SetLoops(-1, LoopType.Yoyo);

            sequenceBlue.Insert(0.0f, blueArea.DOFade(0.0f, AnimationDuration).SetEase(Ease.InOutQuad));

            sequencePink = DOTween.Sequence();
            sequencePink.SetLoops(-1, LoopType.Yoyo);

            sequencePink.Insert(0.0f, pinkArea.DOFade(0.0f, AnimationDuration).SetEase(Ease.InOutQuad));
        }
    }

}
