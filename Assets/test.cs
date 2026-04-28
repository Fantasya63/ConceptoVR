using UnityEngine;

public class test : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"ENter{other.gameObject.name}");
        GrowShrinkController growShrink = other.GetComponent<GrowShrinkController>();
        if (growShrink != null )
            growShrink.Grow();
    }
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Exit");
        GrowShrinkController growShrink = other.GetComponent<GrowShrinkController>();
        if (growShrink != null)
            growShrink.Shrink();
    }

}
