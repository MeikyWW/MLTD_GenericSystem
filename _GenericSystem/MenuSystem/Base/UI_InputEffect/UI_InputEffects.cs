using UnityEngine;
using UnityEngine.EventSystems;
using System;

namespace MLTD.GenericSystem
{

    public class UI_InputEffects : MonoBehaviour , 
        IPointerEnterHandler, 
        IPointerExitHandler, 
        IPointerClickHandler, 
        ISelectHandler,
        ISubmitHandler,
        ICancelHandler
    {
        [SerializeField] public bool playAudioWhenSelected = true;
        public string audioId_Selected = "BtnSelected_Basic";

        [SerializeField] public bool playAudioWhenHighlight = true;
        public string audioId_Highlight = "BtnHighlight_Basic";

        [SerializeField] public bool playAudioWhenCancelled = true;
        public string audioId_Cancel = "BtnCancel_Basic";

        public Action onSelectAction;


        //Local Scene Manager
        [SerializeField] UI_MenuBase menuBaseOwner;
        [SerializeField] public UI_MenuManager menuManager;

        [SerializeField] public bool hoverAsSelected = false; 

        public void OnPointerEnter(PointerEventData eventData)
        {   
            if (hoverAsSelected)
                EventSystem.current.SetSelectedGameObject(gameObject);
        }

        public void OnPointerExit(PointerEventData eventData)
        {   
            if(menuManager.idm.CurrentDeviceInput == DeviceInputType.MouseKeyboard 
                && hoverAsSelected)
                EventSystem.current.SetSelectedGameObject(null);
        }

        public virtual void OnSelect(BaseEventData eventData)
        {
            PlayAudioWhenHighlight(playAudioWhenHighlight);

            onSelectAction?.Invoke();

            if(menuManager)
                menuManager.lastSelectedUI = EventSystem.current.currentSelectedGameObject;
        }

        public virtual void OnSubmit(BaseEventData eventData) //OnSubmit Gamepad
        {
            if (GetComponent<UI_ButtonBack>() != null)
                PlayAudioWhenCancelled(playAudioWhenCancelled);
            else
                PlayAudioWhenSelected(playAudioWhenSelected); 
        }
        
        public void OnPointerClick(PointerEventData eventData)  //OnSubmit Mouse
        {
            if (GetComponent<UI_ButtonBack>() != null)
                PlayAudioWhenCancelled(playAudioWhenCancelled);
            else
                PlayAudioWhenSelected(playAudioWhenSelected); 
        }

        
        public void OnCancel(BaseEventData eventData) //OnCancel Gamepad
        {
            if (menuManager == null)
            return;

            PlayAudioWhenCancelled(playAudioWhenCancelled);
            eventData.Use(); // 👈 VERY IMPORTANT
            menuManager.HandleCancel();
            
            Debug.Log("Cancel Input");
        
        }

        public void PlayAudioWhenSelected(bool state = true)
        {
            if (state && !string.IsNullOrEmpty(audioId_Selected))
                PlayByAudioId(audioId_Selected);
        }

        public void PlayAudioWhenHighlight(bool state = true)
        {
            if (state && !string.IsNullOrEmpty(audioId_Highlight))
                PlayByAudioId(audioId_Highlight);
        }

        public void PlayAudioWhenCancelled(bool state = true)
        {
            if (state && !string.IsNullOrEmpty(audioId_Cancel))
                PlayByAudioId(audioId_Cancel);
        }

        public void PlayByAudioId(string audioId)
        {
            menuManager.aum.PlayByAudioId(audioId);
        }

    }
}