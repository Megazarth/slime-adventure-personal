using System.Runtime.InteropServices;
using UnityEngine;

namespace Kaizen
{
    [RequireComponent(typeof(RectTransform))]
    public class SafeArea : MonoBehaviour
    {
        private RectTransform rectTransform;
        private Rect lastSafeArea = new Rect(0, 0, 0, 0);

        public bool simulateInEditor;
        public Rect simulatedSafeArea;

        private Rect GetSafeArea()
        {
            Rect safeArea;
#if UNITY_EDITOR
            safeArea = simulatedSafeArea;
#else
            safeArea = Screen.safeArea;
#endif
            return safeArea;
        }

        // Use this for initialization
        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();

#if UNITY_EDITOR
            simulatedSafeArea = new Rect(0, 0, Screen.width, Screen.height);
#endif

            Update();
        }

        private void ApplySafeArea(Rect area)
        {
            var anchorMin = area.position;
            var anchorMax = area.position + area.size;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;

            lastSafeArea = area;

#if UNITY_EDITOR
            Debug.Log("Safe Area applied!");
#endif
        }

        private void Update()
        {
            var nativeSafeArea = GetSafeArea();
            Rect safeArea = new Rect(nativeSafeArea.x, nativeSafeArea.y, nativeSafeArea.width, nativeSafeArea.height);
            if (safeArea != lastSafeArea)
                ApplySafeArea(safeArea);
        }
    }
}