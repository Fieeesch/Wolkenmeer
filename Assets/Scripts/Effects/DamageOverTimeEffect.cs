using UnityEngine;

namespace Effects
{
    [CreateAssetMenu(menuName = "ProjectileEffects/DamageOverTimeEffect")]
    public class DamageOverTimeEffect : AbstractDurationEffect, IDurationResetBehavior
    {
        public float damagePerSecond;

        protected override void SetUpEffect(){}
        protected override void UndoEffect(){}
        protected override void ActEffect()
        {
            Health.TakeDamage(damagePerSecond * Health.damageMultiplier);
        }

        public void ResetEffect(AbstractDurationEffect resetEffect)
        {
            duration = resetEffect.duration;
            damagePerSecond = ((DamageOverTimeEffect)resetEffect).damagePerSecond;
        }

        public override void ScaleEffect(float scale)
        {
            damagePerSecond *= scale;
        }
    }
}