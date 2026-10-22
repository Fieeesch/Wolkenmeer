using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class CanonController : MonoBehaviour, IInteractable, IInputReceiver
{
    [SerializeField] private AmmunitionStorage storage;

    public GameObject loadedProjectile;
    public GameObject canonModel;
    public GameObject canonPipe;

    private bool _soundCooldown;
    
    public float distanceBehind = 2;
    public float rotationSpeed = 20f;
    
    [FormerlySerializedAs("maxRotation")] public float maxRotationHorizontal = 50f;
    public float maxRotationVertical = 30f;
    
    private GameObject _interactor;
    private Gamepad _interactorGamepad;
    private Keyboard _interactorKeyboard;

    private float _rotationHorizontal;
    private float _rotationVertical;

    private GameObject _nextProjectile;
    
    public LineRenderer lineRenderer;
    //public Material noHitMaterial;
    //public Material hitMaterial;
    public LayerMask collisionLayer;
    
    public int linePoints = 100;
    public float lineInterval = 0.01f;
    
    public bool Interact(GameObject interactor, InteractionType type, IInputInteraction context)
    {
        if (type != InteractionType.Interact)
        {
            return true;
        }
        return InteractInteraction(interactor);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        var input = context.ReadValue<Vector2>();
        _rotationHorizontal = input.x;
        _rotationVertical = -input.y;
    }

    public void OnInteract(InputAction.CallbackContext context) { }

    public void OnAbort(InputAction.CallbackContext context)
    {
        var playerController = _interactor.GetComponent<PlayerController>();
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().BindPlayerToInput(playerController, playerController.playerIndex);
        _interactor = null;
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        var nextProjectile = storage?.GetFirst();
        if (nextProjectile == null)
        {
            if(!_soundCooldown && TryGetComponent<AudioSource>(out var src)) {
                AudioManager.Instance.PlayNOK(src);
                StartCoroutine(SetSoundCooldown(1f));
            }
            return;
        }
        
        nextProjectile.transform.position = canonPipe.transform.position;
        nextProjectile.transform.rotation = canonPipe.transform.rotation;

        Destroy(nextProjectile, 2.5f);
        var rigidBody = nextProjectile.transform.GetChild(0).GetComponent<Rigidbody>();

        if (rigidBody != null)
        {
            var direction = canonPipe.transform.forward;
            rigidBody.AddForce(direction * 100, ForceMode.Impulse);
        }

        if (nextProjectile.transform.GetChild(0).TryGetComponent<SpecialAmmunition>(out var sa))
        {
            sa.Activate();
        }
    }

    public void OnEdit(InputAction.CallbackContext context) { }

    private bool InteractInteraction(GameObject interactor)
    {
        if (_interactor != null && interactor != _interactor)
        {
            Debug.LogWarning("An interaction has been blocked due to a different interactor.");
            return false;
        }

        var playerController = interactor.GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("Missing Component: PlayerController");
            return false;
        }

        if (playerController.GetCarriedBlock() != null)
        {
            return false;
        }
        
        // Spielerposition wird nur bei erster Interaktion geändert
        if (_interactor == null)
        {
            // Spieler hinter die Kanone setzen
            var behindPosition = gameObject.transform.position - gameObject.transform.forward * distanceBehind;
            behindPosition.y = interactor.transform.position.y;
            interactor.transform.position = behindPosition;
            
            // Spieler Kanone ansehen lassen
            var lookAtPosition = new Vector3(gameObject.transform.position.x, interactor.transform.position.y, gameObject.transform.position.z);
            interactor.transform.LookAt(lookAtPosition);
        }
        
        // Mögliche Animation des Spielers werden abgebrochen
        playerController.playerAnimationController.SetAnimationState(PlayerAnimationController.States.Idle);
        
        // Input wird an Kanone übergeben
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().BindPlayerToInput(this, playerController.playerIndex);
        _interactor = interactor;
        return true;
    }

    private void AdaptToNextProjectile()
    {
        var nextProjectile = storage?.ShowFirst();
        if (nextProjectile is null)
        {
            return;
        }

        if (_nextProjectile is not null && nextProjectile.Equals(_nextProjectile))
        {
            return;
        }
        
        _nextProjectile = nextProjectile;
        
        // Debug.Log(nextProjectile);

        var meshRenderer = nextProjectile.GetComponentInChildren<MeshRenderer>();
        if (meshRenderer is not null && meshRenderer.material.HasProperty("_Color"))
        {
            lineRenderer.material.color = meshRenderer.material.color;
        }
    }

    private void Update()
    {
        if (_interactor is null)
        {
            lineRenderer.positionCount = 0;
            return;
        }
        
        AdaptToNextProjectile();
        DrawTrajectory();
        CheckTrajectoryCollision();
        lineRenderer.enabled = true;
    }

    private void CheckTrajectoryCollision()
    {
        var hasCollided = false;
        var color = lineRenderer.material.color;
        
        var pointCount = lineRenderer.positionCount;
        for (var i = 0; i < pointCount - 1; i++)
        {
            var start = lineRenderer.GetPosition(i);
            var end = lineRenderer.GetPosition(i + 1);

            if (!Physics.Raycast(start, end - start, out var hit, Vector3.Distance(start, end), collisionLayer))
            {
                continue;
            }

            if (hit.collider.gameObject.CompareTag("Enemy"))
            {
                color.a = 0.9f;
                lineRenderer.material.color = color;
                return;
            }
        }
        
        color.a = 0.3f;
        lineRenderer.material.color = color;
    }

    private void FixedUpdate()
    {
        if (_interactor is null)
        {
            return;
        }
        
        // Horizontale Rotation
        var rotationY = _rotationHorizontal * rotationSpeed * Time.fixedDeltaTime;
        var currentRotationY = canonModel.transform.eulerAngles.y;
        
        currentRotationY = (currentRotationY > 180) ? currentRotationY - 360 : currentRotationY;
        
        if (!((currentRotationY >= maxRotationHorizontal && rotationY > 0) 
              || (currentRotationY <= maxRotationHorizontal * -1 && rotationY < 0)))
        {
            canonModel.transform.Rotate(0, rotationY, 0);
        }
        
        
        // Vertikale Rotation
        var rotationX = _rotationVertical * rotationSpeed * Time.fixedDeltaTime;
        var currentRotationX = canonPipe.transform.localEulerAngles.x;
        
        currentRotationX = (currentRotationX > 180) ? currentRotationX - 360 : currentRotationX;
        
        if (!((currentRotationX >= (maxRotationVertical) && rotationX > 0) 
              || (currentRotationX <= maxRotationVertical * -1 && rotationX < 0)))
        {
            canonPipe.transform.localRotation = Quaternion.Euler(currentRotationX+rotationX,0, 0 );
        }
    }
    
    void DrawTrajectory()
    {
        lineRenderer.positionCount = linePoints;
        var points = new List<Vector3>();
        
        var currentPosition = canonPipe.transform.position;
        var velocity = canonPipe.transform.forward*100;
        
        for (int i = 0; i < linePoints; i++)
        {
            points.Add(currentPosition);
            currentPosition += velocity * lineInterval;
            velocity += Physics.gravity * lineInterval;
        }

        lineRenderer.SetPositions(points.ToArray());
    }

    private IEnumerator SetSoundCooldown(float seconds)
    {
        _soundCooldown = true;
        yield return new WaitForSeconds(seconds);
        _soundCooldown = false;
    }
}