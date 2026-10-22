using UnityEngine;
using UnityEngine.InputSystem;

public abstract class ItemBlock :  MonoBehaviour, IInteractable
{
    public bool Interact(GameObject interactor, InteractionType type, IInputInteraction context)
    {
        if (type != InteractionType.Carry)
        {
            return true;
        }
        //OnCarry(interactor);
        return true;
    }
    
    //public abstract void OnCarry(GameObject interactor);
}