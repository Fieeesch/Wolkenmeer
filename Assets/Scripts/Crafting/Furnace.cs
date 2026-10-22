using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;

public class Furnace : InteraktiverBlock
{
    public GameObject basicAmmunition;
    public float processingTime;

    private int _outputFactor;
    private Renderer _renderer;
    private SlimeItem _inputSlime;
    private Material _inputMat;
    private GameObject _outputAmmunition;
    private List<GameObject> _vfxChildren;
    [SerializeField] private GameObject DisplayWorking;
    [SerializeField] private Transform VFXParent;
    [SerializeField] private AudioSource audioSource;

    protected override void Initialize()
    {
        CooldownReductionOnPlayerCount();
        DisplayWorking.SetActive(false);
        CacheVFXChildren();
        foreach (var vfx in _vfxChildren)
        {
            vfx.SetActive(false);
        }
    }

    private void CooldownReductionOnPlayerCount()
    {
        GameObject GameManager = GameObject.FindGameObjectWithTag("GameManager");
        int players = GameManager.transform.childCount;
        switch (players)
        {
            case 4:
                processingTime = 1.2f;
                break;
            default:
                processingTime = 2;
                break;
        }
        // Debug.Log(players + " Spieler, daher CD = " + processingTime);
    }

    protected override void OnReady()
    {
        foreach (var vfx in _vfxChildren)
            {
                vfx.SetActive(false);
            }
        
    }

    protected override void OnProcessing()
    {
        DisplayWorking.SetActive(true);

        if (DisplayWorking.TryGetComponent<SlimeMat>(out SlimeMat sm))
        {
            sm.SetMat(_inputMat);
        }

        foreach (var vfx in _vfxChildren)
        {
            vfx.SetActive(true);
        }
        
        _outputAmmunition = Instantiate(basicAmmunition);
        _outputAmmunition.SetActive(false);
        if (!_outputAmmunition.TryGetComponent(out BasicAmmunition ammo))
        {
            throw new Exception("BasicAmmunition component not found");
        }
        if(!_outputAmmunition.TryGetComponent(out SlimeMat slimeMat))
        {
            throw new Exception("SlimeMat component not found");
        }

        ammo.effects = _inputSlime.effects;
        ammo.amount *= _outputFactor;

        // Change Material
        slimeMat.SetMat(_inputMat);

        Destroy(_inputSlime);

        StartCoroutine(WaitAndSwitchToDone(processingTime));
    }
    
    protected override void OnDone()
    {
        AudioManager.Instance.PlayOFEN_DONE(audioSource);
        foreach (var vfx in _vfxChildren)
        {
            vfx.SetActive(false);
        }

    }

    protected override bool OnInteraction(GameObject interactor)
    {
        switch (GetState())
        {
            case State.Ready:
                if (TryGetSlimeToBake(interactor))
                {
                    SetState(State.Processing);
                }
                break;
            case State.Processing:
                return false;
            case State.Done:
                if (TrySetSlimeInInventory(interactor))
                {
                    DisplayWorking.SetActive(false);
                    SetState(State.Ready);
                }
                break;
        }
        return true;
    }

    private bool TryGetSlimeToBake(GameObject interactor)
    {
        if (!interactor.TryGetComponent(out PlayerController controller))
        {
            return false;
        }

        if (controller.GetCarriedBlock() is null)
        {
            return false;
        }
        
        if (!controller.GetCarriedBlock().CompareTag("Slime"))
        {
            return false;
        }

        if(!controller.GetCarriedBlock().TryGetComponent(out SlimeItem slime))
        {
            throw new Exception("Slime Item is missing SlimeItem-Script");
        }

        if (slime.preventMix) {
            _outputFactor = 2;
        } else
        {
            _outputFactor = 1;
        }
       
        if (!controller.GetCarriedBlock().TryGetComponent(out SlimeMat slimeMat))
        {
            throw new Exception("Slime Item is missing SlimeMat-Script");
        }

        _inputMat = slimeMat.GetMat();
        _inputSlime = slime;
        controller.SetCarriedBlock(null);
        return true;
    }

    private bool TrySetSlimeInInventory(GameObject interactor)
    {
        if (!interactor.TryGetComponent(out PlayerController controller))
        {
            return false;
        }

        if (controller.GetCarriedBlock() != null)
        {
            return false;
        }
        
        controller.SetCarriedBlock(_outputAmmunition);
        _outputAmmunition = null;
        return true;
    }

    
    /// <summary>
    /// Caches the child GameObjects of the first child of the current transform that have names starting with "VFX_" into the _vfxChildren list.
    /// Logs a message if no VFX children are found.
    /// </summary>
    private void CacheVFXChildren()
    {
        _vfxChildren = new List<GameObject>();
        foreach (Transform child in VFXParent)
        {
            if (child.name.StartsWith("VFX_"))
            {
                _vfxChildren.Add(child.gameObject);
            }
        }
    }
}
