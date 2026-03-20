using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class BoxKeyLock : MonoBehaviour
{
    [Header("Config")]
    public string hashkey = "0";
    public Animator boxAnimator;
    public string openParameter = "IsOpen";
    public XRSocketInteractor keySocket;

    [Header("Inside Socket")]
    public XRSocketInteractor insideSocket;

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

    void Start()
    {
        if (insideSocket != null)
            insideSocket.socketActive = false;
    }

    void OnKeyInserted(SelectEnterEventArgs args)
    {
        Paper paper = args.interactableObject.transform.GetComponent<Paper>();
        if (paper != null && paper.data == hashkey)
        {
            boxAnimator.SetBool(openParameter, true);
            if (insideSocket != null)
                insideSocket.socketActive = true;
        }
    }

    void OnKeyRemoved(SelectExitEventArgs args)
    {
        Paper paper = args.interactableObject.transform.GetComponent<Paper>();
        if (paper != null && paper.data == hashkey)
        {
            boxAnimator.SetBool(openParameter, false);
            if (insideSocket != null)
                insideSocket.socketActive = false;
        }
    }
}