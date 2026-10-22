using System;
using UnityEngine;

public class TrashCan : InteraktiverBlock
{
    [SerializeField] private AudioSource audioSource;
    public float processingTime;
    
    protected override void Initialize()
    {
    }

    protected override void OnReady()
    {
    }

    protected override void OnProcessing()
    {
        AudioManager.Instance.PlayTRASH(audioSource);
        StartCoroutine(WaitAndSwitchToReady(processingTime));
    }
    
    protected override void OnDone()
    {
    }

    protected override bool OnInteraction(GameObject interactor)
    {
        switch (GetState())
        {
            case State.Ready:
                if (TryTakeItem(interactor))
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

    private bool TryTakeItem(GameObject interactor)
    {
        if (!interactor.TryGetComponent(out PlayerController controller))
        {
            return false;
        }

        if (controller.GetCarriedBlock().TryGetComponent<InteraktiverBlock>(out InteraktiverBlock block))
        {
            return false;
        }

        controller.SetCarriedBlock(null);
        return true;
    }
}
