using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class BoxKeyLock : MonoBehaviour
{
    [Header("Config")]
    public string index = "0";
    public Animator boxAnimator;
    public string openParameter = "IsOpen";
    public XRSocketInteractor keySocket;

    void OnEnable()
    {
        keySocket.selectEntered.AddListener(OnKeyInserted);
        keySocket.selectExited.AddListener(OnKeyRemoved);
    }

    void OnDisable()
    {
        keySocket.selectEntered.RemoveListener(OnKeyInserted);
        keySocket.selectExited.RemoveListener(OnKeyRemoved);
    }

    void OnKeyInserted(SelectEnterEventArgs args)
    {
        Paper paper = args.interactableObject.transform.GetComponent<Paper>();
        Debug.Log("Paper inserted! Paper: " + paper + " | data: " + (paper != null ? paper.data : "NULL"));
        if (paper != null && paper.data == index)
        {
            boxAnimator.SetBool(openParameter, true);
        }
    }

    void OnKeyRemoved(SelectExitEventArgs args)
    {
        Paper paper = args.interactableObject.transform.GetComponent<Paper>();
        Debug.Log("Paper removed! Paper: " + paper + " | data: " + (paper != null ? paper.data : "NULL"));
        if (paper != null && paper.data == index)
        {
            boxAnimator.SetBool(openParameter, false);
        }
    }
}