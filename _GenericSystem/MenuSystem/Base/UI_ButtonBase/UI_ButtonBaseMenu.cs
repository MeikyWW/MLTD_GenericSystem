using UnityEngine;
using UnityEngine.UI;

namespace MLTD.GenericSystem
{

    public class UI_ButtonBaseMenu : MonoBehaviour
    {
        public UI_MenuManager menuManager;
        [SerializeField] protected Button button;

        void Awake()
        {
            button = GetComponent<Button>();
        }

        void Reset()
        {
            button = GetComponent<Button>();
        }
        
    }
}