//using Firebase;
//using Firebase.Analytics;

namespace Kaizen
{
    public class GameAnalytics : SingletonComponent<GameAnalytics>
    {
        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }

        public void Initialize()
        {
            /*
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    // Create and hold a reference to your FirebaseApp,
                    // where app is a Firebase.FirebaseApp property of your application class.
                    var firebaseApp = FirebaseApp.DefaultInstance;
                    FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                    FirebaseAnalytics.SetSessionTimeoutDuration(System.TimeSpan.FromSeconds(300));
                    // Set a flag here to indicate whether Firebase is ready to use by your app.
                }
            });
            */
        }

        public void TrackTutorialFinished()
        {
            //FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventTutorialComplete, "tutorial", "tutorial_finished");
        }

        public void TrackLevelStart(int level)
        {
            //FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelStart, FirebaseAnalytics.ParameterLevel, level);
        }

        public void TrackLevelEnd(int level)
        {
            //FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelEnd, FirebaseAnalytics.ParameterLevel, level);
        }

        public void TrackLevelComplete(int level)
        {
            //FirebaseAnalytics.LogEvent("level_complete", FirebaseAnalytics.ParameterLevel, level);
        }

        public void TrackLevelRetry(int level)
        {
            //FirebaseAnalytics.LogEvent("level_retry", FirebaseAnalytics.ParameterLevel, level);
        }

        public void TrackLevelDuration(int level, float duration)
        {
            /*
            Parameter[] parameters = {
                new Parameter(FirebaseAnalytics.ParameterLevel, level),
                new Parameter("level_duration", duration),
            };

            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelEnd, parameters);
            */
        }

        public void TrackPlayerDied(int level)
        {
            //FirebaseAnalytics.LogEvent("player_died", FirebaseAnalytics.ParameterLevel, level);
        }

        public void TrackRemoveAdsBought(float price)
        {
            /*
            Parameter[] parameters = {
                new Parameter(FirebaseAnalytics.ParameterItemName, "remove_ads"),
                new Parameter(FirebaseAnalytics.ParameterPrice, price),
            };

            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventPurchase, parameters);
            */
        }
    }
}