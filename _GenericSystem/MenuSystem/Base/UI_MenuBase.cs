using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace MLTD.GenericSystem
{
    public abstract class UI_MenuBase : MonoBehaviour
    {
        //[SerializeField] public string menu_id;

        [Header("State")]

        [SerializeField] public UI_MenuManager menuManager;

        
        [Header("Menu Relations (Must not be null)")]
        public UI_MenuBase parentMenu;
        [SerializeField] private List<UI_MenuBase> childMenu;
        public IReadOnlyList<UI_MenuBase> ChildMenu => childMenu;
        
        [Header("Menu Container ([Default Child] must not be null if [Is Container] is true)")]
        [SerializeField] public bool isContainer; //contaner only
        public UI_MenuBase defaultChild;

        [Header("Input Selectable")]
        [SerializeField] public GameObject firstSelected;
        [SerializeField] public UI_InputEffects[] menuInputEffects;

        [Header("Events")]
        [SerializeField] private UnityEvent onEnterEvent;
        [SerializeField] private UnityEvent onExitEvent;
        
        protected virtual void Awake()
        {
            //CacheChildren();
        }

        public virtual void OnEnter()
        {
            onEnterEvent.Invoke();
            gameObject.SetActive(true);
        }

        public virtual void OnExit()
        {
            onExitEvent.Invoke();
            gameObject.SetActive(false);
        }

        protected virtual void OnEnable()
        {
            //gett all input effect on this menu
            menuInputEffects = GetComponentsInChildren<UI_InputEffects>(true);
            
            if (firstSelected != null)
            {
                EventSystem.current.firstSelectedGameObject = firstSelected;
                
                if (InputDeviceManager.Instance.CurrentDeviceInput == DeviceInputType.Gamepad)
                {   
                    menuManager.SelectUI_FirstObject(firstSelected);
                }
            }
        }

        public GameObject ResolveFirstSelected()
        {
            // 1. This menu owns a selectable
            if (firstSelected != null)
                return firstSelected;

            // 2. Prefer default child
            if (defaultChild != null)
            {
                var resolved = defaultChild.ResolveFirstSelected();
                if (resolved != null)
                    return resolved;
            }

            // 3. Fallback: search other children
            if (childMenu == null)
                return null;

            foreach (var child in childMenu)
            {
                if (child == null || child == defaultChild)
                    continue;

                var resolved = child.ResolveFirstSelected();
                if (resolved != null)
                    return resolved;
            }

            return null;
        }
        
        private void CacheChildren()
        {
            childMenu = new List<UI_MenuBase>();

            var allMenus = GetComponentsInChildren<UI_MenuBase>(true); // true = include inactive

            //filter the ones that are not the direct child
            foreach (var menu in allMenus)
            {
                if (menu == this)
                continue;

                if (menu.parentMenu == this) // direct child
                    childMenu.Add(menu);
            }

            if (defaultChild == null)
            {
                defaultChild = allMenus[0];
            }

        }

        public bool IsSiblingOf(UI_MenuBase other)
        {
            if (other == null)
                return false;

            // Cannot be sibling of itself
            if (other == this)
                return false;

            // Both must have a parent
            if (parentMenu == null || other.parentMenu == null)
                return false;

            // Same parent = siblings
            return parentMenu == other.parentMenu;
        }
    }

}