using UnityEngine;
using UnityEngine.UI;

namespace MLTD.GenericSystem
{

    [RequireComponent(typeof(Toggle))]
    public class UI_ToggleEffects : UI_InputEffects
    {
        [SerializeField] Toggle toggle;

        void Reset()
        {
            toggle = GetComponent<Toggle>();
        }

        void Awake()
        {
            if(toggle == null)
                toggle = GetComponent<Toggle>();
        }
    }

}