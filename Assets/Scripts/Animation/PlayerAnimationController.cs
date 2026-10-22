using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private Animator Animator;

    public enum States
    {
        Idle,
        Move,
        Interact,
        Pickup
    }

    public void SetAnimationState(PlayerAnimationController.States state)
    {
        switch (state)
        {
            case States.Idle:
                Animator.SetInteger("State", 0);
                break;
            case States.Move:
                Animator.SetInteger("State", 1);
                break;
            case States.Interact:
                Animator.SetTrigger("Interact");
                break;
            case States.Pickup:
                Animator.SetTrigger("Pickup");
                break;
            default:
                break;
        }
    }
}
