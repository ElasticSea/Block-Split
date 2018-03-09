using UnityEngine;

namespace Assets.Scripts
{
    public class PlatformFix : MonoBehaviour
    {
        private void Awake()
        {
            // BUG Fixes code stripping in WEBGL https://issuetracker.unity3d.com/issues/webgl-createprimitive-fails-to-create-required-components
            MeshCollider a;
            MeshFilter b;
            MeshRenderer c;
            BoxCollider d;
            var s = Shader.Find("Default");

            Application.targetFrameRate = Screen.currentResolution.refreshRate;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
    }
}