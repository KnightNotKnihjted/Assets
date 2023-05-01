using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class GlobalInputManager : SingletonBehaviour<GlobalInputManager>
{
    public static Dictionary<InputAction, float> inputActionsLastPressed = new();
    public static Dictionary<InputAction, bool> inputActionsDoubleTapped = new();
    private float doubleTapThreshold = 0.3f;

    #region Input Master Handling
    public static InputMaster InputMaster;
    private void OnEnable()
    {
        InputMaster = new InputMaster();
        InputMaster.Enable();

        InputActionMap playerActionMap = InputMaster.asset.FindActionMap("Player");

        foreach (InputAction action in playerActionMap.actions)
        {
            inputActionsLastPressed[action] = -doubleTapThreshold;
            inputActionsDoubleTapped[action] = false;
        }
    }

    private void OnDisable()
    {
        InputMaster?.Disable();
    }
    #endregion

    private void Update()
    {
        List<InputAction> actions = new List<InputAction>(inputActionsLastPressed.Keys);
        foreach (InputAction action in actions)
        {
            if (action.WasPressedThisFrame())
            {
                float lastPressedTime = inputActionsLastPressed[action];
                inputActionsDoubleTapped[action] = Time.time - lastPressedTime < doubleTapThreshold;
                inputActionsLastPressed[action] = Time.time;
            }
        }
    }

    public static bool DoubleTap(InputAction inputAction)
    {
        if (inputActionsDoubleTapped.ContainsKey(inputAction))
        {
            bool x = inputActionsDoubleTapped[inputAction];
            inputActionsDoubleTapped[inputAction] = false;
            return x;
        }
        return false;
    }
}