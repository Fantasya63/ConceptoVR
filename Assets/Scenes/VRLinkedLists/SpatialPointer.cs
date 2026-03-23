using System.Collections;
using TMPro;
using UnityEngine;

namespace Concepto
{
    public class SpatialPointer : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_Text m_DataLabel;

        [Header("Options")]
        [SerializeField] private Vector3 m_Offset = Vector3.up * 0.25f;
        [SerializeField] private Vector3 m_RotAngle = Vector3.zero;
        [SerializeField] private bool m_IsStationary = false;
        [SerializeField] private float m_AnimDuration = 1.0f;

        SpatialNode m_Data = null;
        
        void SetData(SpatialNode data)
        {
            m_Data = data;
            if (m_DataLabel == null)
                return;

            if (data == null)
                m_DataLabel.text = "Null";
            else
                m_DataLabel.text = m_Data.m_Data;
        }

        public SpatialPointer(SpatialNode data)
        {
            transform.rotation = Quaternion.Euler(m_RotAngle);
            SetData(data);
            
        }

        public void PointToNoAnim(SpatialNode node)
        {
            SetData(node);

            if (node == null) return;

            if (m_IsStationary)
            {
                node.transform.position = transform.position + m_Offset;
            }
            else
            {
                transform.position = node.transform.position + m_Offset;
            }
        }

        public IEnumerator PointTo(SpatialNode node)
        {
            SetData(node);

            if (node == null) yield break;

            if (m_IsStationary)
            {
                // Animate node
                Vector3 pos = transform.position + m_Offset;
                Vector3 fromPos = pos + Vector3.up * 0.5f;
                node.transform.position = fromPos;

                node.transform.LeanMove(pos, m_AnimDuration);
                Debug.Log("node move");

                // Wait for animation to finish
                yield return new WaitForSeconds(m_AnimDuration);
            }
            else
            {
                // Animate pointer
                transform.LeanMove(node.transform.position + m_Offset, m_AnimDuration);

                // Wait for animation to finish
                yield return new WaitForSeconds(m_AnimDuration);
            }
        }

        public SpatialNode GetData()
        {
            return m_Data;
        }
    }

}

