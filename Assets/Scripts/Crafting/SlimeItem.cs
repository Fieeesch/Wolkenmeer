using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Effects;
using UnityEngine;
using UnityEngine.UI;

public class SlimeItem : MonoBehaviour, IItem
{
    public bool preventMix = false;
    public List<AbstractEffect> effects;
}