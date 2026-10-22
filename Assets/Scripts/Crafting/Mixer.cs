using System;
using System.Collections.Generic;
using System.Linq;
using Effects;
using UnityEngine;

public class Mixer : InteraktiverBlock
{
    [SerializeField]
    GameObject[] bowls; // bowlAOn = [0], bowlAOff = [1], bowlBOn = [2], bowlBOff = [3]
    [SerializeField] GameObject DisplayFinished;
    [SerializeField] AudioSource audioSource;

    public GameObject basicSlime;
    public float processingTime;
    
    //private Renderer _renderer;
    private SlimeItem _inputSlimeA;
    private SlimeItem _inputSlimeB;
    private GameObject _outputSlime;
    private Material _inputMatA;
    private Material _inputMatB;

    protected override void Initialize()
    {
        ResetDisplay();
        if(bowls.Length != 4)
        {
            Debug.LogWarning("Array fuer Bowl-Anzeige vom Mixtisch fehlerhaft.");
        }
        //_renderer = GetComponent<Renderer>();
        //_renderer.material = new Material(_renderer.material);
    }

    protected override void OnReady()
    {
        //_renderer.material.SetColor("_BaseColor", Color.yellow);
    }

    protected override void OnProcessing()
    {
        //_renderer.material.SetColor("_BaseColor", Color.red);

        _outputSlime = Instantiate(basicSlime);
        _outputSlime.SetActive(false);
        if (!_outputSlime.TryGetComponent(out SlimeItem slime))
        {
            throw new Exception("SlimeItem component not found");
        }

        var sizeA = _inputSlimeA.effects.Count;
        var sizeB = _inputSlimeB.effects.Count;
        slime.effects = new List<AbstractEffect>(sizeA + sizeB);
        slime.effects.AddRange(_inputSlimeA.effects ?? Enumerable.Empty<AbstractEffect>());
        slime.effects.AddRange(_inputSlimeB.effects ?? Enumerable.Empty<AbstractEffect>());
        
        // Change Material
        if (!slime.transform.GetChild(0).TryGetComponent(out MixColorFromMat mixMat))
        {
            throw new Exception("MixColorFromMat not found");
        }

        mixMat.ChangeFirst(_inputMatA);
        mixMat.ChangeSecond(_inputMatB);

        if (DisplayFinished.TryGetComponent<SlimeMat>(out SlimeMat mat))
        {
            if(slime.transform.GetChild(0).TryGetComponent(out MeshRenderer mr))
            {
                mat.SetMat(mr.material);
            }
        }

        Destroy(_inputSlimeA);
        Destroy(_inputSlimeB);
        _inputSlimeA = null;
        _inputSlimeB = null;
        
        StartCoroutine(WaitAndSwitchToDone(processingTime));
    }

    protected override void OnDone()
    {
        AudioManager.Instance.PlayOFEN_DONE(audioSource);
        DisplayFinished.SetActive(true);
        ResetDisplay();
        //_renderer.material.SetColor("_BaseColor", Color.green);
    }

    private void ResetDisplay()
    {
        for (int i = 0; i < bowls.Length; i += 2)
        {
            bowls[i].SetActive(false);
        }
        for (int i = 1; i < bowls.Length; i += 2)
        {
            bowls[i].SetActive(true);
        }
    }

    protected override bool OnInteraction(GameObject interactor)
    {
        switch (GetState())
        {
            case State.Ready:
                if (TryGetSlimeToMix(interactor))
                {
                    SetState(State.Processing);
                }
                break;
            case State.Processing:
                return false;
            case State.Done:
                if (TrySetMixedInInventory(interactor))
                {
                    SetState(State.Ready);
                }
                break;
        }
        return true;
    }

    private bool TryGetSlimeToMix(GameObject interactor)
    {
        if (!interactor.TryGetComponent(out PlayerController controller))
        {
            return false;
        }

        if (controller.GetCarriedBlock() == null)
        {
            return false;
        }

        if (!controller.GetCarriedBlock().CompareTag("Slime"))
        {
            return false;
        }

        if (!controller.GetCarriedBlock().TryGetComponent(out SlimeItem slime))
        {
            throw new Exception("Slime Item is missing SlimeItem-Script");
        }

        if (!controller.GetCarriedBlock().TryGetComponent(out SlimeMat slimeMat))
        {
            throw new Exception("Slime Item is missing SlimeMat-Script");
        }

        if (slime.preventMix)
        {
            return false;
        }

        if (_inputSlimeA is null)
        {
            _inputMatA = slimeMat.GetMat();
            _inputSlimeA = slime;
            controller.SetCarriedBlock(null);
            //_renderer.material.SetColor("_BaseColor", new Color(255 / 255f, 165 / 255f, 0 / 255f));
            bowls[1].SetActive(false);
            bowls[0].SetActive(true);
            if (bowls[0].TryGetComponent<SlimeMat>(out SlimeMat mat))
            {
                mat.SetMat(_inputMatA);
            }
            return false;
        }

        if (_inputSlimeB is null)
        {
            _inputMatB = slimeMat.GetMat();
            _inputSlimeB = slime;
            controller.SetCarriedBlock(null);
            bowls[3].SetActive(false);
            bowls[2].SetActive(true);
            if (bowls[2].TryGetComponent<SlimeMat>(out SlimeMat mat))
            {
                mat.SetMat(_inputMatB);
            }
            return true;
        }
        
        Debug.LogWarning("Both slime slots are filled - forcing processing state");
        return true;
    }

    private bool TrySetMixedInInventory(GameObject interactor)
    {
        if (!interactor.TryGetComponent(out PlayerController controller))
        {
            return false;
        }

        if (controller.GetCarriedBlock() != null)
        {
            return false;
        }

        DisplayFinished.SetActive(false);
        controller.SetCarriedBlock(_outputSlime);
        _outputSlime = null;
        return true;
    }
}
