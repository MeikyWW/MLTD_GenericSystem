using System;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEngine.SceneManagement;
#endif

namespace MLTD.GenericSystem
{
    public class SceneEssentials : MonoBehaviour
    {
        public SceneType sceneType = SceneType.Unknown;
        [SerializeField] UnityEvent sceneInitEvent;

        public void InitScene()
        {
#if UNITY_EDITOR
            Scene currentScene = SceneManager.GetActiveScene();
            Debug.Log("Scene "+ currentScene.name+" Inited");
#endif
            sceneInitEvent.Invoke();
        }
    }
}