using TMPro;
using UnityEngine;

namespace MLTD.GenericSystem
{
    public class UI_Menu_GraphicsSettings : UI_MenuBase
    {

        [Header("Menu Contents")]
        [SerializeField] RectTransform ui_windowMode;
        [SerializeField] RectTransform ui_resolution;

        [Header("Property")]
        [SerializeField] TMP_Dropdown resolutionDropdown;
        [SerializeField] TMP_Dropdown screenModeDropdown;
        Resolution[] resolutions;
        
        void Start()
        {
    #if UNITY_STANDALONE_WIN 
            ui_windowMode.gameObject.SetActive(true);
            ui_resolution.gameObject.SetActive(true);

            if(DisplayManager.Instance)
            {
                DisplayManager.Instance.InitializeResolutionDropdown(resolutionDropdown);
                DisplayManager.Instance.InitializeScreenModeDropdown(screenModeDropdown);
            }
    #else
            ui_windowMode.gameObject.SetActive(false);
            ui_resolution.gameObject.SetActive(false);
    #endif  

            
        }
    }

}