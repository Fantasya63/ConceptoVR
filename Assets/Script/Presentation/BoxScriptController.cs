using TMPro;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BoxScriptController : MonoBehaviour
{
    private Animator animator;

    public Animator GetAnimator() { return animator; }

    [SerializeField]
    private TMP_Text text;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Open()
    {
        animator.SetBool("IsOpen", true);
    }

    public void Close()
    {

        animator.SetBool("IsOpen", false);
    }


    public void SetLabel(string label)
    {
        text.text = label;
    }

}
