using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float delay = 0f;

    void Start()
    {
        Animator animator = GetComponent<Animator>();
        float animDuration = animator.GetCurrentAnimatorStateInfo(0).length;
        Destroy(gameObject, animDuration + delay);
    }
}
