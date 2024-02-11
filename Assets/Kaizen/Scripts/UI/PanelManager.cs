using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kaizen
{
    public class PanelManager : MonoBehaviour
    {
        private Stack<Panel> stackPanels = new Stack<Panel>();
        private Panel currentShownPanel;

        public Panel rootPanel;
        public Button buttonBack;

        protected virtual void Awake()
        {
            var allPanels = GetComponentsInChildren<Panel>(true);

            foreach (var panel in allPanels)
            {
                panel.onSwapPanel = OnSwapPanel;
            }
        }

        protected virtual void Start()
        {
            rootPanel.gameObject.SetActive(true);
            currentShownPanel = rootPanel;
            buttonBack.gameObject.SetActive(false);
        }

        protected virtual void OnSwapPanel(Panel current, Panel next)
        {
            InputBlocker.Instance.DisableInput();
            stackPanels.Push(current);
            current.Hide(Panel.AnimationDirection.Right);

            currentShownPanel = next;
            buttonBack.gameObject.SetActive(currentShownPanel != rootPanel);
            next.Show(Panel.AnimationDirection.Right, () => InputBlocker.Instance.EnableInput());
        }

        public virtual void ButtonBack_OnClicked()
        {
            InputBlocker.Instance.DisableInput();
            currentShownPanel.Hide(Panel.AnimationDirection.Left);

            currentShownPanel = stackPanels.Pop();
            currentShownPanel.Show(Panel.AnimationDirection.Left, () => InputBlocker.Instance.EnableInput());
            buttonBack.gameObject.SetActive(currentShownPanel != rootPanel);
        }
    }
}