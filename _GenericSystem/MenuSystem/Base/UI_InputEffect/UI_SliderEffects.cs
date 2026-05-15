using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MLTD.GenericSystem
{

    public class UI_SliderEffects : UI_InputEffects, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] Slider slider;

        bool isMouseDragging;

        void Reset()
        {
            slider = GetComponent<Slider>();
        }

        void Awake()
        {
            slider = slider ?? GetComponent<Slider>();
            slider.onValueChanged.AddListener(OnSliderValueChanged);
        }
        
        void OnSliderValueChanged(float value)
        {
            // Mouse dragging → do nothing
            if (isMouseDragging &&
                menuManager.inputManager.CurrentDeviceInput == DeviceInputType.MouseKeyboard)
                return;

            // Gamepad → play per step
            if (menuManager.inputManager.CurrentDeviceInput == DeviceInputType.Gamepad)
            {
                PlayAudioWhenSelected(playAudioWhenSelected);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (menuManager.inputManager.CurrentDeviceInput == DeviceInputType.MouseKeyboard)
                isMouseDragging = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (menuManager.inputManager.CurrentDeviceInput == DeviceInputType.MouseKeyboard)
            {
                isMouseDragging = false;
                PlayAudioWhenSelected(playAudioWhenSelected);
            }
        }


    }

}