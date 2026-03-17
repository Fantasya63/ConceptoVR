using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[CreateAssetMenu(fileName = "PaperInteractionLayers",
                 menuName = "XR/Paper Interaction Layers")]
public class PaperInteractionLayers : ScriptableObject
{
    public InteractionLayerMask dataLayerMask;
    public InteractionLayerMask hashkeyLayerMask;
}