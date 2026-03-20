using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class HashkeySocketFilter : MonoBehaviour, IXRSelectFilter
{
    public string requiredHashkey = "0";

    public bool canProcess => isActiveAndEnabled;

    public bool Process(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
    {
        Paper paper = interactable.transform.GetComponent<Paper>();
        if (paper == null) return false;

        return paper.data == requiredHashkey;
    }
}