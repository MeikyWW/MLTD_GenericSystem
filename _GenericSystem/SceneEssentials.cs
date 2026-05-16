using System;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEngine.SceneManagement;
#endif

namespace MLTD.GenericSystem
{
    public abstract class SceneEssentials : MonoBehaviour
    {
        [Header("Scene Essentials")]
        public SceneType sceneType = SceneType.Unknown;
        [SerializeField] UnityEvent sceneInitEvent;
        public void InitSceneEssentials()
        {
            OnSceneInit();
        }

        protected virtual void OnSceneInit()
        {
#if UNITY_EDITOR
            Scene currentScene = SceneManager.GetActiveScene();
            Debug.Log("Scene "+ currentScene.name+" Inited");
#endif
            sceneInitEvent.Invoke();
        }
    }
}