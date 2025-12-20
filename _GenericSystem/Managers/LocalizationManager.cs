using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace MLTD.GenericSystem
{
    public class LocalizationManager : MonoBehaviour
    {
        public static LocalizationManager Instance {get; private set;}

        [SerializeField] public LanguageType currentLanguage;
        
        LanguageType defaultLanguage = LanguageType.English;

        Dictionary<LanguageType,int> languageHandler;

        void Awake()
        {
            
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            
        }

        void Start()
        {
            //init handler
            languageHandler = new Dictionary<LanguageType, int>();
            
            languageHandler[LanguageType.English] = 0;
            languageHandler[LanguageType.Indonesian] = 1;
            languageHandler[LanguageType.Spanish] = 2;
            languageHandler[LanguageType.Ptbr] = 3;
            languageHandler[LanguageType.Japanese] = 4;
            languageHandler[LanguageType.SimplifiedCN] = 5;

            if (!PlayerPrefs.HasKey("CurrentLanguage")) //if no Language Data
            {
                // Set default values here
                ChangeLanguageByType(defaultLanguage);
                return;
            }

            int languageIndex = PlayerPrefs.GetInt("CurrentLanguage", 0);
            ChangeLanguageByIndex(languageIndex);
        }

        public void ChangeLanguageByType(LanguageType languageType)
        {
            int languageIndex = languageHandler[languageType];
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[languageIndex];
            currentLanguage = languageType;
            PlayerPrefs.SetInt("CurrentLanguage", languageIndex);
        }

        public void ChangeLanguageByIndex(int languageIndex)
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[languageIndex];
            currentLanguage = FindLanguageByIndex(languageIndex);
            PlayerPrefs.SetInt("CurrentLanguage", languageIndex);
        }

        LanguageType FindLanguageByIndex(int languageIndex)
        {
            foreach(var lang in languageHandler)
            {
                if (lang.Value == languageIndex)
                {
                    return lang.Key;
                }
            }
            return LanguageType.English;
        }
    }

    public enum LanguageType
    {
        English,
        Indonesian,
        Spanish,
        Ptbr,
        Japanese,
        SimplifiedCN
    }
}
