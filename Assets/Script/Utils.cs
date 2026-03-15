using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Utils
{
    public static Paper GetInsertedPaper(XRSocketInteractor socket)
    {
        XRBaseInteractable insertedObject = (XRBaseInteractable)socket.firstInteractableSelected;
        return insertedObject.GetComponent<Paper>();
    }
}
