using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MLTD.GenericSystem
{

    public class UI_Menu_LanguageSettings : UI_MenuBase
    {
        [Header("Toggles")]
        [SerializeField] Toggle toggleEnglish;
        [SerializeField] Toggle toggleIndonesian;
        [SerializeField] Toggle toggleSpanish;
        [SerializeField] Toggle togglePtbr;
        [SerializeField] Toggle toggleJapanese;
        [SerializeField] Toggle toggleSimpCN;

        List<Toggle> toggleLanguageList;
        Dictionary<LanguageType, Toggle> toggleLanguageHandler;
        
        void Awake()
        {
            toggleLanguageList = new List<Toggle>
            {
                toggleEnglish,
                toggleIndonesian,
                toggleSpanish,
                togglePtbr,
                toggleJapanese,
                toggleSimpCN
            };

            toggleLanguageHandler = new Dictionary<LanguageType, Toggle>();

            toggleLanguageHandler[LanguageType.English] = toggleEnglish;
            toggleLanguageHandler[LanguageType.Indonesian] = toggleIndonesian;
            toggleLanguageHandler[LanguageType.Spanish] = toggleSpanish;
            toggleLanguageHandler[LanguageType.Ptbr] = togglePtbr;
            toggleLanguageHandler[LanguageType.Japanese] = toggleJapanese;
            toggleLanguageHandler[LanguageType.SimplifiedCN] = toggleSimpCN;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            toggleEnglish.onValueChanged.AddListener(ApplyEnglish);
            toggleIndonesian.onValueChanged.AddListener(ApplyIndonesian);
            toggleSpanish.onValueChanged.AddListener(ApplySpanish);
            togglePtbr.onValueChanged.AddListener(ApplyPtbr);
            toggleJapanese.onValueChanged.AddListener(ApplyJapanese);
            toggleSimpCN.onValueChanged.AddListener(ApplySimpCN);

            foreach (Toggle toggle in toggleLanguageList)
            {
                toggle.isOn = false; 
                toggle.interactable = true;
            } 

            //Get The current Language
            LanguageType currentLanguage = LocalizationManager.Instance.currentLanguage;
            toggleLanguageHandler[currentLanguage].isOn = true;
            toggleLanguageHandler[currentLanguage].interactable = false;

        }
        void OnDisable()
        {
            toggleEnglish.onValueChanged.RemoveListener(ApplyEnglish);
            toggleIndonesian.onValueChanged.RemoveListener(ApplyIndonesian);
            toggleSpanish.onValueChanged.RemoveListener(ApplySpanish);
            togglePtbr.onValueChanged.RemoveListener(ApplyPtbr);
            toggleJapanese.onValueChanged.RemoveListener(ApplyJapanese);
            toggleSimpCN.onValueChanged.RemoveListener(ApplySimpCN);
        }

        void ApplyLanguage(Toggle selectedToggle, LanguageType type, bool state)
        {
            if (!state) return; // ignore "off" changes

            foreach(Toggle toggle in toggleLanguageList)
            {
                if(toggle != selectedToggle)
                {
                    toggle.isOn = false; 
                    toggle.interactable = true;
                }
            }

            if (state == true)
            {
                LocalizationManager.Instance.ChangeLanguageByType(type);
                selectedToggle.interactable = false;
                return;
            }

            if (InputDeviceManager.Instance.CurrentDeviceInput == DeviceInputType.Gamepad)
            EventSystem.current.SetSelectedGameObject(selectedToggle.gameObject);
        }

        void ApplyEnglish(bool state)
        {
            ApplyLanguage(toggleEnglish, LanguageType.English, state);

            if (InputDeviceManager.Instance.CurrentDeviceInput == DeviceInputType.Gamepad)
            EventSystem.current.SetSelectedGameObject(toggleIndonesian.gameObject);
        }

        void ApplyIndonesian(bool state)
        {
            ApplyLanguage(toggleIndonesian, LanguageType.Indonesian, state);
            
            if (InputDeviceManager.Instance.CurrentDeviceInput == DeviceInputType.Gamepad)
            EventSystem.current.SetSelectedGameObject(toggleEnglish.gameObject);
        }

        void ApplySpanish(bool state)
        {
            ApplyLanguage(toggleSpanish, LanguageType.Spanish, state);

            if (InputDeviceManager.Instance.CurrentDeviceInput == DeviceInputType.Gamepad)
            EventSystem.current.SetSelectedGameObject(togglePtbr.gameObject);
        }

        void ApplyPtbr(bool state)
        {
            ApplyLanguage(togglePtbr, LanguageType.Ptbr, state);

            if (InputDeviceManager.Instance.CurrentDeviceInput == DeviceInputType.Gamepad)
            EventSystem.current.SetSelectedGameObject(toggleSpanish.gameObject);
        }

        void ApplyJapanese(bool state)
        {
            ApplyLanguage(toggleJapanese, LanguageType.Japanese, state);

            if (InputDeviceManager.Instance.CurrentDeviceInput == DeviceInputType.Gamepad)
            EventSystem.current.SetSelectedGameObject(toggleSimpCN.gameObject);
        }

        void ApplySimpCN(bool state)
        {
            ApplyLanguage(toggleSimpCN, LanguageType.SimplifiedCN, state);

            if (InputDeviceManager.Instance.CurrentDeviceInput == DeviceInputType.Gamepad)
            EventSystem.current.SetSelectedGameObject(toggleJapanese.gameObject);
        }
        

    }

}