using UnityEngine;

[RequireComponent (typeof(BoxCollider))]
public class GrowShrinkController : MonoBehaviour
{
    [SerializeField] float m_TransitionDur = 1.0f;
    [SerializeField] Transform m_TargetTransform;
    
    private Vector3 m_StartScale;

    private void Start()
    {
        m_StartScale = m_TargetTransform.localScale;
        m_TargetTransform.localScale = Vector3.zero;
    }

    public void Grow()
    {
        if (LeanTween.isTweening(gameObject))
            LeanTween.cancel(gameObject);

        m_TargetTransform.localScale = Vector3.zero;
        m_TargetTransform.LeanScale(m_StartScale, m_TransitionDur);
    }

    public void Shrink()
    {
        if (LeanTween.isTweening(gameObject))
            LeanTween.cancel(gameObject);

        m_TargetTransform.LeanScale(Vector3.zero, m_TransitionDur);
    }

    private void OnTriggerEnter(Collider other)
    {
        Grow();
    }

    private void OnTriggerExit(Collider other)
    {
        Shrink();
    }
}
