using System;
using UnityEngine;

namespace Effects
{
    [CreateAssetMenu(menuName = "ProjectileEffects/PoisonEffect")]
    public class PoisonEffect : AbstractDurationEffect, IDurationAppendBehavior
    {
        public float factor;
        
        protected override void ActEffect(){}
        
        protected override void SetUpEffect()
        {
            Health.damageMultiplier *= factor;
        }
        
        protected override void UndoEffect()
        {
            Health.damageMultiplier /= factor;
            Controller.RemoveEffect(this);
        }
        
        public void AppendEffect(AbstractDurationEffect appendedEffect)
        {
            PoisonEffect appendedPoisonEffect = (PoisonEffect)appendedEffect;
            if (appendedPoisonEffect.factor.Equals(factor))
            {
                duration += appendedPoisonEffect.duration;
            }
            else
            {
                duration = appendedPoisonEffect.duration;
                Health.damageMultiplier /= factor;
                Health.damageMultiplier *= appendedPoisonEffect.factor;
                factor = appendedPoisonEffect.factor;
            }
        }
        
        public override void ScaleEffect(float scale)
        {
            factor -= 1; 
            factor *= scale; 
            factor += 1;
        }
    }
}