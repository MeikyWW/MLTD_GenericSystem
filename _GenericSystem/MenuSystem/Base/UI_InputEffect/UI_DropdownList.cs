using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MLTD.GenericSystem
{
    public class UI_DropdownList : MonoBehaviour
    {
        [SerializeField] RectTransform listRoot;   // THIS object
        [SerializeField] public ScrollRect scrollRect;
        [SerializeField] public Scrollbar scrollbar;
        [SerializeField] public RectTransform content;
        [SerializeField] public Toggle[] toggles;
        [SerializeField] public RectTransform viewport;

        
        public int itemHeight = 30;         // Height of each dropdown option
        public int maxVisibleItems = 6;

        void Awake()
        {
            listRoot = GetComponent<RectTransform>();
            
            scrollRect = GetComponent<ScrollRect>();
            scrollbar = scrollRect.verticalScrollbar;
            content = scrollRect.content;
            viewport = scrollRect.viewport;
            toggles = GetComponentsInChildren<Toggle>();

            SetupToggleEvent();
            AdjustHeight();
        }

        void AdjustHeight()
        {
            RectTransform itemRT = toggles[0].GetComponent<RectTransform>();
            float itemHeight = itemRT.rect.height;

            int count = toggles.Length;
            int visible = Mathf.Min(count, maxVisibleItems);

            float contentHeight = itemHeight * count;
            float listHeight = itemHeight * visible;

            // Resize Content (true full list)
            content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, contentHeight);

            // Resize Viewport (window)
            viewport.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, listHeight);

            // Resize ROOT (the dropdown list object you showed)
            listRoot.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, listHeight);

            scrollRect.verticalNormalizedPosition = 1f;
        }
        void SetupToggleEvent()
        {
            for (int i = 0; i < toggles.Length; i++)
            {
                // Add hover event
                UI_InputEffects inputEffects = toggles[i].gameObject.AddComponent<UI_InputEffects>();
                
                inputEffects.audioId_Highlight = "BtnSelected_Basic";
                inputEffects.audioId_Selected = "BtnHighlight_Basic";
                inputEffects.playAudioWhenHighlight = false;
                inputEffects.playAudioWhenSelected = false;

                int capturedIndex = i; 
                
                inputEffects.onSelectAction += () => OnSelectIndex(capturedIndex);
            }
        }

        void OnSelectIndex(int index)
        {
            if(InputDeviceManager.Instance.CurrentDeviceInput == DeviceInputType.Gamepad)
                ScrollToIndex(index);
        }

        public int GetItemCount()
        {
            return toggles.Length;
        }

        public void ScrollToIndex(int index, float duration = 0.2f)
        {
            StartCoroutine(SmoothScrollRoutine(index, duration));
        }

        IEnumerator SmoothScrollRoutine(int index, float duration)
        {
            float itemCount = toggles.Length;
            float target = 1f - (index / (itemCount - 1f));
            float start = scrollbar.value;
            float t = 0;

            while (t < duration)
            {
                t += Time.deltaTime;
                float v = Mathf.Lerp(start, target, t / duration);
                scrollbar.value = v;
                yield return null;
            }

            scrollbar.value = target;
        }
    }
}