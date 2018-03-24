using UnityEngine;

namespace Assets.Scripts
{
    public class PlatformFix : MonoBehaviour
    {
        private void Awake()
        {
            Application.targetFrameRate = Screen.currentResolution.refreshRate;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
    }
}