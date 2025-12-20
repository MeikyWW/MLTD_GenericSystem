using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace MLTD.GenericSystem
{
    public class SplashScreen : MonoBehaviour
    {
        [SerializeField] UnityEvent OnSplashScreenCompleted;

        // Start is called once before the first execution of 
        // Update after the MonoBehaviour is created

        public void SplashScreenCompleted()
        {
            SceneManager.LoadSceneAsync("TitleScreen");
        }

    }
}