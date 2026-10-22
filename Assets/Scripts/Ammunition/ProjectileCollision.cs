using System;
using System.Collections.Generic;
using Effects;
using Enemies;
using UnityEngine;
using UnityEngine.Serialization;

public class ProjectileCollision : MonoBehaviour
{
    public float damage = 35f;
    public List<AbstractEffect> effects;
    
    public Boolean explode = false;
    public Boolean piercing = false;
    public float explosionRadius = 2.5f;
    public GameObject explosionAnimation;
    
    private Rigidbody _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Enemy"))
        {
            return;
        }

        if (explode)
        {
            Explode();
        }
        else
        {
            ApplyEffectsAndDamage(other.gameObject,damage);
        }
        
        if (piercing)
        {
            Destroy(gameObject, 5f);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Explode()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        
        GameObject explosion = Instantiate(explosionAnimation, transform.position, Quaternion.identity);
        explosion.transform.localScale = Vector3.one * explosionRadius;
        Destroy(explosion,10);
        
        foreach (var collider in hitColliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                ApplyEffectsAndDamage(collider.gameObject,damage);
            }
        }
    }
    
    private void ApplyEffectsAndDamage(GameObject other, float finalDamage)
    {
        var enemyController = other.gameObject.GetComponent<EnemyController>();
        var enemyHealth = other.gameObject.GetComponent<EnemyHealth>();

        if (enemyController == null || enemyHealth == null)
        {
            return;
        }
        
        Vector3 direction = _rb.linearVelocity.normalized;
        
        foreach (var effect in effects)
        {
            if (effect != null)
            {
                enemyController.ApplyEffect(effect, direction);
            }
        }
        
        enemyHealth.TakeDamage(finalDamage);
    }
}
