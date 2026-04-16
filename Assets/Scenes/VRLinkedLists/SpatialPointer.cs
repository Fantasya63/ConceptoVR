using System.Collections;
using TMPro;
using UnityEngine;

namespace Concepto
{
    public class SpatialPointer : BasePointer<SpatialNode>
    {
        public SpatialPointer(SpatialNode data)
        {
            transform.rotation = Quaternion.Euler(m_RotAngle);
            SetData(data);
        }

        protected override void SetData(SpatialNode data)
        {
            m_Node = data;
            if (m_DataLabel == null)
                return;

            if (data == null)
                m_DataLabel.text = $"Next: Null";
            else
                m_DataLabel.text = $"Next: {m_Node.Data}";
        }
    }

}

