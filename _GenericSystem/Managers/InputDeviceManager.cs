using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace MLTD.GenericSystem
{
    public class InputDeviceManager : MonoBehaviour
    {
        public static InputDeviceManager Instance { get; private set; } 
        
        [SerializeField] private DeviceInputType currentDeviceInput;
        public DeviceInputType CurrentDeviceInput => currentDeviceInput;

        public static bool blockNavigationThisFrame = false;

        public event System.Action<DeviceInputType> OnDeviceChanged;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this; // Assign the instance
            }

            InputSystem.onAnyButtonPress.Call(OnAnyInput);
            //InputSystem.onEvent += OnInputEvent;
        }

        public void OnAnyInput(InputControl control)
        {
            DeviceDetection(control);

        // Debug.Log("currentSelectedGameObject: " + EventSystem.current.currentSelectedGameObject);
        }

        void DeviceDetection(InputControl control)
        {
            if (control.device is Gamepad)
            {
                if (currentDeviceInput != DeviceInputType.Gamepad)
                {
                    currentDeviceInput = DeviceInputType.Gamepad;
                    OnDeviceChanged?.Invoke(currentDeviceInput);  // Notify listeners
                    DisableMouseCursor();
                    Debug.Log("Device is change to:"+ currentDeviceInput);
                }
            }
            else if (control.device is Touchscreen)
            {
                if (currentDeviceInput != DeviceInputType.TouchScreen)
                {
                    currentDeviceInput = DeviceInputType.TouchScreen;
                    OnDeviceChanged?.Invoke(currentDeviceInput);  // Notify listeners
                    EnableMouseCursor(true); // Touch needs UI visible
                    Debug.Log("Device is change to:"+ currentDeviceInput);
                }
            }
        
            else if (control.device is Keyboard)
            {
                if (currentDeviceInput != DeviceInputType.MouseKeyboard)
                {
                    currentDeviceInput = DeviceInputType.MouseKeyboard;
                    OnDeviceChanged?.Invoke(currentDeviceInput);  // Notify listeners
                    EnableMouseCursor();
                    Debug.Log("Device is change to:"+ currentDeviceInput);
                }
            }

            else if (control.device is Mouse)
            {
                if (!IsMouseClick(control)) return;

                if (currentDeviceInput != DeviceInputType.MouseKeyboard)
                {
                    currentDeviceInput = DeviceInputType.MouseKeyboard;
                    OnDeviceChanged?.Invoke(currentDeviceInput);  // Notify listeners
                    EnableMouseCursor();
                    Debug.Log("Device is change to:"+ currentDeviceInput);
                }
            }
        }

        private void OnInputEvent(InputEventPtr eventPtr, InputDevice device)
        {
            OnAnyInput(device);
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
    }

    public enum DeviceInputType
    {
        MouseKeyboard,
        Gamepad,
        TouchScreen,
    }
}
