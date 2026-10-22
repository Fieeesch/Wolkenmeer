using System;
using UnityEngine;

public class BombenTisch : InteraktiverBlock
{
    [SerializeField] private GameObject DisplayFinish;
    [SerializeField] private GameObject DisplayWorking;
    [SerializeField] private GameObject DisplayReady;
    //[SerializeField] private Animator Working;
    [SerializeField] private GameObject BasicBomb;
    [SerializeField] private float workingDuration;
    [SerializeField] private AudioSource audioSource;
    
    private GameObject output;
    private GameObject input;

    private Material _inputMat;
    private int _inputAmount;
    
    protected override void Initialize()
    {
        ResetDisplay();
        //Working.SetBool("Working", false);
    }

    private void ResetDisplay()
    {
        DisplayFinish.SetActive(true);
        DisplayWorking.SetActive(false);
        DisplayReady.SetActive(true);
    }

    protected override void OnReady()
    {
        DisplayFinish.SetActive(false);
    }
    
    protected override void OnProcessing()
    {
        DisplayReady.SetActive(false);
        DisplayWorking.SetActive(true);
        if(DisplayWorking.TryGetComponent<SlimeMat>(out SlimeMat mat))
        {
            mat.SetMat(_inputMat);
        }
        //Working.SetBool("Working", true);
        output = Instantiate(BasicBomb);
        RecolorBomb(output,_inputMat);
        RecolorBomb(DisplayFinish,_inputMat);
        if (output.TryGetComponent<BasicAmmunition>(out BasicAmmunition ba))
        {
            ba.effects = input.GetComponent<BasicAmmunition>().effects;
            ba.amount = _inputAmount;
        }
        output.SetActive(false);
        StartCoroutine(WaitAndSwitchToDone(workingDuration));
    }
    
    private void RecolorBomb(GameObject BombAsset, Material mat)
    {
        if (BombAsset.TryGetComponent(out SlimeMat slimeMat))
        {
            slimeMat.SetMat(_inputMat);
        }
    }
    
    protected override void OnDone()
    {
        AudioManager.Instance.PlayOFEN_DONE(audioSource);
        DisplayFinish.SetActive(true);
        //Working.SetBool("Working", false);
        DisplayWorking.SetActive(false);
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
                    AudioManager.Instance.PlayBOMB_PICK(audioSource);
                    ResetDisplay();
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

        float amount = ba.amount / 3;
        _inputAmount = (int) Math.Round(amount);

        if (controller.GetCarriedBlock().TryGetComponent<SpecialAmmunition>(out SpecialAmmunition sa))
        {
            return false;
        }

        if (!controller.GetCarriedBlock().TryGetComponent(out SlimeMat slimeMat))
        {
            return false;
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
