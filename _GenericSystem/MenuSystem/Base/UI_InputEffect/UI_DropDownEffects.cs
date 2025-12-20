using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MLTD.GenericSystem
{

    [RequireComponent(typeof(TMP_Dropdown))]
    public class UI_DropDownEffects : UI_InputEffects
    {
        [SerializeField] TMP_Dropdown dropdown;

        void Awake()
        {
            if(dropdown == null)
                dropdown = GetComponent<TMP_Dropdown>();
        }

        void Reset()
        {
            dropdown = GetComponent<TMP_Dropdown>();
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
        }

        public override void OnSubmit(BaseEventData eventData)
        {
            base.OnSubmit(eventData);
            StartCoroutine(GetListNextFrame());
        }

        // public override void OnPointerClick(PointerEventData eventData)
        // {
        //     base.OnPointerClick(eventData);
        //     StartCoroutine(GetListNextFrame());
        // }

        [SerializeField] UI_DropdownList dropdownList;
        
        private void OnDisable()
        {
            dropdownList = null;
        }

        IEnumerator GetListNextFrame()
        {
            // Wait until TMP instantiates "Dropdown List"
            Transform listTransform = null;

            while (listTransform == null)
            {
                yield return null; // wait one frame
                listTransform = dropdown.transform.Find("Dropdown List");
            }

            // Now the object is guaranteed to exist
            dropdownList = listTransform.AddComponent<UI_DropdownList>();
        }
    }
}