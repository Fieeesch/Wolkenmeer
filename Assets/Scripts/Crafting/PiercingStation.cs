using System;
using System.Collections.Generic;
using System.Linq;
using Effects;
using UnityEngine;


public class PiercingStation : InteraktiverBlock
{
    [SerializeField] private GameObject DisplayFinish;
    [SerializeField] private GameObject DisplayWorking;
    //[SerializeField] private Animator PiercingAnimator;
    [SerializeField] private GameObject BasicPiercing;
    [SerializeField] private float workingDuration;

    private GameObject input;
    private GameObject output;
    private Material _inputMat;
    private int _inputAmount;

    protected override void Initialize()
    {
        //animation not yet implemented:
        //PiercingAnimator.SetBool("Working",false) 
    }

    protected override void OnReady()
    {
        DisplayFinish.SetActive(false);
    }

    protected override void OnProcessing()
    {
        //PiercingAnimator.SetBool("Working",true);
        output = Instantiate(BasicPiercing);
        Material mat = input.GetComponentInChildren<MeshRenderer>().sharedMaterial;
        RecolorPiercing(output, _inputMat);
        RecolorPiercing(DisplayFinish, _inputMat);
        if (output.TryGetComponent<BasicAmmunition>(out BasicAmmunition ba))
        {
            ba.effects = input.GetComponent<BasicAmmunition>().effects;
            ba.amount = _inputAmount;
        }
        output.SetActive(false);
        //DisplayWorking.transform.GetChild(0).GetComponent<MeshRenderer>().material = _inputMat;
        DisplayWorking.SetActive(true);
        StartCoroutine(WaitAndSwitchToDone(workingDuration));

    }

    private void RecolorPiercing1(GameObject PiercingAsset, Material mat)
    {
        Transform Ammunitions = PiercingAsset.transform.Find("multiple_piercing"); //exchange with mutiple_piercing
        foreach (Transform ammo in Ammunitions)
        {
            ammo.GetComponent<MeshRenderer>().material = mat;
        }
    }

    private void RecolorPiercing(GameObject PiercingAsset, Material mat)
    {
        if (!PiercingAsset.TryGetComponent(out SlimeMat slimeMat))
        {
            throw new Exception("SlimeMat component not found");
        }

        slimeMat.SetMat(_inputMat);
    }

    protected override void OnDone()
    {
        //PiercingAnimator.SetBool("Working",false);
        DisplayWorking.SetActive(false);
        DisplayFinish.SetActive(true);
    }

    protected override bool OnInteraction(GameObject interactor)
    {
        switch (GetState())
        {
            case State.Ready:
                if (TryGetBasicAmmunition(interactor))
                {
                    SetState(State.Processing);
                }
                break;
            case State.Processing:
                return false;
            case State.Done:
                if(TrySetItemOnPlayer(interactor))
                {
                    SetState(State.Ready);
                }
                break;
        }
        return true;
    }

    private bool TryGetBasicAmmunition(GameObject interactor)
    {
        if (!interactor.TryGetComponent(out PlayerController controller))
        {
            return false;
        }

        if(controller.GetCarriedBlock() == null)
        {
            return false;
        }

        if (!controller.GetCarriedBlock().TryGetComponent<BasicAmmunition>(out BasicAmmunition ba))
        {
            return false;
        }

        _inputAmount = ba.amount;

        if (controller.GetCarriedBlock().TryGetComponent<SpecialAmmunition>(out SpecialAmmunition sa))
        {
            return false;
        }
        if (!controller.GetCarriedBlock().TryGetComponent(out SlimeMat slimeMat))
        {
            throw new Exception("Slime Item is missing SlimeMat-Script");
        }
        
        _inputMat = slimeMat.GetMat();

        input = controller.GetCarriedBlock();
        controller.SetCarriedBlock(null);
        return true; 
    }


    private bool TrySetItemOnPlayer(GameObject interactor)
    {
        if (interactor.TryGetComponent(out PlayerController player))
        {
            if (player.GetCarriedBlock() is null)
            {
                DisplayFinish.SetActive(false);
                player.SetCarriedBlock(output);
                SetState(State.Ready);
                return true;
            }
        }

        return false;
    }
}