using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Events;

namespace MLTD.GenericSystem
{
    public class GlobalGameManager : MonoBehaviour
    {
        public static GlobalGameManager Instance;

        [TextArea(2, 4)]
        [SerializeField] private string description =
            "This is the Global Game Manager. Its Gameobject contains all"+ 
            "singleton managers and persists across scenes via DontDestroyOnLoad.";

        [Header("Game States (ReadOnly)")]
        [SerializeField] private GameStates currentGameState;
        public GameStates CurrentGameState => currentGameState;

        [SerializeField] private GameStates previousGameState;
        public GameStates PreviousGameState => previousGameState;
        
        public event Action<GameStates> OnGameStateChanged;

        [Header("Track Scenes (ReadOnly)")]
        public string previousScene;
        public string currentScene;
        
        [SerializeField] private bool isLoadingScenes;
        public bool IsLoadingScenes => isLoadingScenes;

        private Dictionary<GameStates, Action> gameStateOnEnterHandlers;

        [Header("Managers")]
        [SerializeField] public SystemSaveManager ssm;
        [SerializeField] public InputDeviceManager idm;
        [SerializeField] public InputGameplayManager igm;
        [SerializeField] public AudioManager aum;
        [SerializeField] public LocalizationManager lcm;
        //[SerializeField] public DisplayManager dspm;
        [SerializeField] public GlobalUIManager gl_uim;

        public bool SystemsReady { get; private set; }

        private async void Awake()
        {
            if (Instance == null)
            {
                Instance = this; // Assign the instance
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject); // Prevent duplicate GameManagers\
                return;
            }

            ssm = SystemSaveManager.Instance;
            idm = InputDeviceManager.Instance;
            igm = InputGameplayManager.Instance;
            aum = AudioManager.Instance;
            lcm = LocalizationManager.Instance;
            //dspm = DisplayManager.Instance;
            //gl_uim = GlobalUIManager.Instance;
            
            ValidateManagers();

            InitGameStates();

            // Should be called only once Per Lifetime of the game. 
            // Ideally When SplashScreen because is the only scene 
            // that Wasn't going to be visited anymore. 
        }

        void ValidateManagers()
        {
            if (!ssm) Debug.LogError("SystemSaveManager missing");
            if (!idm) Debug.LogError("InputDeviceManager missing");
            if (!igm) Debug.LogError("InputDeviceManager missing");
            if (!aum) Debug.LogError("AudioManager missing");
            if (!lcm) Debug.LogError("LocalizationManager missing");
            //if (!dspm) Debug.LogError("DisplayManager missing");
            //if (!gl_uim) Debug.LogError("GlobalUIManager missing");

            SystemsReady =
                ssm  
                && idm
                && igm  
                && aum  
                && lcm  
                //&& dspm  
                //&& gl_uim
                ;
        }

        private void OnEnable()
        {
            //"Hey Unity — when you finish loading a scene, 
            //call my HandleSceneLoaded(Scene, LoadSceneMode) method."
            SceneManager.sceneLoaded += HandleSceneLoaded; 
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }

    #region Main Game Flow Logic

        void InitGameStates()
        {
            gameStateOnEnterHandlers = new Dictionary<GameStates, Action>
            {
                { GameStates.Init,      OnEnter_InitState },
                { GameStates.Splash,    OnEnter_SplashState },
                { GameStates.Title,     OnEnter_TitleState },
                { GameStates.MainGameplay,  OnEnter_GameplayState },
                { GameStates.Cutscene,  OnEnter_CutsceneState },
                //{ GameStates.MainMenu,  MainMenuState() },
                //{ GameStates.MiniGame,  GameoverState() },
            };

            Debug.Log("Init Game States Completed");
        }

        //After Scene is Loaded, check the Scene State
        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            //Check which Scene you are residing
            var marker = FindAnyObjectByType<SceneEssentials>();
            SceneType currentSceneType = marker != null ? marker.sceneType : SceneType.Unknown;

            //Switch behaviour depends on the Scene Type
            switch (currentSceneType)
            {
                case SceneType.Splash:
                    SetGameState(GameStates.Splash);
                    break;

                case SceneType.Title:
                    SetGameState(GameStates.Title);
                    break;

                case SceneType.Gameplay:
                    SetGameState(GameStates.MainGameplay);
                    break;
                
                case SceneType.Cutscene:
                    SetGameState(GameStates.Cutscene);
                    break;

                default:
                    Debug.LogWarning("SceneType not recognized.");
                    break;
            }
        }

        //Main State Changer
        public void SetGameState(GameStates newState)
        {
            previousGameState = currentGameState;
            currentGameState = newState;

            OnGameStateChanged?.Invoke(currentGameState);

            if (gameStateOnEnterHandlers.TryGetValue(CurrentGameState, out Action action)) // Call On Entering the states
            {
                action.Invoke();
            }
            else
            {
                Debug.LogWarning($"State {CurrentGameState} not implemented.");
            }
        }

    #endregion

    #region State
        public void OnEnter_InitState()
        {
            Debug.Log("Entering InitState");
        }
        public void OnEnter_SplashState()
        {
            Debug.Log("Entering Splash State");
        }

        public void OnEnter_TitleState() // Called when SplashScreen Logo are done Played
        {
            Debug.Log("Entering Title State");
        }
        public void OnEnter_GameplayState()
        {
            Debug.Log("Entering Gameplay State");
        }public void OnEnter_CutsceneState()
        {
            Debug.Log("Entering Cutscene State");
        }
    #endregion

    #region Scene Loader

        private IEnumerator LoadSceneRoutine(string sceneName, bool useLoadingScreen)
        {
            if (string.IsNullOrEmpty(previousScene))
            {
                previousScene = SceneManager.GetActiveScene().name;
            }

            else
            {
                previousScene = currentScene;
            }

            isLoadingScenes = true;

            //call Loading Screen 
            if (useLoadingScreen)
                GlobalUIManager.Instance.CallLoadingScreen();

            //wait for loading scene transition first
            yield return new WaitForSeconds(1f);

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

            while (!asyncLoad.isDone)
            {
                yield return null; // Wait until the scene fully loads
            }

            isLoadingScenes = false;
            currentScene = sceneName;

        }

        public void LoadScene(string sceneName)
        {
            StartCoroutine(LoadSceneRoutine(sceneName, false));
        }
        public void LoadScene(string sceneName, bool useLoadingScreen)
        {
            StartCoroutine(LoadSceneRoutine(sceneName, useLoadingScreen));
        }

        public void ReloadCurrentScene(bool useLoadingScreen)
        {
            Scene currentScene = SceneManager.GetActiveScene();

            StartCoroutine(LoadSceneRoutine(currentScene.name, useLoadingScreen));
        }

        public void LoadTitleScreen()
        {
            LoadScene("TitleScreen");
        }

    #endregion

        public void QuitGame()
        {
            // If running in the Unity Editor
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;

    #else
            // If running in a build
            Application.Quit();

    #endif
        }

        [SerializeField] UnityEvent newGameEvent;

        public void NewGame()
        {
            newGameEvent.Invoke();
        }

    }


    public enum GameStates
    {
        Init,
        MainMenu,
        MainGameplay,
        OtherGameplay,
        Cutscene,
        Splash,
        Title,

    }

    public enum SceneType
    {
        Splash,
        Title,
        MainMenu,
        Gameplay,
        Cutscene,
        Unknown
    }

}

