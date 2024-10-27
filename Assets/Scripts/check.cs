using UnityEngine;
using UnityEngine.InputSystem;  // Required for new Input System

public class check : MonoBehaviour
{
    private InputAction action;

    void OnEnable()
    {
        action = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/g");
        action.performed += ctx => Debug.Log("HI");
        action.Enable();
    }

    void OnDisable()
    {
        action.Disable();
    }
}
