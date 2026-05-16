using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace MLTD.GenericSystem
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }

        [SerializeField] PlayerInput playerInput;
        public PlayerInput PlayerInput => playerInput;

        IDisposable _anyButtonPressSubscription;

#region GENERIC_INPUT (safe for package)
        //UI
        [HideInInspector] public InputAction TabLeftAction;
        [HideInInspector] public InputAction TabRightAction;
#endregion

        public ActionMapType currentActionMap;
        public ActionMapType previousActionMap;

        [Header("Action Map Enabled State (ReadOnly)")]
        [SerializeField] private bool isPlayerActionMapEnabled;
        [SerializeField] private bool isMenuActionMapEnabled;
        [SerializeField] private bool isSequenceActionMapEnabled;
        [SerializeField] private bool isUIActionMapEnabled;
        [SerializeField] private bool isDisabledActionMapEnabled;
        
        public GameObject currentSelectedGO;

        //Events
        public event Action<ActionMapType, ActionMapType> OnActionMapAllDisabled;
        public event Action<ActionMapType, ActionMapType> OnActionMapPlayerEnabled;
        public event Action<ActionMapType, ActionMapType> OnActionMapSequenceEnabled;

        [SerializeField] private DeviceInputType currentDeviceInput;
        public DeviceInputType CurrentDeviceInput => currentDeviceInput;

        public static bool blockNavigationThisFrame = false;

        public event Action<DeviceInputType> OnDeviceChanged;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            _anyButtonPressSubscription = InputSystem.onAnyButtonPress.Call(OnAnyButtonPressed);
        }

        void OnDestroy()
        {
            _anyButtonPressSubscription?.Dispose();
            _anyButtonPressSubscription = null;
        }
        
        #region Input Device System
        void OnAnyButtonPressed(InputControl control)
        {
            DeviceDetection(control);
        }
        // Required by PlayerInput Send Messages for the "AnyInput" action.
        // Device detection is handled via onAnyButtonPress → OnAnyButtonPressed.
        public void OnAnyInput(InputValue value) { }

        void DeviceDetection(InputControl control)
        {
            if (control.device is Gamepad)
            {
                if (currentDeviceInput != DeviceInputType.Gamepad)
                {
                    currentDeviceInput = DeviceInputType.Gamepad;
                    OnDeviceChanged?.Invoke(currentDeviceInput);
                    DisableMouseCursor();
                    Debug.Log("Device is change to:" + currentDeviceInput);
                }
            }
            else if (control.device is Touchscreen)
            {
                if (currentDeviceInput != DeviceInputType.TouchScreen)
                {
                    currentDeviceInput = DeviceInputType.TouchScreen;
                    OnDeviceChanged?.Invoke(currentDeviceInput);
                    EnableMouseCursor(true);
                    Debug.Log("Device is change to:" + currentDeviceInput);
                }
            }
            else if (control.device is Keyboard)
            {
                if (currentDeviceInput != DeviceInputType.MouseKeyboard)
                {
                    currentDeviceInput = DeviceInputType.MouseKeyboard;
                    OnDeviceChanged?.Invoke(currentDeviceInput);
                    EnableMouseCursor();
                    Debug.Log("Device is change to:" + currentDeviceInput);
                }
            }
            else if (control.device is Mouse)
            {
                if (!IsMouseClick(control)) return;

                if (currentDeviceInput != DeviceInputType.MouseKeyboard)
                {
                    currentDeviceInput = DeviceInputType.MouseKeyboard;
                    OnDeviceChanged?.Invoke(currentDeviceInput);
                    EnableMouseCursor();
                    Debug.Log("Device is change to:" + currentDeviceInput);
                }
            }
        }

        bool IsMouseClick(InputControl control)
        {
            return control == Mouse.current.leftButton ||
                control == Mouse.current.rightButton ||
                control == Mouse.current.middleButton;
        }

        void EnableMouseCursor(bool lockCursor = false)
        {
            Cursor.visible = true;
            Cursor.lockState = lockCursor ? CursorLockMode.None : CursorLockMode.None;
        }

        void DisableMouseCursor()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        #endregion

        #region Action Map Handlers
        public void SwitchActionMap(ActionMapType actionMap)
        {
            //Check if the action map is already active
            if (currentActionMap == actionMap) return;

            //Disable all intended action maps except UI
            OnActionMapAllDisabled?.Invoke(currentActionMap, previousActionMap);
            playerInput.actions.FindActionMap("Disabled").Disable();

            //Track Current and previous action map
            previousActionMap = currentActionMap;
            currentActionMap = actionMap;

            //Activate the intended action map
            switch (actionMap)
            {
                case ActionMapType.Player:
                    OnActionMapPlayerEnabled?.Invoke(currentActionMap, previousActionMap);
                    break;

                case ActionMapType.Sequence:
                    OnActionMapSequenceEnabled?.Invoke(currentActionMap, previousActionMap);
                    break;

                case ActionMapType.UI:
                    ActivateActionMapUI();
                    break;
                case ActionMapType.Disabled:
                    playerInput.actions.FindActionMap("Disabled").Enable();
                    break;
            }

            ChackAllActionMapState();
        }

        public void ChackAllActionMapState()
        {
            isPlayerActionMapEnabled = IsActionMapEnabledInAsset("Player");
            isSequenceActionMapEnabled = IsActionMapEnabledInAsset("Sequence");
            isUIActionMapEnabled = IsActionMapEnabledInAsset("UI");
            isDisabledActionMapEnabled = IsActionMapEnabledInAsset("DIsabled");
        }

        bool IsActionMapEnabledInAsset(string mapName)
        {
            InputActionMap map = playerInput.actions.FindActionMap(mapName);
            return map != null && map.enabled;
        }

        public void BacktoPreviousActionMap()
        {
            ActionMapType mapBeforeSwitch = previousActionMap;

            previousActionMap = currentActionMap;

            SwitchActionMap(mapBeforeSwitch);
        }



        public void ActivateActionMapUI()
        {
            InputActionMap actionMapUI = playerInput.actions.FindActionMap("UI");
            actionMapUI.Enable();

            TabLeftAction = actionMapUI.FindAction("TabLeft");
            TabRightAction = actionMapUI.FindAction("TabRight");

            TabLeftAction.performed -= OnTabLeftPerformed;
            TabRightAction.performed -= OnTaRightPerformed;

            TabLeftAction.performed += OnTabLeftPerformed;
            TabRightAction.performed += OnTaRightPerformed;

            ChackAllActionMapState();
        }

        
        #endregion

        void OnTabLeftPerformed(InputAction.CallbackContext ctx)
        {
        }

        void OnTaRightPerformed(InputAction.CallbackContext ctx)
        {
        }

        

        public float holdRepeatDelay = 0.5f;
        public float holdRepeatRate = 0.1f;

        public void HandleHoldInput(InputAction inputAction, ref float timer, Action action)
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
                timer = 0f;
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

    public enum DeviceInputType
    {
        MouseKeyboard,
        Gamepad,
        TouchScreen,
    }
}
