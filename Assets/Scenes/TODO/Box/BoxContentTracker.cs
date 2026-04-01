using UnityEngine;

public class BoxContentTracker : MonoBehaviour
{
    [Header("Config")]
    public Animator boxAnimator;
    public string openParameter = "IsOpen";

    private void OnTriggerEnter(Collider other)
    {
        if (!boxAnimator.GetBool(openParameter)) return;

        Debug.Log("Object placed inside box: " + other.gameObject.name);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!boxAnimator.GetBool(openParameter)) return;

        Debug.Log("Object removed from box: " + other.gameObject.name);
    }
}