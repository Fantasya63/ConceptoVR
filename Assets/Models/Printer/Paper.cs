using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(MeshRenderer))]
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
    private bool m_IsInteractive = true;

    public bool IsInteractive
    {
        get
        {
            return m_IsInteractive;
        }
    }

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

    
    public void RemoveInteractivity()
    {
        if (!m_IsInteractive)
            return;

        // Disable interaction while animating
        XRGrabInteractable grab = gameObject.GetComponent<XRGrabInteractable>();
        if (grab != null)
            Destroy(grab);

        // Disable physics
        Rigidbody paperRigidBody = gameObject.gameObject.GetComponent<Rigidbody>();
        if (paperRigidBody != null)
            Destroy(paperRigidBody);

        BoxCollider boxCollider = gameObject.gameObject.GetComponent<BoxCollider>();
        if (boxCollider != null)
            Destroy(boxCollider);

        m_IsInteractive = false;
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