using UnityEngine;

namespace Enemies
{
    public class EnemyMovement : MonoBehaviour
    {
        [SerializeField]
        private Transform target;

        [Range(0f, 5f)] public float speedMultiplier = 1f;
        public float speed = 1f;
    
        public Transform Target
        {
            get { return target; }
            set { target = value; }
        }

        void Update()
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * speedMultiplier * Time.deltaTime);
            
            Vector3 targetDirection = target.position - transform.position;
            float singleStep = speed * speedMultiplier * Time.deltaTime;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
            
            transform.rotation = Quaternion.LookRotation(newDirection);
            
        }
    }
}
