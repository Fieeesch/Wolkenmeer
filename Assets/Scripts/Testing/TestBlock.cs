using System.Collections;
using UnityEngine;

public class TestBlock : InteraktiverBlock
{
    private Renderer _renderer;
    
    protected override void Initialize()
    {
        _renderer = GetComponent<Renderer>();
        _renderer.material = new Material(_renderer.material);
    }

    protected override void OnReady()
    {
        _renderer.material.SetColor("_BaseColor", Color.yellow);
    }

    protected override void OnProcessing()
    {
        _renderer.material.SetColor("_BaseColor", Color.red);
        StartCoroutine(WaitAndSwitchToDone(4f));
    }

    protected override void OnDone()
    {
        _renderer.material.SetColor("_BaseColor", Color.green);
    }

    protected override bool OnInteraction(GameObject interactor)
    {
        switch (GetState())
        {
            case State.Ready:
                SetState(State.Processing);
                break;
            case State.Processing:
                return false;
            case State.Done:
                SetState(State.Ready);
                break;
        }
        return true;
    }
}
