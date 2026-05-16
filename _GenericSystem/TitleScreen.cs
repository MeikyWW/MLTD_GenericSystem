
using UnityEngine;

namespace MLTD.GenericSystem
{
    public class TitleScreen : SceneEssentials
    {   
        [Header("Title Screen")]
        [SerializeField] UI_MenuManager titleScreen_MenuManager;

        protected override void OnSceneEssentialsInit()
        {
            base.OnSceneEssentialsInit();
        }
    }
}