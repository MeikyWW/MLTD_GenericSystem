
using UnityEngine;
using UnityEngine.UI;

namespace MLTD.GenericSystem
{

    [RequireComponent(typeof(Button))]
    public class UI_ButtonEffects : UI_InputEffects 
    {
        [SerializeField] Button button;

        void Reset()
        {
            button = GetComponent<Button>();
        }

        void Awake()
        {
            if(button == null)
                button = GetComponent<Button>();
        }

    }
}