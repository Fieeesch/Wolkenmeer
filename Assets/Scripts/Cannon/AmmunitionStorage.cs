using Effects;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using SysRandom = System.Random;

[System.Serializable]
public class Ammunition<gives, requires>
{
    public gives GivesType;
    public requires RequiredType;

    public bool SameObject(requires Required)
    {
        return Required.Equals(RequiredType) ? true : false;
    }

    public bool SameType(requires Required)
    {
        return Required.GetType().Equals(RequiredType.GetType()) ? true : false;
    }
}

public class AmmunitionStorage : MonoBehaviour, IInteractable
{
    [SerializeField] private Ammunition<Material, AbstractEffect>[] EffectMaterials;
    [SerializeField] private Ammunition<GameObject, SpecialAmmunition.Specials>[] SpecialsPrefabs;
    [SerializeField] private GameObject Basic;
    [SerializeField] private Material BasicMaterial;
    [SerializeField] private int maxStorage = 10;
    [SerializeField] private float scale = 0.35f;
    [SerializeField] private GameObject AmmunitionSpawner;
    [SerializeField] private AudioSource audioSource;
    private List<GameObject> Storage = new List<GameObject>();

    void Awake()
    {
        // Find the AmmunitionSpawner child object
        AmmunitionSpawner = transform.Find("ammunition_spawner").gameObject;
        AmmunitionSpawner.transform.parent = transform;
        if (AmmunitionSpawner == null)
        {
            Debug.LogError("ammunition_spawner child object of AmmunitionStorage not found!");
        }
    }

    public GameObject GetFirst()
    {
        if (Storage.Count < 1) return null;
        GameObject FirstProjectile = Storage[0];
        if (FirstProjectile)
        {
            Storage.RemoveAt(0);
            MakeReady(FirstProjectile);
            return FirstProjectile;
        }
        return null;
    }

    public GameObject ShowFirst()
    {
        if (Storage.Count < 1) return null;

        GameObject FirstProjectile = Storage[0];
        if (FirstProjectile)
        {
            return FirstProjectile;
        }
        return null;
    }

    private bool IsFull()
    {
        return Storage.Count >= maxStorage;
    }

    /// <summary>
    /// Loads Ammunition into the Storage Bowl
    /// </summary>
    /// <param name="effects">List of the Ammunition's elemental Effects</param>
    /// <param name="amount">Amount of Ammunition to be put into the Storage</param>
    /// <param name="special">Ammuntion Type</param>
    /// <returns>leftover amount that can't be put in storage</returns>
    public int Reload(List<AbstractEffect> effects, int amount, SpecialAmmunition.Specials special)
    {
        if (amount < 1 || IsFull()) return amount;
        GameObject ProjectilePrefab = CreateProjectile(effects, special);
        if (!ProjectilePrefab) return amount;
        AddToStorage(ProjectilePrefab);

        for (int i = 1; i < amount; i++)
        {
            if (!IsFull())
            {
                GameObject instance = Instantiate(ProjectilePrefab);

                ProjectileCollision collision = instance.GetComponent<ProjectileCollision>();
                if (collision != null)
                {
                    List<AbstractEffect> newEffects = new List<AbstractEffect>();
                    foreach (var effect in collision.effects)
                    {
                        newEffects.Add(Instantiate(effect));
                    }
                    collision.effects = newEffects;
                }

                AddToStorage(instance);
            }
            else if (IsFull())
            {
                return amount - i;
            }
        }
        return 0;
    }

    public void AddToStorage(GameObject ProjectilePrefab)
    {
        ShowProjectile(ProjectilePrefab);
        Storage.Add(ProjectilePrefab);
    }

    /// <summary>
    /// Readies the Projectile to be shot: sets Position, Rotation and Collider.
    /// </summary>
    /// <param name="futureProjectile">Projectile to be shot</param>
    private void MakeReady(GameObject FutureProjectile)
    {
        Transform child = FutureProjectile.transform.GetChild(0);
        child.localRotation = Quaternion.Euler(Vector3.zero);
        child.localPosition = Vector3.zero;
        if (child.TryGetComponent<Collider>(out Collider c))
        {
            c.isTrigger = true;
        }
        if (child.TryGetComponent<ProjectileCollision>(out ProjectileCollision pc))
        {
            pc.enabled = true;
        }
        FutureProjectile.transform.localScale = Vector3.one;
    }

