using UnityEngine;

namespace Concepto
{
    public class SpatialNode : BaseNode<string, SpatialPointer, SpatialNode>
    {
        
        public override string Data
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



    }

}