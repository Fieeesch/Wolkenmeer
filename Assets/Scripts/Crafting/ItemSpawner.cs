using System;
using System.Collections;
using UnityEngine;

public class ItemSpawner : InteraktiverBlock
{
    private Renderer _renderer;

    public float refillDuration;
    public GameObject sourceItem;
    public Transform playerTransform;
    public Transform lidTransform;
    [SerializeField] private Animator LidAnimator;


    protected override void Initialize()
    {
        if (LidAnimator)
        {
            LidAnimator.SetBool("LidOpen", false);
        }
        _renderer = GetComponentInChildren<Renderer>();
        if (_renderer == null)
        {
            _renderer = GetComponent<Renderer>();
        }
        _renderer.material = new Material(_renderer.material);
    }

    protected override void OnReady()
    {
        
    }

    protected override void OnProcessing()
    {
        if (LidAnimator)
        {   
            openLid();
        }
        StartCoroutine(WaitAndSwitchToDone(refillDuration));
    }

    protected override void OnDone()
    {
        if (LidAnimator)
        {
            LidAnimator.SetBool("LidOpen", false);
        }
        SetState(State.Ready);
    }

    protected override bool OnInteraction(GameObject interactor)
    {
        switch (GetState())
        {
            case State.Ready:
                if(SetItemOnPlayer(interactor))
                {
                    SetState(State.Processing);
                }
                break;
            case State.Processing:
                return false;
            case State.Done:
                break;
        }
        return true;
    }

    private Boolean SetItemOnPlayer(GameObject interactor)
    {
        if(interactor.TryGetComponent(out PlayerController player))
        {
            if(player.GetCarriedBlock() is null)
            {
                AnimateIfCanonStack();
                GameObject newSlime = Instantiate(sourceItem);
                newSlime.SetActive(false);
                player.SetCarriedBlock(newSlime);
                return true;
            }
        }
        return false;
    }
    
    private void AnimateIfCanonStack()
    {
        if (TryGetComponent<AnimateCanonStack>(out AnimateCanonStack acs))
        {
            acs.RemoveFromStack();
        }
    }
    
    /// <summary>
    /// visually opens the lid of the slime_source.
    /// </summary>
    private void openLid()
    {
        Transform interactor = findNearestPlayer().transform;
        lidTransform = transform.Find("source_slime_lid");
        if (lidTransform == null || interactor == null)
        {
            Debug.LogWarning("animator or player not found");
        }
        Vector3 directionToPlayer = interactor.position - transform.position;
        float angle = Mathf.Atan2(directionToPlayer.z, directionToPlayer.x) * Mathf.Rad2Deg;
        int direction = GetDirection(angle);
        if (LidAnimator != null)
        {
            LidAnimator.SetInteger("openDirection", direction);
            LidAnimator.SetBool("LidOpen", true);
        }
        else
        {
            Debug.LogWarning("LidAnimator is null");
        }
    }

    /// <summary>
    /// Determines the direction based on the given angle.
    /// </summary>
    /// <param name="angle">The angle in degrees.</param>
    /// <returns>
    /// An integer representing the direction:
    /// 0 - West,
    /// 1 - South,
    /// 2 - East,
    /// 3 - North.
    /// </returns>
    private int GetDirection(float angle)
    {
        if (angle < 0) angle += 360;
        if (angle >= 45 && angle < 135) return 3;
        if (angle >= 135 && angle < 225) return 2;
        if (angle >= 225 && angle < 315) return 1;
        return 0;
    }

    /// <summary>
    /// Finds the nearest player to the slime_source.
    /// </summary>
    private GameObject findNearestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject nearestPlayer = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject p in players)
        {
            float distance = Vector3.Distance(transform.position, p.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestPlayer = p;
            }
        }

        return nearestPlayer;
    }
}
