using UnityEngine;

public class SafeUnscaledTimeAgent : MonoBehaviour
{
        public static SafeUnscaledTimeAgent Instance;

        float pausedTimeTotal = 0, lastPausedUnscaledTime = 0, lastUnpausedUnscaledTime = 0;
        float[] lastFrameSafeUnscaledTime = new float[2];
        int pausedFrame;

        public float SafeUnscaledTime
        {
            get
            {
                #if UNITY_EDITOR
                if (lastPausedUnscaledTime > lastUnpausedUnscaledTime) //Paused
                {
                    return Time.unscaledTime - pausedTimeTotal - (Time.unscaledTime - lastPausedUnscaledTime);
                }
                else if (lastUnpausedUnscaledTime > lastPausedUnscaledTime) //Playing
                {
                    return Time.unscaledTime - pausedTimeTotal;
                }
                else return Time.unscaledTime; // Not yet paused once
                #else
                return Time.unscaledTime;
                #endif
            }
        }

        public float SafeUnscaledDeltaTime
        {
            get
            {                
                if (lastPausedUnscaledTime > lastFrameSafeUnscaledTime[0] && lastUnpausedUnscaledTime > lastPausedUnscaledTime)
                {
                    return lastFrameSafeUnscaledTime[0] - lastFrameSafeUnscaledTime[1] - (lastUnpausedUnscaledTime - lastPausedUnscaledTime);
                }
                return lastFrameSafeUnscaledTime[0] - lastFrameSafeUnscaledTime[1];
            }
        }

        
        #if UNITY_EDITOR
        
        [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.AfterSceneLoad )]
        static void Init()
        {
            // Initialize singleton instance
            Instance = new GameObject( "SafeUnscaledTime" ).AddComponent<SafeUnscaledTimeAgent>();
            DontDestroyOnLoad( Instance.gameObject );
        }


        void Awake ()
        {      
            UnityEditor.EditorApplication.pauseStateChanged -= OnPauseChange;
            UnityEditor.EditorApplication.pauseStateChanged += OnPauseChange;
        }

        void Update ()
        {            
            if ( (lastUnpausedUnscaledTime == 0 && lastPausedUnscaledTime == 0) || (lastUnpausedUnscaledTime >= lastPausedUnscaledTime))
            {
                lastFrameSafeUnscaledTime[1] = lastFrameSafeUnscaledTime[0];
                lastFrameSafeUnscaledTime[0] = SafeUnscaledTime;
            }
            lastFrameSafeUnscaledTime[0] = SafeUnscaledTime;
        }

        void LateUpdate()
        {    
        }

        // void OnGUI ()
        // {
        //     GUI.Label(new Rect(16, 400, 240, 32), "LUUT: " + lastUnpausedUnscaledTime + "/LPUT: " + lastPausedUnscaledTime);
        //     GUI.Label(new Rect(16, 430, 240, 32), "PAUSED: " + (lastPausedUnscaledTime > lastUnpausedUnscaledTime));
        // }

        void OnPauseChange (UnityEditor.PauseState pause)
        {
            switch (pause)
            {
                case UnityEditor.PauseState.Paused:
                    pausedFrame = Time.frameCount;
                    lastPausedUnscaledTime = Time.unscaledTime;
                    Debug.Log("Paused at frame: " + pausedFrame + "/ unscaledTime: " + Time.unscaledTime + "(" + SafeUnscaledTime + ")");
                    break;
                case UnityEditor.PauseState.Unpaused:
                    if (Time.frameCount > pausedFrame)
                    {
                        pausedTimeTotal += 1f/60f * (Time.frameCount - pausedFrame);
                        Debug.Log("Frame count mismatch. Auto progress frames: " + (Time.frameCount - pausedFrame));
                    };
                    pausedTimeTotal += Time.unscaledTime - lastPausedUnscaledTime;
                    lastUnpausedUnscaledTime = Time.unscaledTime;
                    break;
            }
        }
        #endif
}
