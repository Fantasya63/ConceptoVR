using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(XRGrabInteractable))]
public class Paper : MonoBehaviour
{
    public enum PAPER_TYPE
    {
        Data = 0,
        Hashkey,
    };

    [Header("Config")]
    [SerializeField] private PaperInteractionLayers interactionConfig;

    [Header("Materials")]
    [SerializeField] private Material hashkeyMat;
    [SerializeField] private Material dataMat;

    public string data = "$$$";

    private MeshRenderer meshRenderer;
    private XRGrabInteractable interactable;
    private PAPER_TYPE paperType = PAPER_TYPE.Data;

    public PAPER_TYPE PaperType
    {
        get => paperType;
        set
        {
            if (paperType != value)
            {
                paperType = value;
                UpdateInteractionLayer();
                UpdateMaterial();
            }
        }
    }

    void Awake()
    {
        interactable = GetComponent<XRGrabInteractable>();
        meshRenderer = GetComponent<MeshRenderer>();

        UpdateInteractionLayer();
        UpdateMaterial();
    }

    private void UpdateInteractionLayer()
    {
        if (interactable == null || interactionConfig == null) return;

        switch (paperType)
        {
            case PAPER_TYPE.Data:
                interactable.interactionLayers = interactionConfig.dataLayerMask;
                break;

            case PAPER_TYPE.Hashkey:
                interactable.interactionLayers = interactionConfig.hashkeyLayerMask;
                break;
        }
    }

    private void UpdateMaterial()
    {
        if (meshRenderer == null) return;

        switch (paperType)
        {
            case PAPER_TYPE.Data:
                meshRenderer.material = dataMat;
                break;

            case PAPER_TYPE.Hashkey:
                meshRenderer.material = hashkeyMat;
                break;
        }
    }
}