    /// <summary>
    /// Chooses the Projectile for a given Effect and Ammunition Type
    /// </summary>
    /// <param name="effects">Effects of the Ammunition</param>
    /// <param name="special">Ammunition Type</param>
    /// <returns>Chosen Projectile</returns>
    private GameObject CreateProjectile(List<AbstractEffect> Effects, SpecialAmmunition.Specials special)
    {
        GameObject Projectile = Instantiate(ChosenAmmo(special));
        if (!Projectile) return null;
        Transform child = Projectile.transform.GetChild(0);
        if (!child) return null;

        if (Effects.Count == 1)
        {
            if (child.TryGetComponent<MeshRenderer>(out MeshRenderer mr))
            {
                mr.material = ChosenAmmo(Effects[0]);
            }
        }
        else if (Effects.Count > 1)
        {
            MixColorFromMat mixer = child.AddComponent<MixColorFromMat>();
            if (child.TryGetComponent<MeshRenderer>(out MeshRenderer mr))
            {
                mr.material = mixer.MixMat;
            }
            mixer.ChangeFirst(ChosenAmmo(Effects[0]));
            mixer.ChangeSecond(ChosenAmmo(Effects[1]));
        }

        if (child.TryGetComponent<Collider>(out Collider c))
        {
            c.isTrigger = false;
        }

        if (child.TryGetComponent<ProjectileCollision>(out ProjectileCollision pc))
        {
            pc.effects = Effects;
            pc.enabled = false;
        }

        return Projectile;
    }

    /// <summary>
    /// Chooses the Projectile Material for a given Ammunition Effect
    /// </summary>
    /// <param name="effect">Effect of the Ammunition (Fire, Honey, Poison, Wind)</param>
    /// <returns>Chosen Material</returns>
    private Material ChosenAmmo(AbstractEffect Effect)
    {
        foreach (var ammo in EffectMaterials)
        {
            if (ammo.SameType(Effect))
            {
                return ammo.GivesType;
            }
        }
        return BasicMaterial;
    }
    
    /// <summary>
    /// Chooses the Projectile Game Object for a given Ammunition Type
    /// </summary>
    /// <param name="specialType">Type of the Ammunition (Standard, Burst, Bomb, ...)</param>
    /// <returns>Chosen Prefab</returns>
    private GameObject ChosenAmmo(SpecialAmmunition.Specials SpecialType)
    {
        foreach (var ammo in SpecialsPrefabs)
        {
            if (ammo.SameObject(SpecialType))
            {
                return ammo.GivesType;
            }
        }
        return Basic;
    }

    /// <summary>
    /// Displays the Ammunition in the Bowl
    /// </summary>
    /// <param name="ammunition">Game Object of the Ammuntion to display</param>
    private void ShowProjectile(GameObject Ammunition)
    {
        Ammunition.transform.localScale = new Vector3(scale, scale, scale);
        Vector3 sPosition = AmmunitionSpawner.transform.position;
        float range = 0.2f;
        Vector3 SpawnPosition = new Vector3(sPosition.x + UnityEngine.Random.Range(-range, range), sPosition.y + UnityEngine.Random.Range(-range, range), sPosition.z + UnityEngine.Random.Range(-range, range));
        Ammunition.transform.position = SpawnPosition;
    }

    public bool Interact(GameObject interactor, InteractionType type, IInputInteraction context)
    {
        if (type == InteractionType.Interact)
        {
            return ReloadInteraction(interactor);
        }
        return true;
    }

    /// <summary>
    /// Handles the Interaction between Player and Ammunition Bowl. Loads Ammunition from the Player's Hand into the Bowl.
    /// </summary>
    /// <param name="interactor"></param>
    /// <returns>true</returns>
    private bool ReloadInteraction(GameObject interactor)
    {
        if (interactor.TryGetComponent<PlayerController>(out PlayerController player))
        {
            if (player.GetCarriedBlock() == null) return false;
            if (player.GetCarriedBlock().TryGetComponent<BasicAmmunition>(out BasicAmmunition ammo))
            {
                int leftover = ammo.amount;

                if (player.GetCarriedBlock().TryGetComponent<SpecialAmmunition>(out SpecialAmmunition special))
                {
                    switch(special.SpecialType) {
                        case SpecialAmmunition.Specials.Bomb:
                            AudioManager.Instance.PlayBOMB_PLACE(audioSource);
                            break;
                        default:
                            break;
                    }
                    leftover = Reload(ammo.effects, ammo.amount, special.SpecialType);
                }
                else
                {
                    leftover = Reload(ammo.effects, ammo.amount, SpecialAmmunition.Specials.Standard);
                }

                if (leftover == 0)
                {
                    player.SetCarriedBlock(null);
                }
                else
                {
                    ammo.amount = leftover;
                }
            }
        }
        return true;
    }
}