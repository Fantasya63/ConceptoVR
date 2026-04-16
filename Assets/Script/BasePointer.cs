using Concepto;
using System.Collections;
using TMPro;
using UnityEngine;

public abstract class BasePointer<T> : MonoBehaviour
    where T : MonoBehaviour
{
    protected T m_Data;


    [Header("References")]
    [SerializeField] protected TMP_Text m_DataLabel;
    [SerializeField] protected GameObject m_ArrowObject;

    [Header("Options")]
    [SerializeField] protected Vector3 m_Offset = Vector3.up * 0.25f;
    [SerializeField] protected Vector3 m_RotAngle = Vector3.zero;
    [SerializeField] protected bool m_IsStationary = false;
    [SerializeField] protected float m_AnimDuration = 1.0f;
    [SerializeField] protected LeanTweenType m_TweenType = LeanTweenType.easeInOutQuad;
    [SerializeField] protected bool ShowDataValOnLabel = false;


    public void SetLabel(string label)
    {
        m_DataLabel.text = label;
    }

    protected abstract void SetData(T data);

    public void PointToNoAnim(T node)
    {
        SetData(node);

        if (node == null) return;

        if (m_IsStationary)
        {
            node.transform.position = GetPointedPosition();
        }
        else
        {
            transform.position = node.transform.position + transform.rotation * m_Offset;
        }
    }

    public Vector3 GetPointedPosition()
    {
        return transform.position + transform.rotation * m_Offset;
    }

    public void PointToNoAnim(BasePointer<T> otherPointer)
    {
        if (m_IsStationary)
            return;

        if (otherPointer.m_Data == null)
        {
            transform.position = otherPointer.GetPointedPosition() + transform.rotation * m_Offset;
            SetData(otherPointer.m_Data);
        }
        else
        {
            PointToNoAnim(otherPointer.m_Data);
        }
    }



    public IEnumerator PointTo(BasePointer<T> otherPointer)
    {
        Debug.Assert(otherPointer != null);

        if (m_IsStationary)
            yield break;

        if (otherPointer.m_Data == null)
        {
            transform.LeanMove(otherPointer.GetPointedPosition() + transform.rotation * m_Offset, m_AnimDuration).setEase(m_TweenType);
            SetData(otherPointer.m_Data);
            yield return new WaitForSeconds(m_AnimDuration);
            yield break;
        }
        else
        {
            yield return PointTo(otherPointer.m_Data);
        }

    }

    public IEnumerator LookAt(T node)
    {
        SetData(node);

        if (node == null)
        {
            m_ArrowObject.transform.rotation = Quaternion.identity;
            yield break;
        }


        Quaternion start = m_ArrowObject.transform.rotation;
        m_ArrowObject.transform.LookAt(node.transform);
        Quaternion end = m_ArrowObject.transform.rotation;


        LeanTween.value(0.0f, 1.0f, m_AnimDuration)
            .setOnUpdate((float value) => {
                m_ArrowObject.transform.rotation = Quaternion.Slerp(start, end, value);

            });

        yield return new WaitForSeconds(m_AnimDuration);
    }

    public void LookAtNoAnim(T node)
    {
        m_ArrowObject.transform.LookAt(node.transform);
        SetData(node);
    }

    public IEnumerator PointTo(T node)
    {
        SetData(node);

        if (node == null) yield break;

        if (m_IsStationary)
        {
            // Animate node
            Vector3 pos = GetPointedPosition();
            Vector3 fromPos = pos + Vector3.up * 0.5f;
            node.transform.position = fromPos;

            node.transform.LeanMove(pos, m_AnimDuration).setEase(m_TweenType);
            Debug.Log("node move");

            // Wait for animation to finish
            yield return new WaitForSeconds(m_AnimDuration);
        }
        else
        {
            // Animate pointer
            transform.LeanMove(node.transform.position + transform.rotation * m_Offset, m_AnimDuration).setEase(m_TweenType);

            // Wait for animation to finish
            yield return new WaitForSeconds(m_AnimDuration);
        }
    }

    public T GetData()
    {
        return m_Data;
    }
}
