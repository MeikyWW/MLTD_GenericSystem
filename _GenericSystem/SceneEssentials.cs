using System;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEngine.SceneManagement;
#endif

namespace MLTD.GenericSystem
{
    [DefaultExecutionOrder(-50)]
    public abstract class SceneEssentials : MonoBehaviour
    {
        [Header("Scene Essentials")]
        public SceneType sceneType = SceneType.Unknown;
        [SerializeField] UnityEvent sceneInitEvent;

        [Header("Bootstrap")]
        [SerializeField] GlobalGameManager globalGameManagerPrefab;

        void Awake()
        {
            EnsureGlobalGameManagerExists();
        }

        void EnsureGlobalGameManagerExists()
        {
            if (GlobalGameManager.Instance != null)
                return;

            if (FindAnyObjectByType<GlobalGameManager>() != null)
                return;

            if (globalGameManagerPrefab == null)
            {
                Debug.LogError(
                    "No Global Game Manager in scene and globalGameManagerPrefab is not assigned on SceneEssentials.",
                    this);
                return;
            }

            GlobalGameManager.EnsureExists(globalGameManagerPrefab);
        }

        public void InitSceneEssentials()
        {
            OnSceneEssentialsInit();
        }

        protected virtual void OnSceneEssentialsInit()
        {
#if UNITY_EDITOR
            Scene currentScene = SceneManager.GetActiveScene();
            Debug.Log("Scene "+ currentScene.name+" Inited");
#endif
            sceneInitEvent.Invoke();
        }
    }
}
