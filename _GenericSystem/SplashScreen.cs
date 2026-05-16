using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace MLTD.GenericSystem
{
    public class SplashScreen : SceneEssentials
    {
        [Header("Splash Screen")]
        [SerializeField] bool isSamples = false;
        [SerializeField] string TitleScreenScene = "TitleScreen";
        [SerializeField] string TitleScreenSampleScene = "TitleScreen (Samples)";
        [SerializeField] UnityEvent OnSplashScreenCompleted;

        [SerializeField] Canvas splashScreenCanvas;
        [SerializeField] Animator animator;

        void Start()
        {
            splashScreenCanvas.gameObject.SetActive(false);
        }

        protected override void OnSceneInit()
        {
            splashScreenCanvas.gameObject.SetActive(true);
            animator.SetTrigger("PlaySplashScreen");
            base.OnSceneInit();
        }

        //Called from the Animation Event
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