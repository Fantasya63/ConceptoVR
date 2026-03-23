using UnityEngine;

namespace Concepto
{
    public class SpatialNode : MonoBehaviour
    {
        [Header("References")]
        public SpatialPointer m_NextPointer;
        public string m_Data = "Null";
    }

}