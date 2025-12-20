using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Threading.Tasks;

namespace MLTD.GenericSystem
{
    public class UI_MenuManager : MonoBehaviour
    {
        [Header("Main")]
        [SerializeField] public string MenuManagerId;
        [SerializeField] private Canvas menuCanvas;
        [SerializeField] UI_MenuBase defaultMenu;
        
        [Header("State (ReadOnly)")]
        [SerializeField] public UI_MenuBase currentMenu;
        [SerializeField] public GameObject lastSelectedUI; 

    #if UNITY_EDITOR
        [Header("Stack (ReadOnly)")]
        [SerializeField] private List<UI_MenuBase> displayMenuStack = new();
    #endif
        private Stack<UI_MenuBase> menuStack = new Stack<UI_MenuBase>();

        [Header("All Menu System Object (ReadOnly)")]
        [SerializeField] UI_MenuBase[] allMenuBaseOnScene;
        [SerializeField] UI_InputEffects[] allInputEffect;
        [SerializeField] UI_ButtonBaseMenu[] allButton_OpenMenu;
        [SerializeField] UI_PopUpBase[] allPopUpMenu;

        [Header("Managers (ReadOnly)")]
        public InputDeviceManager idm;
        public InputGameplayManager igm;
        public AudioManager aum;

        private async void Awake()
        {
            await GetUsedManagers();

            Init();
            
            menuStack.Push(defaultMenu);
            currentMenu = defaultMenu;
            lastSelectedUI = currentMenu.firstSelected;

            idm.OnDeviceChanged += OnDeviceChangeSelected;
            OnDeviceChangeSelected(idm.CurrentDeviceInput);

            SyncDisplayStack();
        }

        void OnDeviceChangeSelected(DeviceInputType device)
        {
            bool isGamepad = device == DeviceInputType.Gamepad;

            // Only react to gamepad
            if (device != DeviceInputType.Gamepad)
                return;

            // Only run if THIS menu canvas is active
            if (!menuCanvas.gameObject.activeInHierarchy)
                return;
            
            
            if (lastSelectedUI != null)
                SelectUI_WhileBlockingNavigation(lastSelectedUI);
            else
                SelectUI_WhileBlockingNavigation(currentMenu.firstSelected);
        
        }

        async Task GetUsedManagers()
        {
            idm = InputDeviceManager.Instance;
            aum = AudioManager.Instance;
            igm = InputGameplayManager.Instance;
        }
        
        public void Init()
        {
            allMenuBaseOnScene = menuCanvas.GetComponentsInChildren<UI_MenuBase>(true);
            foreach (var menu in allMenuBaseOnScene)
                RegisterMenuBase(menu);

            allInputEffect = menuCanvas.GetComponentsInChildren<UI_InputEffects>(true);
            foreach (var effect in allInputEffect)
                RegisterInputEffects(effect);

            allButton_OpenMenu = menuCanvas.GetComponentsInChildren<UI_ButtonBaseMenu>(true);
            foreach (var button in allButton_OpenMenu)
                RegisterMenuButton(button);

            allPopUpMenu = menuCanvas.GetComponentsInChildren<UI_PopUpBase>(true);
            foreach (var popUp in allPopUpMenu)
                RegisterPopUpMenu(popUp);
        }

        //===========================================//
        //  Stack                                    //
        //===========================================//
        // Push → put something on top               //
        // Pop → remove the top                      //
        // Peek → look at the top (without removing) //
        //===========================================//

        /// Open a menu
        public void OpenMenu(UI_MenuBase targetMenu)
        {
            //Checks if the same menu
            if (currentMenu == targetMenu) return;

            // First, Close or exit current Menu 
            currentMenu.OnExit(); 
            bool targetMenu_isContainer = targetMenu.ChildMenu.Count > 0 || targetMenu.isContainer;
            bool isSiblings = currentMenu.IsSiblingOf(targetMenu);

            // if the targetMenu has childMenu, register the defaultMenuChild Instead 
            if (targetMenu_isContainer && !isSiblings) 
            { 
                //Debug.Log("Case 1");
                var defaultChildMenu = targetMenu.defaultChild; 
                menuStack.Push(defaultChildMenu); 
                targetMenu.OnEnter();
                defaultChildMenu.OnEnter();
                currentMenu = defaultChildMenu;
            } 
            
            // if the target menu is trying to open and it's siblings else 
            else if (!targetMenu_isContainer && isSiblings) 
            { 
                //Debug.Log("Case 2");
                menuStack.Pop(); 
                menuStack.Push(targetMenu); 
                targetMenu.OnEnter(); 
                currentMenu = targetMenu;
            } 
            
            // if the target menu is trying to open and it's siblings, and it has childMenu 
            else if (targetMenu_isContainer && isSiblings) 
            { 
                //Debug.Log("Case 3");
                var defaultChildMenu = targetMenu.defaultChild; 
                menuStack.Push(defaultChildMenu); 
                targetMenu.OnEnter();
                defaultChildMenu.OnEnter();
                currentMenu = defaultChildMenu;
            } 
            
            //Unrelated condition 
            else 
            {
                //Debug.Log("Case 4");
                menuStack.Push(targetMenu); 
                targetMenu.OnEnter(); 
                currentMenu = targetMenu;
            } 
            
            //refresh stack list On Inspector 
            SyncDisplayStack();

            //  Set Selections
            var resolvedFirstSelection = currentMenu.ResolveFirstSelected();
            if (resolvedFirstSelection != null)
                SelectUI_FirstObject(resolvedFirstSelection);

            lastSelectedUI = resolvedFirstSelection;
        }

