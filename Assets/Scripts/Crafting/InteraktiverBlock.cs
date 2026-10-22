using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public enum State
{
    Ready, Processing, Done
}

public abstract class InteraktiverBlock : MonoBehaviour, IInteractable
{
    private State _state;

    protected void Start()
    {
        Initialize();
        SetState(State.Ready);
    }
    
    protected abstract void Initialize();

    public State GetState()
    {
        return _state;
    }

    protected void SetState(State state)
    {
        _state = state;
        switch (_state)
        {
            case State.Ready:
                OnReady();
                break;
            case State.Processing:
                OnProcessing();
                break;
            case State.Done:
                OnDone();
                break;
        }
    }

    protected abstract void OnReady();
    
    protected abstract void OnProcessing();
    
    protected abstract void OnDone();

    public bool Interact(GameObject interactor, InteractionType type, IInputInteraction context)
    {
        if (type != InteractionType.Interact)
        {
            return true;
        }
        return OnInteraction(interactor);
    }

    protected abstract bool OnInteraction(GameObject interactor);

    protected IEnumerator WaitAndSwitchToDone(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SetState(State.Done);
    }
    
    protected IEnumerator WaitAndSwitchToReady(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SetState(State.Ready);
    }
}
