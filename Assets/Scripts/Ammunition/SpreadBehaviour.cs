using System.Collections.Generic;
using Effects;
using UnityEngine;

public class SpreadBehaviour
{
    public int rad = 5;
    public int bullets = 10;
    public int force = 100;
    public float destroyTime = 0.5f;

    public void Spread(GameObject gameObject)
    {
        ProjectileCollision projectile = gameObject.GetComponentInParent<ProjectileCollision>();
        
        for (int i = 0; i < bullets; i++)
        {
            Vector3 gPos = gameObject.transform.parent.position;
            Vector3 gRot = gameObject.transform.parent.rotation.eulerAngles;
            Vector3 rot = new Vector3(gRot.x + Random.Range(-rad, rad), gRot.y + Random.Range(-rad, rad), gRot.z + Random.Range(-rad, rad));
            
            GameObject pallet = Object.Instantiate(gameObject, gPos, Quaternion.Euler(rot), gameObject.transform.parent);
            
            if (pallet.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                Vector3 direction = pallet.transform.forward;
                rb.AddForce(direction * force, ForceMode.Impulse);
            }
            
            ProjectileCollision collision = pallet.GetComponent<ProjectileCollision>();
            if (collision != null)
            {
                List<AbstractEffect> newEffects = new List<AbstractEffect>();
                foreach (var effect in collision.effects)
                {
                    AbstractEffect newEffect = Object.Instantiate(effect);
                    newEffect.ScaleEffect(1f/bullets);
                    newEffects.Add(newEffect);
                }
                collision.effects = newEffects;
            }
            collision.damage /= bullets;
        }
        Object.Destroy(gameObject.transform.parent.gameObject, destroyTime);
    }
}