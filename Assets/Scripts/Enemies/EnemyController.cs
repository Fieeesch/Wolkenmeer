using System.Collections;
using System.Collections.Generic;
using Effects;
using UnityEngine;

namespace Enemies
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private GameObject fireParticleEffect;
        [SerializeField] private GameObject honeyParticleEffect;
        [SerializeField] private GameObject poisonParticleEffect;
        [SerializeField] private GameObject windParticleEffect;
        
        [SerializeField] private EnemyMovementBezier movementBezier;
        
        private Dictionary<System.Type, AbstractDurationEffect> _activeEffects =
            new Dictionary<System.Type, AbstractDurationEffect>();

        /// <summary>
        /// Add the effect to the enemy
        /// </summary>
        /// <param name="effect">Effect</param>
        /// <param name="direction">Direction the Projectile comes from (deprecated)</param>
        public void ApplyEffect(AbstractEffect effect, Vector3 direction)
        {
            if (effect is AbstractDurationEffect durationEffect)
            {
                AbstractDurationEffect effectInstance = Instantiate(durationEffect);

                if (!_activeEffects.ContainsKey(effectInstance.GetType()))
                {
                    _activeEffects.Add(effectInstance.GetType(), effectInstance);
                    effectInstance.ApplyEffect(gameObject, direction);
                }
                else
                {
                    switch (_activeEffects[effectInstance.GetType()])
                    {
                        case IDurationResetBehavior activeResetEffect:
                            activeResetEffect.ResetEffect(effectInstance);
                            break;
                        case IDurationAppendBehavior activeApendEffect:
                            activeApendEffect.AppendEffect(effectInstance);
                            break;
                    }
                }
            }
            else
            {
                effect.ApplyEffect(gameObject, direction);
            }
            ActivateEffectGraphics(effect);
        }

        /// <summary>
        /// Remove the effect after its duration
        /// </summary>
        /// <param name="effect">Effect to remove</param>
        public void RemoveEffect(AbstractEffect effect)
        {
            _activeEffects.Remove(effect.GetType());
            DeactivateEffectGraphics(effect);
        }

        private void ActivateEffectGraphics(AbstractEffect effect)
        {
            if (effect is DamageOverTimeEffect)
            {
                fireParticleEffect.SetActive(true);
            }
            else if (effect is SlowDownEffect)
            {
                honeyParticleEffect.SetActive(true);
            }
            else if (effect is PoisonEffect)
            {
                poisonParticleEffect.SetActive(true);
            }
            else if (effect is PushBackEffect)
            {
                StartCoroutine(WindEffectGraphics());
            }
        }

        private void DeactivateEffectGraphics(AbstractEffect effect)
        {
            if (effect is DamageOverTimeEffect)
            {
                fireParticleEffect.SetActive(false);
            }
            else if (effect is SlowDownEffect)
            {
                honeyParticleEffect.SetActive(false);
            }
            else if (effect is PoisonEffect)
            {
                poisonParticleEffect.SetActive(false);
            }
        }
        
        private IEnumerator WindEffectGraphics()
        {
            windParticleEffect.SetActive(true);
            while (movementBezier.KnockBack)
            {
                yield return null;
            }
            windParticleEffect.SetActive(false);
        }
    }
}
