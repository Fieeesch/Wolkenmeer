using UnityEngine;
using UnityEngine.InputSystem;

public enum InteractionType
{
    Interact,
    Abort,
    Shoot,
    Carry
}

internal interface IInteractable
{
    public bool Interact(GameObject interactor, InteractionType type, IInputInteraction context);
}

public class InteractionController : MonoBehaviour
{
    public GameObject interactorSource;

    public AudioSource audioSource;

    public float interactRange;

    private InputSystem_Actions _inputActions;
    private IInteractable _lastInteractor;

    private int _layerMask;

    private void Awake()
    {
        _inputActions = new InputSystem_Actions();
    }

    private void Start()
    {
        _layerMask = LayerMask.GetMask("Tile");
    }

    public void Interact(IInputInteraction interaction)
    {
        // Ist das interagierbare Objekt in der Range vom Player?
        var ray = new Ray(interactorSource.transform.position, interactorSource.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction, Color.red);
        if (!Physics.Raycast(ray, out var hit, interactRange))
        {
            return;
        }

        if (!hit.collider.gameObject.TryGetComponent(out IInteractable interactable)) {Debug.LogWarning("Can't find interactable"); return;}
        _lastInteractor = interactable;
        bool result = interactable.Interact(interactorSource, InteractionType.Interact, interaction);
        if(!result) {
            AudioManager.Instance.PlayNOK(audioSource);
        }
    }

    public void Abort(IInputInteraction interaction)
    {
        // Verhindert einen Deadlock, wenn man nicht mehr in der Range des Objekts ist
        _lastInteractor?.Interact(interactorSource, InteractionType.Abort, interaction);
        _lastInteractor = null;

        // Ist das interagierbare Objekt in der Range vom Player?
        var ray = new Ray(interactorSource.transform.position, interactorSource.transform.forward);
        if (!Physics.Raycast(ray, out var hit, interactRange))
        {
            return;
        }

        if (hit.collider.gameObject.TryGetComponent(out IInteractable interactable))
        {
            interactable.Interact(interactorSource, InteractionType.Abort, interaction);
        }
    }

    public void Shoot(IInputInteraction interaction)
    {
        // Ist das interagierbare Objekt in der Range vom Player?
        var ray = new Ray(interactorSource.transform.position, interactorSource.transform.forward);
        if (!Physics.Raycast(ray, out var hit, interactRange))
        {
            return;
        }

        if (hit.collider.gameObject.TryGetComponent(out IInteractable interactable))
        {
            interactable.Interact(interactorSource, InteractionType.Shoot, interaction);
        }
    }

    public void Edit(IInputInteraction interaction)
    {
        if (!Physics.Raycast(GetFloorRay(), out var hit, 10, _layerMask))
        {
            // Debug.Log("no tile found");
            return;
        }

        if (hit.collider.gameObject.TryGetComponent(out IInteractable interactable))
        {
            interactable.Interact(interactorSource, InteractionType.Carry, interaction);
        }
    }

    private Ray GetFloorRay()
    {
        var floor = interactorSource.transform.forward * 2.5f;
        floor.y = interactorSource.transform.position.y * -1.5f;
        var ray = new Ray(interactorSource.transform.position, floor);
        Debug.DrawRay(interactorSource.transform.position, floor, Color.yellow);
        return ray;
    }
}