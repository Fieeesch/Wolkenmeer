using System;
using UnityEngine;

namespace Effects
{
    [CreateAssetMenu(menuName = "ProjectileEffects/SlowDownEffect")]
    public class SlowDownEffect : AbstractDurationEffect, IDurationAppendBehavior
    {
        public float factor;
        
        protected override void ActEffect(){}
        
        protected override void SetUpEffect()
        {
            BezierMovement.speedMultiplier *= factor;
        }
        
        protected override void UndoEffect()
        {
            BezierMovement.speedMultiplier /= factor;
            Controller.RemoveEffect(this);
        }
        
        public void AppendEffect(AbstractDurationEffect appendedEffect)
        {
            SlowDownEffect appendedSlowDownEffect = (SlowDownEffect)appendedEffect;
            if (appendedSlowDownEffect.factor.Equals(factor))
            {
                duration += appendedSlowDownEffect.duration;
            }
            else
            {
                duration = appendedSlowDownEffect.duration;
                BezierMovement.speedMultiplier /= factor;
                BezierMovement.speedMultiplier *= appendedSlowDownEffect.factor;
                factor = appendedSlowDownEffect.factor;
            }
        }
        
        public override void ScaleEffect(float scale)
        {
            if (scale > 1)
            {
                factor /= scale;
            }
            else
            {
                factor = 1 - (1 - factor) * scale;
            }
        }
    }
}