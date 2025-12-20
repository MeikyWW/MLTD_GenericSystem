using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MLTD.GenericSystem
{

    public class UI_PopUpBase : MonoBehaviour
    {
        [SerializeField] public UI_MenuManager menuManager;

        [SerializeField] public TMP_Text textConfirmation;

        [SerializeField] CanvasGroup cachedBehindCanvas;

        [SerializeField] public RectTransform rootUi;
        
        [SerializeField] public GameObject firstSelected;
        
        [SerializeField] public GameObject previousMenuLastSelected;

        public bool isOpen { get; private set; }

        protected System.Action<bool> onResult;


        void Start()
        {
            rootUi.gameObject.SetActive(false);
        }

        public virtual void OpenPopUp(string message, System.Action<bool> resultCallback, Button pressedButton)
        {
            if (rootUi.gameObject.activeSelf)//prevent reopening self
                return;

            textConfirmation.text = message;
            onResult = resultCallback;
            DisableMenuBehind();

            rootUi.gameObject.SetActive(true);
            isOpen = true;

            SelectFirstObject();
            previousMenuLastSelected = pressedButton.gameObject;
        }

        public virtual void ClosePopUp()
        {
            rootUi.gameObject.SetActive(false);
            RestoreMenuBehind();
            onResult = null;
            isOpen = false;

        }

    // Called by UI buttons
    public void Confirm(bool value)
    {
        // Debug.Log("Confirm ="+value);
        onResult?.Invoke(value);
        ClosePopUp();
    }

    public void SelectFirstObject()
    {
        //selected first object must not play a sound
        firstSelected.GetComponent<UI_InputEffects>().playAudioWhenHighlight = false;
        firstSelected.GetComponent<UI_InputEffects>().playAudioWhenSelected = false;
        EventSystem.current.SetSelectedGameObject(firstSelected);
        firstSelected.GetComponent<UI_InputEffects>().playAudioWhenHighlight = true;
        firstSelected.GetComponent<UI_InputEffects>().playAudioWhenSelected = true;
    }

    void DisableMenuBehind()
    {
        if (menuManager == null) return;

        var currentMenu = menuManager.currentMenu; // your active menu
        cachedBehindCanvas = currentMenu.GetComponent<CanvasGroup>();

        if (cachedBehindCanvas == null)
            cachedBehindCanvas = currentMenu.gameObject.AddComponent<CanvasGroup>();

        cachedBehindCanvas.interactable = false;
        cachedBehindCanvas.blocksRaycasts = false;
        
    }

    void RestoreMenuBehind()
    {
        if (cachedBehindCanvas == null) return;

        cachedBehindCanvas.interactable = true;
        cachedBehindCanvas.blocksRaycasts = true;

        if (menuManager != null)

        //set selections
        if(menuManager.idm.CurrentDeviceInput == DeviceInputType.Gamepad)
        menuManager.SelectUI_FirstObject(previousMenuLastSelected);

        menuManager.lastSelectedUI = menuManager.currentMenu.firstSelected;
    }

    public void OnCancel(BaseEventData eventData)
    {
        eventData.Use();   // 👈 VERY IMPORTANT
        ClosePopUp();
    }
}
}