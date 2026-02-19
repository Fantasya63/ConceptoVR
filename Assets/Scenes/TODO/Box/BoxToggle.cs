using UnityEngine;

public class BoxToggle : MonoBehaviour
{
    private Animator animator;
    private bool isOpen = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void ToggleBox()
    {
        if (!isOpen)
        {
            animator.SetTrigger("Open");
            isOpen = true;
        }
        else
        {
            animator.SetTrigger("Close");
            isOpen = false;
        }
    }
}
