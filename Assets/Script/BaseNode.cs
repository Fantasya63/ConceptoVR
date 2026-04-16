using Concepto;
using UnityEngine;

// T is the Data Type
// U is the Pointer Type
public abstract class BaseNode<TData, TPointer, TNode> : MonoBehaviour
    where TNode : BaseNode<TData, TPointer, TNode>
    where TPointer : BasePointer<TNode>
{
    [Header("References")]
    public TPointer NextPointer;
    [SerializeField] protected TData m_Data;
    
    public abstract TData Data
    {
        get; set;
    }

    public void Move(Vector3 pos)
    {
        transform.position = pos;
        BaseNode<TData, TPointer, TNode> next = NextPointer.GetData();
        if (next != null)
        {
            next.Move(NextPointer.GetPointedPosition());
        }
    }

    public void LeanMove(Vector3 pos, float time)
    {
        LeanTween.cancel(transform.gameObject);
        BaseNode<TData, TPointer, TNode> next = NextPointer.GetData();

        transform.LeanMove(pos, time)
            .setOnUpdate((float t) => {
                if (next != null)
                {
                    Debug.Log($"Moving node at Pos: {pos}, moving child at: {next.NextPointer.GetPointedPosition()}");
                    //next.LeanMove(next.m_NextPointer.GetPointedPosition(), time);
                    next.Move(NextPointer.GetPointedPosition());
                }
            });
    }

}
