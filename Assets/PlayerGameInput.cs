using Player;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGameInput : MonoBehaviour
{
    public IInputReceiver Player;

    public void OnMove(InputAction.CallbackContext context)
    {
        Player?.OnMove(context);
    }
    
    public void OnInteract(InputAction.CallbackContext context)
    {
        Player?.OnInteract(context);
    }

    public void OnAbort(InputAction.CallbackContext context)
    {
        Player?.OnAbort(context);
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        Player?.OnShoot(context);
    }

    public void OnEdit(InputAction.CallbackContext context)
    {
        Player?.OnEdit(context);
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        Player?.OnPause(context);
    }
}
