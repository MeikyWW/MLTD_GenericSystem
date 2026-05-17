
using UnityEngine;
using UnityEngine.UI;

namespace MLTD.GenericSystem
{
    public class TitleScreen : SceneEssentials
    {   
        [Header("Title Screen")]
        [SerializeField] UI_MenuManager titleScreen_MenuManager;
        // [SerializeField] Button newGameButton;

        protected override void OnSceneEssentialsInit()
        {
            base.OnSceneEssentialsInit();
            // newGameButton.Select();
            titleScreen_MenuManager.Init();
        }

        
    }
}