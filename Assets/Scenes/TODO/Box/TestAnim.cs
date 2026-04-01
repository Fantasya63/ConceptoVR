using UnityEngine;

public class TestAnim : MonoBehaviour
{
    public Animator animator;
    public Animation animationClip;

    public void TestAnimator()
    {
        animator.SetTrigger("Open");
    }
}
