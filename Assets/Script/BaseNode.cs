using Concepto;
using UnityEngine;

// T is the Data Type
// U is the Pointer Type
public abstract class BaseNode<T, U, TNode> : MonoBehaviour
    where TNode : BaseNode<T, U, TNode>
    where U : BasePointer<TNode>
{
    [Header("References")]
    public U NextPointer;
    [SerializeField] protected T m_Data;
    
    public abstract T Data
    {
        get; set;
    }

    public void Move(Vector3 pos)
    {
        transform.position = pos;
        BaseNode<T, U, TNode> next = NextPointer.GetData();
        if (next != null)
        {
            next.Move(NextPointer.GetPointedPosition());
        }
    }

    public void LeanMove(Vector3 pos, float time)
    {
        LeanTween.cancel(transform.gameObject);
        BaseNode<T, U, TNode> next = NextPointer.GetData();

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
