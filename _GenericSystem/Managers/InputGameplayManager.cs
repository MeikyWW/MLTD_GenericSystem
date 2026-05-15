using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MLTD.GenericSystem
{
    public class InputGameplayManager : MonoBehaviour
    {
        public static InputGameplayManager Instance { get; private set; }

        [SerializeField] PlayerInput playerInput;
        public PlayerInput PlayerInput => playerInput;

#region GENERIC_INPUT (safe for package)
        //UI
        [HideInInspector] public InputAction TabLeftAction;
        [HideInInspector] public InputAction TabRightAction;
#endregion

        public ActionMapType currentActionMap;
        public ActionMapType previousActionMap;


        public GameObject currentSelectedGO;

        //Events
        public event Action<ActionMapType, ActionMapType> OnActionMapAllDisabled;
        public event Action<ActionMapType, ActionMapType> OnActionMapPlayerEnabled;
        public event Action<ActionMapType, ActionMapType> OnActionMapSequenceEnabled;

        
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this; // Assign the instance
            }
        }

        void OnDestroy()
        {
            // if (GlobalGameManager.Instance != null)
            //     GlobalGameManager.Instance.OnGameStateChanged -= ApplyActionMapFromGameState;
        }

        private void Start()
        {

            // GlobalGameManager.Instance.OnGameStateChanged += ApplyActionMapFromGameState;
            // ApplyActionMapFromGameState(GlobalGameManager.Instance.CurrentGameState);
        }

        #region Handler
        public void SwitchActionMap(ActionMapType actionMap)
        {
            if (currentActionMap == actionMap) return;

            //Signal all Outside the generic ActionMap to be disabled, they live outside of Generic System.
            OnActionMapAllDisabled?.Invoke(currentActionMap, currentActionMap);


            previousActionMap = currentActionMap;
            currentActionMap = actionMap;

            switch(actionMap)
            {
                case ActionMapType.Player:
                    OnActionMapPlayerEnabled?.Invoke(previousActionMap, currentActionMap);
                    break;

                case ActionMapType.Sequence:
                    OnActionMapSequenceEnabled?.Invoke(previousActionMap, currentActionMap);
                    break;

                case ActionMapType.UI:
                    //Special: ActionMap UI is never disabled (It doesn't have Disabler), But can be called, meaning all other action map is disabled.
                    ActivateActionMapUI();
                    break;
                case ActionMapType.Disabled:
                    //Doesn't Send any signal so essentially all actionmap is disabled
                    break;
            }

            //Signal specific Action Map outside the generic ActionMap to be disabled, they live outside of Generic System.
        }

        public void BacktoPreviousActionMap()
        {   
            ActionMapType mapBeforeSwitch = previousActionMap;

            previousActionMap = currentActionMap;

            SwitchActionMap(mapBeforeSwitch);
        }

        #endregion

        // void ApplyActionMapFromGameState(GameStates state)
        // {
        //     switch (state)
        //     {
        //         case GameStates.Splash:        
        //             SwitchActionMap(ActionMapType.Disabled);
        //             break;

        //         case GameStates.Title:
        //             SwitchActionMap(ActionMapType.UI);
        //             break;
                
        //         case GameStates.MainGameplay:
        //             SwitchActionMap(ActionMapType.Player); 
        //             break;
                
        //         case GameStates.Cutscene:
        //             SwitchActionMap(ActionMapType.Disabled);
        //             break;
        //     }
        // }
        
        //Used for Title Screen so it is valid for Generic System
    #region UI Action Map 

        public void ActivateActionMapUI()
        {
            InputActionMap actionMapUI = playerInput.actions.FindActionMap("Dialogue");
            actionMapUI.Enable();

            TabLeftAction = actionMapUI.FindAction("TabLeft");
            TabRightAction = actionMapUI.FindAction("TabRight");

            TabLeftAction.performed -= OnTabLeftPerformed;
            TabRightAction.performed -= OnTaRightPerformed;

            TabLeftAction.performed += OnTabLeftPerformed;
            TabRightAction.performed += OnTaRightPerformed;
        }


        void OnTabLeftPerformed(InputAction.CallbackContext ctx)
        {

        }

        void OnTaRightPerformed(InputAction.CallbackContext ctx)
        {

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
