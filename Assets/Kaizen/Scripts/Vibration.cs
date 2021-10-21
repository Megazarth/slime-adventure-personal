using UnityEngine;

namespace Kaizen
{
    public static class Vibration
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        private static string VibrateID = "vibrate";
        private static long[] tapticDuration = new long[] { 100, 300, 500 };

        private static AndroidJavaClass unityPlayer;
        private static AndroidJavaObject currentActivity;
        private static AndroidJavaObject vibrator;

        public static void Initialize()
        {
            unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
        }

        public static void Vibrate(long milliseconds)
        {
            vibrator.Call(VibrateID, milliseconds);
        }

        public static void Vibrate(long[] pattern, int repeat)
        {
            vibrator.Call(VibrateID, pattern, repeat);
        }

        public static void TriggerTaptic(int power)
        {
            power = Mathf.Clamp(power - 1, 0, 2);
            vibrator.Call(VibrateID, tapticDuration[power]);
        }
#elif UNITY_EDITOR



        public static void Initialize()
        {
            Debug.Log($"UNITY EDITOR: Vibration class initialized.");
        }

        public static void Vibrate(long milliseconds)
        {
            Debug.Log($"UNITY EDITOR: Vibration called for {milliseconds} ms.");
        }

        public static void Vibrate(long[] pattern, int repeat)
        {
            Debug.Log($"UNITY EDITOR: Vibration pattern called. Pattern: {pattern.ToString()}. Repeat: {repeat}");
        }

        public static void TriggerTaptic(int power)
        {
            Debug.Log($"UNITY EDITOR: Taptic triggered. Power: {power}.");
        }
#endif
    }
}