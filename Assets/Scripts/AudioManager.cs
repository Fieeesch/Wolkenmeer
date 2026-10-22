using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    
    public float volume = 1f;

    [Header("Sound Clips")]
    public AudioClip UI_CLICK;
    public AudioClip UI_MOVE;
    public AudioClip UI_BACK;
    public AudioClip UI_START;
    public AudioClip NOK;
    public AudioClip BOMB_PICK;
    public AudioClip BOMB_PLACE;
    public AudioClip OFEN_DONE;
    public AudioClip TRASH;
    public AudioClip SHOOT_BOMB;
    public AudioClip HIT_BASE;
    
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
                Debug.LogError("AudioManager is null!");

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance)
        {
            Debug.LogWarning("AudioManager already exists!");
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }

        DontDestroyOnLoad(this);
    }

    public void PlayUI_CLICK(AudioSource src) {
        src.PlayOneShot(UI_CLICK, volume);
    }

    public void PlayUI_MOVE(AudioSource src) {
        src.PlayOneShot(UI_MOVE, volume);
    }

    public void PlayUI_BACK(AudioSource src) {
        src.PlayOneShot(UI_BACK, volume);
    }

    public void PlayUI_START(AudioSource src) {
        src.PlayOneShot(UI_START, volume);
    }

    public void PlayNOK(AudioSource src) {
        src.PlayOneShot(NOK, volume);
    }

    public void PlayBOMB_PICK(AudioSource src) {
        src.PlayOneShot(BOMB_PICK, volume);
    }

    public void PlayBOMB_PLACE(AudioSource src) {
        src.PlayOneShot(BOMB_PLACE, volume);
    }

    public void PlayOFEN_DONE(AudioSource src) {
        src.PlayOneShot(OFEN_DONE, volume);
    }

    public void PlayTRASH(AudioSource src) {
        src.PlayOneShot(TRASH, volume);
    }

    public void PlaySHOOT_BOMB(AudioSource src) {
        src.PlayOneShot(SHOOT_BOMB, volume);
    }

    public void PlayHIT_BASE(AudioSource src) {
        src.PlayOneShot(HIT_BASE, volume);
    }
}
