using System.Collections.Generic;
using Effects;
using UnityEngine;

public class BasicAmmunition : MonoBehaviour, IItem
{
    public int amount;
    public List<AbstractEffect> effects;
    public GameObject block;
}
