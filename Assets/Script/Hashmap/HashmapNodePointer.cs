using Concepto;
using UnityEngine;

public class HashmapNodePointer : BasePointer<HashmapNode>
{
    
    protected override void SetData(HashmapNode data)
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
