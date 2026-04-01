using NUnit.Framework;
using System.Collections;
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

        public void Move(Vector3 pos)
        {
            transform.position = pos;
            SpatialNode next = m_NextPointer.GetData();
            if (next != null)
            {
                next.Move(pos);
            }
        }

        public void LeanMove(Vector3 pos, float time)
        {
            transform.LeanMove(pos, time);
            SpatialNode next = m_NextPointer.GetData();
            if (next != null)
            {
                next.LeanMove(next.m_NextPointer.GetPointedPosition(), time);
            }
        }


        [SerializeField] private BoxScriptController m_Controller = null;
        [SerializeField] private string m_Data = "Null";

    }

}