using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MLTD.GenericSystem
{
    public class InputGameplayManager : MonoBehaviour
    {
        public static InputGameplayManager Instance { get; private set; } 
            
        //Gameplay
        public InputAction attackAction;
        public InputAction speAttackAction;
        public InputAction moveAction;
        public InputAction jumpAction;
        public InputAction dashAction;
        public InputAction interactAction;
        public InputAction pauseAction;

        //Dialogue
        [HideInInspector] public InputAction nextDialogueAction;

        //Menu
        [HideInInspector] public InputAction pauseMenuAction;
        [HideInInspector] public InputAction nextMenuAction;
        [HideInInspector] public InputAction previousMenuAction;
        [HideInInspector] public InputAction upSelectAction;
        [HideInInspector] public InputAction downSelectAction;
        [HideInInspector] public InputAction leftSelectAction;
        [HideInInspector] public InputAction rightSelectAction;
        [HideInInspector] public InputAction confirmAction;
        [HideInInspector] public InputAction returnAction;

#region GENERIC_INPUT (safe for package)
        //UI
        [HideInInspector] public InputAction TabLeftAction;
        [HideInInspector] public InputAction TabRightAction;
#endregion

        public string currentActionMap;
        [SerializeField] PlayerInput playerInput;

        public GameObject currentSelectedGO;
        
        //plugins
        //public FixedJoystick movementJoystick;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this; // Assign the instance
            }
        }

        private void Start()
        {
            DisableAllInput(); // Start disabled (During Splash Screen)
            //currentActionMap = playerInput?.currentActionMap?.name;

            //movementJoystick = FindAnyObjectByType<FixedJoystick>();
            GlobalGameManager.Instance.OnGameStateChanged += OnInputMapChange;
            OnInputMapChange(GlobalGameManager.Instance.CurrentGameState);
        }

        void OnInputMapChange(GameStates state)
        {
            switch (state)
            {
                case GameStates.Splash:
                    DisableAllInput();                    
                    currentActionMap = "Input Disabled";
                    playerInput.DeactivateInput(); // prevents PlayerInput callbacks too
                    break;

                case GameStates.Title:
                    SwitchActionMap("UI");
                    currentActionMap = "UI";
                    playerInput.ActivateInput();
                    break;
                
                case GameStates.MainGameplay:
                    SwitchActionMap("Player");
                    currentActionMap = "Player";   
                    playerInput.ActivateInput(); 
                    break;
                
                case GameStates.Cutscene:
                    
                    break;
            }
        }
        
    #region Init

        public void InitPlayerInput()
        {
            InputActionMap actionMapPlayer = playerInput.actions.FindActionMap("Player");
            moveAction = actionMapPlayer.FindAction("Move");
            jumpAction = actionMapPlayer.FindAction("Jump");
            dashAction = actionMapPlayer.FindAction("Dash");
            interactAction = actionMapPlayer.FindAction("Interact");
            pauseAction = actionMapPlayer.FindAction("Pause");
            attackAction = actionMapPlayer.FindAction("Attack");
            speAttackAction = actionMapPlayer.FindAction("SpecialAttack");
        }

        public void InitMenuInput()
        {
            InputActionMap actionMapMenu = playerInput.actions.FindActionMap("Menu");
            pauseAction = actionMapMenu.FindAction("Pause");
            nextMenuAction = actionMapMenu.FindAction("NextTab");
            previousMenuAction = actionMapMenu.FindAction("PreviousTab");
            upSelectAction = actionMapMenu.FindAction("Up");
            downSelectAction = actionMapMenu.FindAction("Down");
            leftSelectAction = actionMapMenu.FindAction("Left");
            rightSelectAction = actionMapMenu.FindAction("Right");
            confirmAction = actionMapMenu.FindAction("Confirm");
            returnAction = actionMapMenu.FindAction("Return");
        }

        public void InitUIInput()
        {
            InputActionMap actionMapMenu = playerInput.actions.FindActionMap("UI");
            TabLeftAction = actionMapMenu.FindAction("TabLeft");
            TabRightAction = actionMapMenu.FindAction("TabRight");
        }

        private void InitDialogueActions()
        {
            InputActionMap actionMapDialogue = playerInput.actions.FindActionMap("Dialogue");
            nextDialogueAction = actionMapDialogue.FindAction("NextDialogue");
        }

    #endregion

    #region Handler
        public void SwitchActionMap(string actionMap)
        {
            DisableAllInput();
            playerInput.SwitchCurrentActionMap(actionMap);
            EnableAllInputForCurrentActionMap();
        }

        private IEnumerable<InputAction> GetAllActions()
        {
            //Main Gameplay
            yield return attackAction;
            yield return speAttackAction;
            yield return moveAction;
            yield return jumpAction;
            yield return dashAction;
            yield return interactAction;
            yield return pauseAction;

            //Dialogue
            yield return nextDialogueAction;

            //Menu
            yield return pauseMenuAction;
            yield return nextMenuAction;
            yield return previousMenuAction;
            yield return upSelectAction;
            yield return downSelectAction;
            yield return leftSelectAction;
            yield return rightSelectAction;
            yield return confirmAction;
            yield return returnAction;

            //UI
            yield return TabLeftAction;
            yield return TabRightAction;
        }

        public void DisableAllInput()
        {
            foreach (var action in GetAllActions())
            {
                action?.Disable();
            }
        }

        public void EnableAllInputForCurrentActionMap()
        {
            playerInput.ActivateInput(); // re-enable PlayerInput internal system

            string currentMap = playerInput.currentActionMap.name;

            if (currentMap == "Player")
                InitPlayerInput();
            else if (currentMap == "Menu")
                InitMenuInput();
            else if (currentMap == "Dialogue")
                InitDialogueActions();
            else if (currentMap == "UI")
                InitUIInput();

            currentActionMap = currentMap;
        }

    #endregion

        //utility
        public float holdRepeatDelay = 0.5f;   // Delay before first repeat
        public float holdRepeatRate = 0.1f;    // Interval between repeats

        public void HandleHoldInput(InputAction inputAction, ref float timer, System.Action action)
        {
            if (inputAction.WasPressedThisFrame())
            {
                action.Invoke();
                timer = Time.time + holdRepeatDelay;
            }
            else if (inputAction.IsPressed())
            {
                if (Time.time >= timer)
                {
                    action.Invoke();
                    timer = Time.time + holdRepeatRate;
                }
            }
            else if (inputAction.WasReleasedThisFrame())
            {
                timer = 0f; // Reset on release
            }
        }
    }
}