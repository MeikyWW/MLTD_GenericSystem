using UnityEngine;
using UnityEngine.UI;

namespace MLTD.GenericSystem
{

    [RequireComponent(typeof(Button))]
    public class UI_ButtonBack : UI_ButtonBaseMenu
    {   
        void OnEnable()
        {
            button.onClick.AddListener(GoBack);
        }

        void OnDisable()
        {
            button.onClick.RemoveListener(GoBack);
        }

        void GoBack()
        {
            menuManager.Back();
        }
    }
}