using UnityEngine;

public struct HashmapNodeData
{
    public Paper key;
    public Paper value;
}


public class HashmapNode : BaseNode<HashmapNodeData, HashmapNodePointer, HashmapNode>
{
    public override HashmapNodeData Data {
        get => throw new System.NotImplementedException();
        set => throw new System.NotImplementedException(); 
    }
}
