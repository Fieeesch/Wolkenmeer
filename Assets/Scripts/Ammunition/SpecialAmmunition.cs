using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class SpecialAmmunition : MonoBehaviour, IMovementProperties
{
    [SerializeField] private AudioSource audioSource;

    public enum Specials
    {
        Standard,
        Spread,
        Piercing,
        Penetrating,
        Bomb
    }

    [SerializeField] public Specials SpecialType;

    public void Activate()
    {
        switch (SpecialType)
        {
            case Specials.Standard:
                break;
            case Specials.Spread:
                SpreadBehaviour behaviour = new SpreadBehaviour();
                behaviour.Spread(gameObject);
                break;
            case Specials.Piercing:
                break;
            case Specials.Penetrating:
                break;
            case Specials.Bomb:
                break;
            default:
                break;
        }
    }

    public void OnPickup() {
        switch(SpecialType) {
            case Specials.Bomb:
                AudioManager.Instance.PlayBOMB_PICK(audioSource);
                break;
            default:
                break;
        }
    }

    public void OnPlace() {
        switch(SpecialType) {
            case Specials.Bomb:
                AudioManager.Instance.PlayBOMB_PLACE(audioSource);
                break;
            default:
                break;
        }
    }
}
