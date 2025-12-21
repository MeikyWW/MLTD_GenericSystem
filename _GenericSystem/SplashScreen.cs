using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace MLTD.GenericSystem
{
    public class SplashScreen : MonoBehaviour
    {
        [SerializeField] bool isSamples = false;
        [SerializeField] string TitleScreenScene = "TitleScreen";
        [SerializeField] string TitleScreenSampleScene = "TitleScreen (Samples)";
        [SerializeField] UnityEvent OnSplashScreenCompleted;

        public void SplashScreenCompleted()
        {
            OnSplashScreenCompleted.Invoke();

            string targetScene;

            if(!isSamples)
                targetScene = TitleScreenScene;
            else
                targetScene = TitleScreenSampleScene;

            SceneManager.LoadScene(targetScene);
        }

    }
}

//test