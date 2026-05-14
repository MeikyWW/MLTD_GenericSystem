using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MLTD.GenericSystem
{
    public class InputGameplayManager : MonoBehaviour
    {
        public static InputGameplayManager Instance { get; private set; }

        /// <summary>Same <see cref="PlayerInput"/> used for all action maps; gameplay bindings live in the game project.</summary>
        public PlayerInput PlayerInput => playerInput;

        //Sequence
        [HideInInspector] public InputAction ConfirmAction;

#region GENERIC_INPUT (safe for package)
        //UI
        [HideInInspector] public InputAction TabLeftAction;
        [HideInInspector] public InputAction TabRightAction;
#endregion

        public string currentActionMap;
        public string previousActionMap;
        [SerializeField] PlayerInput playerInput;

        public GameObject currentSelectedGO;
        
        //plugins
        //public FixedJoystick movementJoystick;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this; // Assign the instance
            }
        }

        private void Start()
        {
            DisableAllInput(); // Start disabled (During Splash Screen)

            GlobalGameManager.Instance.OnGameStateChanged += OnInputMapChange;
            OnInputMapChange(GlobalGameManager.Instance.CurrentGameState);
        }

        void OnInputMapChange(GameStates state)
        {
            switch (state)
            {
                case GameStates.Splash:        
                    SwitchActionMap(ActionMapType.Disabled.ToString());
                    break;

                case GameStates.Title:
                    SwitchActionMap(ActionMapType.UI.ToString());
                    break;
                
                case GameStates.MainGameplay:
                    SwitchActionMap(ActionMapType.Player.ToString()); 
                    break;
                
                case GameStates.Cutscene:
                    SwitchActionMap(ActionMapType.Disabled.ToString());
                    break;
            }
        }
        
    #region Init

        public void DisablePlayerInput()
        {
            playerInput.DeactivateInput();
        }

        public void InitUIInput()
        {
            playerInput.ActivateInput();
            InputActionMap actionMapMenu = playerInput.actions.FindActionMap(ActionMapType.UI.ToString());
            TabLeftAction = actionMapMenu.FindAction("TabLeft");
            TabRightAction = actionMapMenu.FindAction("TabRight");
        }

        private void InitSequenceActions()
        {
            playerInput.ActivateInput();
            InputActionMap actionMapSequence = playerInput.actions.FindActionMap(ActionMapType.Sequence.ToString());
            ConfirmAction = actionMapSequence.FindAction("Confirm");
        }

    #endregion

    #region Handler
        public void SwitchActionMap(string actionMap)
        {
            if (currentActionMap == actionMap) return;

            previousActionMap = currentActionMap;
            currentActionMap = actionMap;

            DisableAllInput();
            playerInput.SwitchCurrentActionMap(actionMap);
            InitSelectedActionMap();
        }

        public void BacktoPreviousActionMap()
        {   
            if (string.IsNullOrEmpty(previousActionMap)) return;

            string temp = currentActionMap;

            currentActionMap = previousActionMap;
            previousActionMap = temp;

            DisableAllInput();
            playerInput.SwitchCurrentActionMap(currentActionMap);
            InitSelectedActionMap();
        }

        private IEnumerable<InputAction> GetAllActions()
        {
            //Sequence
            yield return ConfirmAction;

            //UI
            yield return TabLeftAction;
            yield return TabRightAction;
        }

        public void DisableAllInput()
        {
            foreach (var action in GetAllActions())
            {
                action?.Disable();
            }
        }

        public void InitSelectedActionMap()
        {
            if (currentActionMap == ActionMapType.Player.ToString())
            {
                // Player map actions are bound in the game project (MainGameplayInputManager.BindPlayerActions).
            }
            else if (currentActionMap == ActionMapType.Sequence.ToString())
                InitSequenceActions();
            else if (currentActionMap == ActionMapType.UI.ToString())
                InitUIInput();
        }

    #endregion

        //utility
        public float holdRepeatDelay = 0.5f;   // Delay before first repeat
        public float holdRepeatRate = 0.1f;    // Interval between repeats

        public void HandleHoldInput(InputAction inputAction, ref float timer, System.Action action)
        {
            if (inputAction.WasPressedThisFrame())
            {
                action.Invoke();
                timer = Time.time + holdRepeatDelay;
            }
            else if (inputAction.IsPressed())
            {
                if (Time.time >= timer)
                {
                    action.Invoke();
                    timer = Time.time + holdRepeatRate;
                }
            }
            else if (inputAction.WasReleasedThisFrame())
            {
                timer = 0f; // Reset on release
            }
        }
        [ContextMenu("SwitchActionMap_Player")]
        public void SwitchActionMap_Player()
        {
            SwitchActionMap(ActionMapType.Player.ToString());
        }
        [ContextMenu("SwitchActionMap_Menu")]
        public void SwitchActionMap_Menu()
        {
            SwitchActionMap(ActionMapType.Menu.ToString());
        }
        [ContextMenu("SwitchActionMap")]
        public void SwitchActionMap_UI()
        {
            SwitchActionMap(ActionMapType.UI.ToString());
        }
        [ContextMenu("SwitchActionMap_Sequence")]
        public void SwitchActionMap_Sequence()
        {
            SwitchActionMap(ActionMapType.Sequence.ToString());
        }
    }
}

public enum ActionMapType
{
    Disabled,
    Player,
    Menu,
    Sequence,
    UI
}
