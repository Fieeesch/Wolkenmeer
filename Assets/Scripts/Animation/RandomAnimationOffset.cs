using UnityEngine;

public class RandomAnimationOffset : MonoBehaviour
{
    void Start()
    {
        if(TryGetComponent<Animator>(out Animator a))
        {
            AnimatorStateInfo state = a.GetCurrentAnimatorStateInfo(0);
            a.Play(state.fullPathHash, -1, Random.Range(0f, 1f));
        } 
    }
}
