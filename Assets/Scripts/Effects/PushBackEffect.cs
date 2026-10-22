using System.Collections;
using UnityEngine;

namespace Effects
{
    [CreateAssetMenu(menuName = "ProjectileEffects/PushBackEffect")]
    public class PushBackEffect : AbstractEffect
    {
        public float pushForce;
        
        public override void ApplyEffect(GameObject target, Vector3 direction)
        {
            EnemyMovementBezier bezierMovement = target.GetComponent<EnemyMovementBezier>();
            if (bezierMovement == null)
            {
                return;
            }
            
            bezierMovement.KnockbackQuadratic(-pushForce,5);
        }
        
        public override void ScaleEffect(float scale)
        {
            pushForce *= scale;
        }
    }
}