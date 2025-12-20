using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace MLTD.GenericSystem
{
    public class UI_Menu_TitleSettings : UI_MenuBase
    {
        //[SerializeField] public List<RectTransform> ui_settingContent;
        //[SerializeField] public List<UI_MenuBase> ui_settingContentMenu;

        [Header("Tabs")]
        public List<Button> UI_Tabs; 
        [SerializeField] int currentIndex = 0;
        

        [Header("Input Display")]
        [SerializeField] RectTransform uiImage_lb;
        [SerializeField] RectTransform uiImage_rb;

        // Store delegates so we can unsubscribe correctly
        private System.Action<InputAction.CallbackContext> leftTabCallback;
        private System.Action<InputAction.CallbackContext> rightTabCallback;

        protected override void Awake()
        {
            base.Awake();

            for (int i = 0; i < UI_Tabs.Count; i++)
            {
                int index = i; // capture
                UI_Tabs[index].onClick.AddListener(() => SetSettingsMenu(index));
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            SetTabEffects(0, false); // set default selected menu but don't play sound

            leftTabCallback = ctx => OnNavigateLeftTab(ctx);
            rightTabCallback = ctx => OnNavigateRightTab(ctx);

            menuManager.igm.TabLeftAction.performed += leftTabCallback;
            menuManager.igm.TabRightAction.performed += rightTabCallback;
            
            menuManager.idm.OnDeviceChanged += UpdateGamepadUI;
            UpdateGamepadUI(menuManager.idm.CurrentDeviceInput);
        }

        void OnDisable()
        {
            menuManager.igm.TabLeftAction.performed -= leftTabCallback;
            menuManager.igm.TabRightAction.performed -= rightTabCallback;

            menuManager.idm.OnDeviceChanged -= UpdateGamepadUI;
        }

        public void OnNavigateLeftTab(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;

            currentIndex--;

            if (currentIndex < 0)
                currentIndex = ChildMenu.Count - 1;

            SetSettingsMenu(currentIndex);
        }

        public void OnNavigateRightTab(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;

            currentIndex++;

            if (currentIndex >= ChildMenu.Count)
                currentIndex = 0;

            SetSettingsMenu(currentIndex);
        }

        public void SetSettingsMenu(int index)
        {
            SetSettingsMenu(index, true);
        }

        public void SetSettingsMenu(int index, bool playAudio)
        {
            currentIndex = index;
            menuManager.OpenMenu(ChildMenu[index]);
            
            SetTabEffects(index, playAudio);
        }

        void SetTabEffects(int index, bool playAudio)
        {
            for (int i = 0; i < UI_Tabs.Count; i++)
            {
                UI_Tabs[i].OnDeselect(null);   // reset to normal
            }

            var activeButton = UI_Tabs[index];
            activeButton.OnSelect(null);

            // 3. Optional audio (gamepad only)
            if (menuManager.idm.CurrentDeviceInput == DeviceInputType.Gamepad)
            {
                activeButton.GetComponent<UI_ButtonEffects>()?.PlayAudioWhenSelected(playAudio);
            }
        }

        void UpdateGamepadUI(DeviceInputType device)
        {
            bool isGamepad = device == DeviceInputType.Gamepad;
            //
            //    or  (Long version)
            //
            // bool isGamepad;
            // if (device == DeviceInputType.Gamepad)
            // {
            //     isGamepad = true;
            // }
            // else
            // {
            //     isGamepad = false;
            // }

            uiImage_lb.gameObject.SetActive(isGamepad);
            uiImage_rb.gameObject.SetActive(isGamepad);
        }
    }
}