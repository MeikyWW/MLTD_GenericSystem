using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MLTD.GenericSystem
{
    public class UI_Menu_TitleMainMenu : UI_MenuBase
    {
        [Header("Title Main Menu Button List")]
        [SerializeField] List <ButtonMenuEventPair> buttonMenuList;


        protected override void OnEnable()
        {
            base.OnEnable();

            foreach (var pair in buttonMenuList)
            {
                if (pair.buttonMenu == null || pair.assignedEvent == null)
                    continue;

                pair.buttonMenu.onClick.AddListener(pair.assignedEvent.Invoke);
            }
        }

        protected void OnDisable()
        {
            foreach (var pair in buttonMenuList)
            {
                if (pair.buttonMenu == null || pair.assignedEvent == null)
                    continue;

                pair.buttonMenu.onClick.RemoveListener(pair.assignedEvent.Invoke);
            }
        }

        // ===== MAIN MENU ACTIONS =====

        public void _Continue()
        {
            Debug.Log("Continue");
        }

        public void _NewGame()
        {
            RequestNewGame();
            //Debug.Log("NewGame");
        }

        public void _LoadGame()
        {
            //Debug.Log("Opened Using ButtonOpenMenu");
        }

        public void _Settings()
        {
            //Debug.Log("Opened Using ButtonOpenMenu");
        }

        public void _QuitGame()
        {
            RequestQuitGame();
            //Debug.Log("Quit Game");
        }

        [Header("Dependencies")]
        [SerializeField] UI_PopUp_TitleScreen titleScreenPopup;


        public void RequestNewGame()
        {
            titleScreenPopup.PopUp_NewGameConfirmation();
        }

        public void RequestQuitGame()
        {
            titleScreenPopup.PopUp_QuitGameConfirmation();
        }
    }

    [System.Serializable]
    public class ButtonMenuEventPair
    {
        public Button buttonMenu;
        public UnityEvent assignedEvent;
    }
}