        /// Navigate back
        public void Back()
        {
            //On Menu Root
            if (menuStack.Count <= 1)
            {
                //Debug.Log("Already On root");
                return;
            }
                
            //Check if menu has Parent Menu
            var leaveMenu = menuStack.Pop();
            
            if(leaveMenu.parentMenu != null)
            {   Debug.Log("Leave Menu: "+leaveMenu.parentMenu);
                leaveMenu.OnExit();
                leaveMenu.parentMenu.OnExit();
            }
            else
            {
                Debug.Log("Leave Menu: "+leaveMenu);
                leaveMenu.OnExit();
            }
            //Debug.Log("Leaving Menu: "+leaveMenu);

            var previousMenu = menuStack.Peek();
            currentMenu = previousMenu;
            previousMenu.OnEnter();
            
            //Debug.Log("Return to Menu: "+previousMenu);
            SyncDisplayStack();

            //  Set Selections
            var resolvedFirstSelection = previousMenu.ResolveFirstSelected();
            if (resolvedFirstSelection != null)
                SelectUI_FirstObject(resolvedFirstSelection);

            lastSelectedUI = resolvedFirstSelection;
        }

        private void SyncDisplayStack()
        {
    #if UNITY_EDITOR
            displayMenuStack.Clear();

            // Stack enumerates from TOP → BOTTOM
            foreach (var menu in menuStack)
                displayMenuStack.Add(menu);

            // Reverse so Inspector shows ROOT → CURRENT
            displayMenuStack.Reverse();

    #endif   
        }
        
        public void SelectUI(GameObject uiObject)
        {
            if (uiObject == null) return;

            EventSystem.current.SetSelectedGameObject(uiObject);
        }

        //First Selected Object should not play any audio
        public void SelectUI_FirstObject(GameObject uiObject)
        {
            if (uiObject == null) return;

            if (idm.CurrentDeviceInput != DeviceInputType.Gamepad) //only do when the Input is a gamePad
            return;

            //selected first object must not play a sound
            uiObject.GetComponent<UI_InputEffects>().playAudioWhenHighlight = false;
            uiObject.GetComponent<UI_InputEffects>().playAudioWhenSelected = false;
            EventSystem.current.SetSelectedGameObject(uiObject);
            uiObject.GetComponent<UI_InputEffects>().playAudioWhenHighlight = true;
            uiObject.GetComponent<UI_InputEffects>().playAudioWhenSelected = true;
        }

        public void SelectUI_WhileBlockingNavigation(GameObject uiObject)
        {
            if (uiObject == null) return;

            StartCoroutine(BlockNavigationUntilNeutral(uiObject));
        }

        public void RegisterMenuBase(UI_MenuBase menu)
        {
            menu.menuManager = this;
        }

        public void RegisterInputEffects(UI_InputEffects effect)
        {
            effect.menuManager = this;
        }

        public void RegisterMenuButton(UI_ButtonBaseMenu btn)
        {
            btn.menuManager = this;
        }
        public void RegisterPopUpMenu(UI_PopUpBase popUpMenu)
        {
            popUpMenu.menuManager = this;
        }

        private IEnumerator BlockNavigationUntilNeutral(GameObject uiObject)
        {
            EventSystem.current.sendNavigationEvents = false;
            //selected first object must not play a sound
            uiObject.GetComponent<UI_InputEffects>().playAudioWhenHighlight = false;
            uiObject.GetComponent<UI_InputEffects>().playAudioWhenSelected = false;
            EventSystem.current.SetSelectedGameObject(uiObject);

            // Wait 1 frame to lock in selection
            yield return null;

            //float waitTime = 0f;

            while (IsNavigationHeld())
            {
                //waitTime += Time.unscaledDeltaTime;
                //Debug.Log($"Waiting for navigation release: {waitTime:F2}s");
                yield return null;
            }

            EventSystem.current.sendNavigationEvents = true;
            uiObject.GetComponent<UI_InputEffects>().playAudioWhenHighlight = true;
            uiObject.GetComponent<UI_InputEffects>().playAudioWhenSelected = true;
        }

        private bool IsNavigationHeld()
        {
            if (Gamepad.current == null) return false;

            Vector2 nav = Gamepad.current.leftStick.ReadValue();

            // Also check DPad (optional)
            Vector2 dpad = Gamepad.current.dpad.ReadValue();

            return Mathf.Abs(nav.x) > 0.2f || Mathf.Abs(nav.y) > 0.2f ||
                Mathf.Abs(dpad.x) > 0.2f || Mathf.Abs(dpad.y) > 0.2f;
        }    
        

        public bool IsAnyPopUpOpen()
        {
            foreach (var popUp in allPopUpMenu)
            {
                if (popUp.isOpen)
                    return true;
            }
            return false;
        }

        public void HandleCancel()
        {
            // 1️⃣ Popup has priority
            var popup = GetOpenPopup();
            if (popup != null)
            {
                popup.ClosePopUp();
                return;
            }

            // 2️⃣ Otherwise, go back menu
            Back();
        }

        public UI_PopUpBase GetOpenPopup()
        {
            foreach (var popup in allPopUpMenu)
            {
                if (popup != null && popup.isOpen)
                    return popup;
            }
            return null;
        }
    }
}