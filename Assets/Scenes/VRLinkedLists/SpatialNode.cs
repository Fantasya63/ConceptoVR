using NUnit.Framework;
using UnityEngine;

namespace Concepto
{
    public class SpatialNode : MonoBehaviour
    {
        [Header("References")]
        public SpatialPointer m_NextPointer;
        public string Data
        {
            get
            {
                return m_Data;
            }
            set
            {
                if (m_Data != value)
                {
                    m_Data = value;
                    m_Controller.SetLabel(m_Data);
                }
            }
        }

        [SerializeField] private BoxScriptController m_Controller = null;
        [SerializeField] private string m_Data = "Null";

    }

}