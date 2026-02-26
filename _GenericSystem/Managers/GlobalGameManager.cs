using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Events;
using UnityEngine.UI;

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

        [Header("Loading Screen State")]
        [SerializeField] private bool isLoadingScreenProgressing;
        public bool IsLoadingScreenProgressing => isLoadingScreenProgressing;
        public event Action<bool> OnLoadingScreenStateChanged;

        private Dictionary<GameStates, Action> gameStateOnEnterHandlers;

        [Header("Managers")]
        [SerializeField] public SystemSaveManager ssm;
        [SerializeField] public InputDeviceManager idm;
        [SerializeField] public InputGameplayManager igm;
        [SerializeField] public LocalizationManager lcm;
        [SerializeField] public AudioManager aum;
        [SerializeField] public GlobalUIManager gl_uim;
        //[SerializeField] public DisplayManager dspm;

        public bool ManagersReady { get; private set; }

        void Awake()
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

            InitGameStates();

            // Should be called only once Per Lifetime of the game. 
            // Ideally When SplashScreen because is the only scene 
            // that Wasn't going to be visited anymore. 
        }

        void Start()
        {
            StartCoroutine(InitManagersRoutine());

            if(!bootCompleted)
                StartCoroutine(BootFlow());
            
            // Initialize loading screen state tracking
            UpdateLoadingScreenState();
        }
        
        IEnumerator InitManagersRoutine()
        {
            // Wait until all singleton Instances exist
            yield return new WaitUntil(() =>
                SystemSaveManager.Instance &&
                InputDeviceManager.Instance &&
                InputGameplayManager.Instance &&
                LocalizationManager.Instance &&
                AudioManager.Instance
            );

            ssm = SystemSaveManager.Instance;
            idm = InputDeviceManager.Instance;
            igm = InputGameplayManager.Instance;
            lcm = LocalizationManager.Instance;
            aum = AudioManager.Instance;

            ValidateManagers();
        }


        void ValidateManagers()
        {
            if (!ssm) Debug.LogError("SystemSaveManager missing");
            if (!idm) Debug.LogError("InputDeviceManager missing");
            if (!igm) Debug.LogError("InputGameplayManager missing");
            if (!aum) Debug.LogError("AudioManager missing");
            if (!lcm) Debug.LogError("LocalizationManager missing");
            //if (!dspm) Debug.LogError("DisplayManager missing");
            //if (!gl_uim) Debug.LogError("GlobalUIManager missing");

            ManagersReady =
                ssm  
                && idm
                && igm  
                && aum  
                && lcm  
                //&& dspm  
                //&& gl_uim
                ;
        }
    
    #region Boot

        [SerializeField] float progress;
        
        [SerializeField] Slider progressBar;
        [SerializeField] public bool bootCompleted = false;
        [SerializeField] Canvas canvasBoot;

    
        IEnumerator BootFlow()
        {
            if (!canvasBoot.gameObject.activeSelf)
                canvasBoot.gameObject.SetActive(true);

            yield return WaitForManagers(0f, 0.25f, 0.6f);
            yield return LoadSaveData(0.25f, 0.5f, 0.6f);

            // Later:
            // yield return InitAddressables(0.5f, 1f);

            SetProgress(1f);

            // 🔹 HOLD at full
            yield return new WaitForSeconds(1f); // 0.5–1 sec feels good

            bootCompleted = true;
            canvasBoot.gameObject.SetActive(false);
            
            SceneManager.sceneLoaded -= HandleSceneLoaded; 
            SceneManager.sceneLoaded += HandleSceneLoaded; 
            
            //manual init scene
            HandleSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);

            Debug.Log("Boot is finished");
        }

        IEnumerator WaitForManagers(float start, float end, float minDuration)
        {
            float timer = 0f;

            while (!ManagersReady || timer < minDuration)
            {
                timer += Time.deltaTime;

                float t = Mathf.Clamp01(timer / minDuration);
                SetProgress(Mathf.Lerp(start, end, t));

                yield return null;
            }

            SetProgress(end);
            Debug.Log("All Managers are loaded");
        }

        IEnumerator LoadSaveData(float start, float end, float minDuration)
        {
            // SystemSaveManager.Instance.Load();

            float timer = 0f;

            while (timer < minDuration)
            {
                timer += Time.deltaTime;

                float t = Mathf.Clamp01(timer / minDuration);
                SetProgress(Mathf.Lerp(start, end, t));

                yield return null;
            }

            SetProgress(end);
            Debug.Log("Save Data is loaded");
        }

        // IEnumerator InitAddressables(float start, float end)
        // {
        //     var handle = Addressables.InitializeAsync();

        //     while (!handle.IsDone)
        //     {
        //         float p = Mathf.Lerp(start, end, handle.PercentComplete);
        //         SetProgress(p);
        //         yield return null;
        //     }

        //     SetProgress(end);
        // }

        public void SetProgress(float value)
        {
            progressBar.value = value;
        }

#endregion

    #region Game State
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
         //After Scene is Loaded, check the Scene State
        public void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            var sceneEssentials = FindAnyObjectByType<SceneEssentials>();
            SceneType currentSceneType = sceneEssentials != null ? sceneEssentials.sceneType : SceneType.Unknown;

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

            sceneEssentials.InitScene();
        }

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

            //call Loading Screen 
            if (useLoadingScreen)
            {
                GlobalUIManager.Instance.CallLoadingScreen();
            }

            //wait for loading scene transition first
            yield return new WaitForSeconds(1f);

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

            while (!asyncLoad.isDone)
            {
                yield return null; // Wait until the scene fully loads
            }

            currentScene = sceneName;
            
            // Update loading state after scene loads (loading screen will handle its own disable)
            if (useLoadingScreen)
            {
                UpdateLoadingScreenState();
            }

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

        /// <summary>
        /// Updates the loading screen state by checking both scene loading and UI visibility.
        /// Fires OnLoadingScreenStateChanged event when state changes.
        /// </summary>
        public void UpdateLoadingScreenState()
        {
            bool wasProgressing = isLoadingScreenProgressing;
            
            // Check if loading screen UI is visible
            bool uiLoading = GlobalUIManager.Instance != null && 
                           GlobalUIManager.Instance.canvasLoading != null && 
                           GlobalUIManager.Instance.canvasLoading.gameObject.activeSelf;
            
            isLoadingScreenProgressing = uiLoading;

            if (wasProgressing != isLoadingScreenProgressing)
            {
                OnLoadingScreenStateChanged?.Invoke(isLoadingScreenProgressing);
            }
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

