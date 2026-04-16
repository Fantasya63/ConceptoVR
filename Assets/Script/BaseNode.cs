using Concepto;
using UnityEngine;

public abstract class BaseNode<T> : MonoBehaviour
{
    [Header("References")]
    public SpatialPointer NextPointer;
    [SerializeField] protected T m_Data;
    
    public abstract T Data
    {
        get; set;
    }

    protected enum NodeDataType
    {
        String,
        HashmapNode,
        None
    }

    [SerializeField] protected NodeDataType m_NodeDataType;
}
