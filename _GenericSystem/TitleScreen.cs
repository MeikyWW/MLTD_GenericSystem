
using UnityEngine;

namespace MLTD.GenericSystem
{
        
    public class TitleScreen : MonoBehaviour
    {   
        [Header("UI")]
        [SerializeField] RectTransform ui_Settings;
        [SerializeField] RectTransform ui_TitleMenu;

        public void LoadSceneBus(string scene)
        {
            try
            {
                GlobalGameManager.Instance.LoadScene(scene);
            }
            catch
            {
                Debug.LogError("Game Manager is Not Found");
            }
        }

        public void TitleMenu()
        {
            ui_Settings.gameObject.SetActive(false);
            ui_TitleMenu.gameObject.SetActive(true);
        }

        public void NewGame()
        {
        
        }

        public void Settings()
        {
            ui_Settings.gameObject.SetActive(true);
            ui_TitleMenu.gameObject.SetActive(false);
        }

        public void QuitGame()
        {
            GlobalGameManager.Instance.QuitGame();
        }

        public void DeleteSaveData()
        {
            
        }
        
    }
}