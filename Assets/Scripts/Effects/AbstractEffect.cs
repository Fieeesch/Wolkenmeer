using UnityEngine;

namespace Effects
{
    public abstract class AbstractEffect : ScriptableObject
    {
        public abstract void ApplyEffect(GameObject target, Vector3 direction);
        public abstract void ScaleEffect(float scale);
    }
}