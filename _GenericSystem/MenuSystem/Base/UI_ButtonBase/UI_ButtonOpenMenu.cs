using UnityEngine;
using UnityEngine.UI;

namespace MLTD.GenericSystem
{

    [RequireComponent(typeof(Button))]
    public class UI_ButtonOpenMenu : UI_ButtonBaseMenu
    {   
        void OnEnable()
        {
            button.onClick.AddListener(Open);    
        }

        void OnDisable()
        {
            button.onClick.RemoveListener(Open);    
        }

        public UI_MenuBase targetMenu;

        public void Open()
        {
            if (targetMenu != null)
            menuManager.OpenMenu(targetMenu);
            
        }
    }
}