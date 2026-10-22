using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public interface IMovementProperties {
    public void OnPickup();
    public void OnPlace();
}

public class TileController : MonoBehaviour, IInteractable
{
    private Renderer _renderer;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        _renderer.material = new Material(_renderer.material);
    }
    
    public bool Interact(GameObject interactor, InteractionType type, IInputInteraction context)
    {
        if (type != InteractionType.Carry)
        {
            return true;
        }
        return Carry(interactor, context);
    }

    /**
     * This method is responsible for carrying a block.
     * If the player is not carrying a block, the block will be picked up.
     * If the player is carrying a block, the block will be placed on the tile.
     */
    private bool Carry(GameObject interactor, IInputInteraction context)
    {
        if (!interactor.TryGetComponent(out PlayerController playerController))
        {
            return false;
        }
        
        if (playerController.GetCarriedBlock() == null)
        {
            if (transform.childCount == 0)
            {
                return false;
            }

            var block = transform.GetChild(0).gameObject;
            
            // Nur leere Blöcke dürfen bewegt werden
            if (block.TryGetComponent(out InteraktiverBlock interaktiverBlock))
            {
                
                if(context is not HoldInteraction)
                {
                    return false;
                }
                if (interaktiverBlock.GetState() != State.Ready)
                {
                    return false;
                }
                playerController.SetShowGrid(true);
            }

            if(block.TryGetComponent(out IMovementProperties props)) {
                props.OnPickup();
            }
            playerController.SetCarriedBlock(block,block.GetComponent<InteraktiverBlock>() != null);
            block.SetActive(false);
            block.transform.SetParent(null);
            return true;
        }
        //Wenn kein Block getragen wird und das Feld leer ist, wird der Block abgelegt.
        else 
        {
            if (transform.childCount != 0) 
            {
                return false;
            }

            var carriedBlock = playerController.GetCarriedBlock();
            
            carriedBlock.transform.SetParent(transform, true);

            var position = carriedBlock.transform.localPosition;
            position.x = 0;
            position.z = 0;
            carriedBlock.transform.localPosition = position;
            
            carriedBlock.SetActive(true);
            playerController.SetCarriedBlock(null,carriedBlock.GetComponent<InteraktiverBlock>() != null);
            playerController.SetShowGrid(false);

            if(carriedBlock.TryGetComponent(out IMovementProperties props)) {
                props.OnPlace();
            }
        }
        return true;
    }
}
