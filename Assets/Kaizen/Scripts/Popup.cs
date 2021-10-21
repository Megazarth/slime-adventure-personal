using UnityEngine;

namespace Kaizen
{
    public class Popup : MonoBehaviour
    {
        public delegate void OnShowDelegate();
        public delegate void OnHideDelegate();

        protected bool destroyWhenClosed;

        public OnShowDelegate OnShow;
        public OnHideDelegate OnHide;

        public virtual void Show(bool pauseGame = true)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="destroy">Should this popup be destroyed when closed?</param>
        public virtual void Hide()
        {

        }
    }
}