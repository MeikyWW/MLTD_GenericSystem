
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace MLTD.GenericSystem
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance {get; private set;}

        [Header("Source and Mixers")]
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource sfxSource;

        [field: SerializeField, Header("Volumes")]
        public float CurrentMasterVolume { get; set; } = 1f;
        
        [field: SerializeField]
        public float CurrentBGMVolume { get; set; } = 1f;

        [field: SerializeField]
        public float CurrentSFXVolume { get; set; } = 1f;

        [Header("AudioCallback")]
        [SerializeField]

        private List<AudioEntry> audioEntries = new List<AudioEntry>();
        private Dictionary<string, AudioClip> audioHandler;
        
        SystemSaveManager ssm;

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

            audioHandler = new Dictionary<string, AudioClip>();

            foreach (var entry in audioEntries)
            {
                if (!audioHandler.ContainsKey(entry.id))
                    audioHandler.Add(entry.id, entry.clip);
            }

            StartCoroutine(WaitForRequiredManagers());
        }

        private IEnumerator WaitForRequiredManagers()
        {
            yield return new WaitUntil(() => SystemSaveManager.Instance != null);

            ssm = SystemSaveManager.Instance;

            LoadAudioData();
        }

        public void SetMasterVolume(float sliderValue)
        {
            CurrentMasterVolume = sliderValue;
            audioMixer.SetFloat("MasterVolume", LinearToDecibel(sliderValue));
        }

        public void SetBGMVolume(float sliderValue)
        {
            CurrentBGMVolume = sliderValue;
            audioMixer.SetFloat("BgmVolume", LinearToDecibel(sliderValue));
        }
        
        public void SetSFXVolume(float sliderValue)
        {
            CurrentSFXVolume = sliderValue;
            audioMixer.SetFloat("SfxVolume", LinearToDecibel(sliderValue));
        }


        void ApplyVolumesToMixer()
        {
            // Immediately apply to mixer
            audioMixer.SetFloat("MasterVolume", LinearToDecibel(CurrentMasterVolume));
            audioMixer.SetFloat("BgmVolume", LinearToDecibel(CurrentBGMVolume));
            audioMixer.SetFloat("SfxVolume", LinearToDecibel(CurrentSFXVolume));
        }

        //Conversion
        public float LinearToDecibel(float linear)
        {
            if (linear <= 0.0001f)
                return -80f; // Unity's "mute"

            return Mathf.Log10(linear) * 20f;
        }

        public float DecibelToLinear(float dB)
        {
            return Mathf.Pow(10f, dB / 20f);
        }

        public void PlayByAudioId(string id)
        {
            if (audioHandler.TryGetValue(id, out AudioClip clip))
            {
                sfxSource.PlayOneShot(clip);
            }
            else
            {
                Debug.LogWarning($"Audio ID not found: {id}");
            }
        }

        public void SaveAudioData()
        {
            if (ssm.Data == null)
            return;

            ssm.Data.masterVolume = CurrentMasterVolume;
            ssm.Data.bgmVolume = CurrentBGMVolume;
            ssm.Data.sfxVolume = CurrentSFXVolume;

            ssm.Save();
        }
        
        public void LoadAudioData()
        {
            if (ssm.Data == null)
            return;

            CurrentMasterVolume = ssm.Data.masterVolume;
            CurrentBGMVolume = ssm.Data.bgmVolume;
            CurrentSFXVolume = ssm.Data.sfxVolume;

            ApplyVolumesToMixer();
        }
    }

    [System.Serializable]
    public class AudioEntry
    {
        public string id;
        public AudioClip clip;
    }

}