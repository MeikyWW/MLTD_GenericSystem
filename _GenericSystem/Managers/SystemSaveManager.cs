using System.IO;
using UnityEngine;

namespace MLTD.GenericSystem
{
    public class SystemSaveManager : MonoBehaviour
    {
        // File name for global system data
        public static SystemSaveManager Instance {get; private set;}

        const string FILE_NAME = "system.dat";

        // Full path: persistentDataPath/system.dat
        string FilePath => Path.Combine(Application.persistentDataPath, FILE_NAME);

        // In-memory system data
        [SerializeField]
        SystemSaveData data;

        public SystemSaveData Data => data;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this; // Assign the instance
            }
        
            Load();
        }

        // Save system data to disk
        public void Save()
        {
            if (data  == null)
                data  = CreateDefault();

            WriteSystemFile(data);
        }

        // Example setting change
        public void SetLanguage(string code)
        {
            data.languageCode = code;
            Save();
        }

        // Safe write (prevents corruption on mobile)
        void WriteSystemFile(SystemSaveData data)
        {
            string tempPath = FilePath + ".tmp";
            string json = JsonUtility.ToJson(data, true);

            File.WriteAllText(tempPath, json);
            File.Copy(tempPath, FilePath, true);
            File.Delete(tempPath);
        }

        // Load system data from disk
        public void Load()
        {
            //if data is not exist
            if (!File.Exists(FilePath))
            {
                data = CreateDefault();
                Save();
                return;
            }

            string json = File.ReadAllText(FilePath);
            data = JsonUtility.FromJson<SystemSaveData>(json);
        }

        // Default values (first launch)
        SystemSaveData CreateDefault()
        {
            return new SystemSaveData
            {
                version = "0.1",

                //audio
                masterVolume = 1f,
                bgmVolume = 1f,
                sfxVolume = 1f,

                //language
                languageCode = "en"
            };
        }
    }


    [System.Serializable]
    public class SystemSaveData
    {
        public string version;

        [Header("Audio")]
        public float masterVolume;
        public float bgmVolume;
        public float sfxVolume;

        //public bool mute;

        [Header("Language")]
        public string languageCode;

        [Header("PC Only")]
        public int lastActiveSlot;
        public int qualityLevel;
        public bool fullscreen;
    }
}


