using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Utils
{
    public static Paper GetInsertedPaper(XRSocketInteractor socket)
    {
        XRBaseInteractable insertedObject = (XRBaseInteractable)socket.firstInteractableSelected;
        if (insertedObject != null) { 
            return insertedObject.GetComponent<Paper>();
        }
        return null;
    }

    public static Paper CopyPaper(Paper paper, Transform parent)
    {
        Paper newPaper = Object.Instantiate(paper, parent);
        newPaper.transform.position = paper.transform.position;
        newPaper.transform.rotation = paper.transform.rotation;
        newPaper.GetComponent<MeshRenderer>().material = paper.GetComponent<MeshRenderer>().material;
        newPaper.RemoveInteractivity();

        return newPaper;
    }
}
