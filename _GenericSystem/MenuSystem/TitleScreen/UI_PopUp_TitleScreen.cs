using UnityEngine;
using UnityEngine.UI;

namespace MLTD.GenericSystem
{
    public class UI_PopUp_TitleScreen : UI_PopUpBase
    {
        [SerializeField] private string text_QuitGameConfirmation;
        [SerializeField] private string text_NewGameConfirmation;

        [SerializeField] public Button btnMenu_QuitGame;
        [SerializeField] public Button btnMenu_NewGame;

        public void PopUp_QuitGameConfirmation()
        {
            OpenPopUp(text_QuitGameConfirmation, OnQuitGameResult, btnMenu_QuitGame);
        }

        public void PopUp_NewGameConfirmation()
        {
            OpenPopUp(text_NewGameConfirmation, OnNewGameResult, btnMenu_NewGame);
        }

        private void OnQuitGameResult(bool confirmed)
        {
            if (confirmed)
                GlobalGameManager.Instance.QuitGame();
        }

        private void OnNewGameResult(bool confirmed)
        {
            if (confirmed)
            {
                GlobalGameManager.Instance.NewGame();
            }
        }

        
    }
}