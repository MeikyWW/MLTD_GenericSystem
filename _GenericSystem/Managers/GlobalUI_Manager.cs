using UnityEngine;

namespace MLTD.GenericSystem
{
    public class GlobalUIManager : MonoBehaviour
    {
        public static GlobalUIManager Instance { get; private set; } // Singleton Instance

        [SerializeField] public Canvas canvasLoading;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject); // Optional: destroy duplicates
            }
        }

        public void CallLoadingScreen()
        {
            canvasLoading.gameObject.SetActive(true);
        }
    
    }
}