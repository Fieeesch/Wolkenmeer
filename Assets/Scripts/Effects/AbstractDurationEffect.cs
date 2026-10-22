using System.Collections;
using Enemies;
using UnityEngine;

namespace Effects
{
    public abstract class AbstractDurationEffect : AbstractEffect
    {
        public float duration;
        protected EnemyController Controller;
        protected EnemyMovementBezier BezierMovement;
        protected EnemyHealth Health;

        public override void ApplyEffect(GameObject target, Vector3 direction)
        {
            Controller = target.GetComponent<EnemyController>();
            BezierMovement = target.GetComponent<EnemyMovementBezier>();
            Health = target.GetComponent<EnemyHealth>();
            SetUpEffect();
            Controller.StartCoroutine(EffectRunner());
        }
        
        private IEnumerator EffectRunner()
        {
            while (duration > 0)
            {
                ActEffect();
                yield return new WaitForSeconds(1f);
                duration--;
            }
            
            UndoEffect();
            Controller.RemoveEffect(this);
        }

        protected abstract void SetUpEffect();
        protected abstract void ActEffect();
        protected abstract void UndoEffect();
    }
}