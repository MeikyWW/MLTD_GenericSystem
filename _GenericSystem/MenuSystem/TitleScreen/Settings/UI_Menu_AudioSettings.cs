using UnityEngine;
using UnityEngine.UI;

namespace MLTD.GenericSystem
{

    public class UI_Menu_AudioSettings : UI_MenuBase
    {
        [SerializeField] public Slider slider_Master;
        [SerializeField] public Slider slider_BGM;
        [SerializeField] public Slider slider_SFX;

        AudioManager aum;

        void Awake()
        {
            aum = AudioManager.Instance;

            slider_Master.value = aum.CurrentMasterVolume;
            slider_BGM.value = aum.CurrentBGMVolume;
            slider_SFX.value = aum.CurrentSFXVolume;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            slider_Master.onValueChanged.AddListener(SetMasterVolume);
            slider_BGM.onValueChanged.AddListener(SetBGMVolume);
            slider_SFX.onValueChanged.AddListener(SetSFXVolume);

        }

        void OnDisable()
        {
            AudioManager.Instance.SaveAudioData();
            
            slider_Master.onValueChanged.RemoveListener(SetMasterVolume);
            slider_BGM.onValueChanged.RemoveListener(SetBGMVolume);
            slider_SFX.onValueChanged.RemoveListener(SetSFXVolume);
        }

        public void SetMasterVolume(float value)
        {
            aum.SetMasterVolume(value);
        }

        public void SetBGMVolume(float value)
        {
            aum.SetBGMVolume(value);
        }

        public void SetSFXVolume(float value)
        {
            aum.SetSFXVolume(value);
        }

        
    }

}