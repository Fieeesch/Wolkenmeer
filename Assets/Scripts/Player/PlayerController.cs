using Player;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IInputReceiver
{
    private Rigidbody _rb;
    [SerializeField] private InteractionController interactionController;
    
    private float _movementX;
    private float _movementY;
    
    public float speed = 5f;
    public float rotationSpeed = 10f;

    private Quaternion _rotation;

    private int _layerMask;

    public int playerIndex = -1;
    
    private string _gridIdentifier;

    private GameObject _grid;

    private Material _gridMaterial;

    private bool _showGrid;

    private GameObject _carriedBlock;
    public PlayerAnimationController playerAnimationController;

    public GameObject RightHand;
    private GameObject SmallCarriedBlock;

    private void Start()
    {
        var playerCount = GameObject.FindGameObjectsWithTag("Player").Length;
        _gridIdentifier = "_Pos" + playerCount;
        _grid = GameObject.Find("TileGrid");
        playerAnimationController = GetComponentInChildren<PlayerAnimationController>();

        _rb = GetComponent<Rigidbody>();
        _layerMask = LayerMask.GetMask("Tile");
        _gridMaterial = _grid.GetComponent<Renderer>().material;
    }

    public void Spawn(PlayerConfiguration pc)
    {
        playerIndex = pc.PlayerIndex;
        var spawnPosition = transform.position + Vector3.down * 1.2f;
        Instantiate(pc.CharacterPrefab, spawnPosition, Quaternion.identity, transform);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        var movementVector = context.ReadValue<Vector2>();
        _movementX = movementVector.x;
        _movementY = movementVector.y;
        if(context.performed)
        {
            playerAnimationController.SetAnimationState(PlayerAnimationController.States.Move);
        }
        else if(context.canceled)
        {
            playerAnimationController.SetAnimationState(PlayerAnimationController.States.Idle);
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            playerAnimationController.SetAnimationState(PlayerAnimationController.States.Interact);
            interactionController.Interact(context.interaction);
        }
    }

    public void OnAbort(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            interactionController.Abort(context.interaction);
        }
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            playerAnimationController.SetAnimationState(PlayerAnimationController.States.Interact);
            interactionController.Shoot(context.interaction);
        }
    }

    public void OnEdit(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            playerAnimationController.SetAnimationState(PlayerAnimationController.States.Pickup);
            interactionController.Edit(context.interaction);
        }
    }

    private void ShowCarriedItem(GameObject item)
    {
        GameObject copy = Instantiate(item);
        copy.SetActive(true);
        Collider[] colliders = copy.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders) { Destroy(col); }
        if(copy.TryGetComponent<Collider>(out Collider collider)) Destroy(collider);
        copy.transform.parent = RightHand.transform;
        copy.transform.localScale = new Vector3(2,2,2);
        copy.transform.localPosition = Vector3.zero;
        SmallCarriedBlock = copy;
    }

    private void RemoveItemCopy()
    {
        Destroy(SmallCarriedBlock);
    }

    private void FixedUpdate()
    {
        var input = new Vector3(_movementX, 0, _movementY);
        
        var currentYVelocity = _rb.linearVelocity.y;

        if (input.magnitude > 0.1f)
        {
            var movement = Camera.main.transform.TransformDirection(input);
            movement.y = 0;
            movement.Normalize();
            
            var adjustedSpeed = speed * input.magnitude;
            
            var newVelocity = movement * adjustedSpeed;
            newVelocity.y = currentYVelocity;
            _rb.linearVelocity = newVelocity;
            
            var targetRotation = Quaternion.LookRotation(movement);
            _rb.rotation = Quaternion.Slerp(_rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
        else
        {
            // Nur horizontale Geschwindigkeit auf null setzen, Vertikalgeschwindigkeit beibehalten
            var newVelocity = new Vector3(0, currentYVelocity, 0);
            _rb.linearVelocity = newVelocity;
        }

        // Grid-Anzeige aktualisieren (falls erforderlich)
        if (!_showGrid) return;
        if (Physics.Raycast(GetFloorRay(), out var hit, 10, _layerMask))
        {
            _gridMaterial.SetVector(_gridIdentifier, hit.point);
        }
        else
        {
            var pos = new Vector3(0, -9999, 0);
            _gridMaterial.SetVector(_gridIdentifier, pos);
        }
    }

    public GameObject GetCarriedBlock()
    {
        return _carriedBlock;
    }

    public void SetCarriedBlock(GameObject carriedBlock)
    {
        SetCarriedBlock(carriedBlock,false);
    }
    
    public void SetCarriedBlock(GameObject carriedBlock, bool heavy)
    {
        if (carriedBlock)
        {
            ShowCarriedItem(carriedBlock);
            if (heavy)
            {
                speed /= 2;
            }
        } 
        else
        {
            RemoveItemCopy();
            if (heavy)
            {
                speed *= 2;
            }
        }
        
        this._carriedBlock = carriedBlock;
    }

    public void SetShowGrid(bool show)
    {
        _showGrid = show;
        
        if (_showGrid) return;
        var pos = new Vector3(0, -9999, 0);
        _gridMaterial.SetVector(_gridIdentifier, pos);
    }

    private Ray GetFloorRay()
    {
        var floor = transform.forward * 2.5f;
        floor.y = transform.position.y * -1.5f;
        var ray = new Ray(transform.position, floor);
        //Debug.DrawRay(transform.position, floor, Color.cyan);
        return ray;
    }
}