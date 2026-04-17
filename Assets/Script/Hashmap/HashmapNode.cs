using NUnit.Framework.Constraints;
using System.Collections;
using UnityEngine;

public class HashmapNodeData
{
    public Paper key;
    public Paper value;

    public HashmapNodeData(Paper _key, Paper _value)
    {
        key = _key;
        value = _value;
    }
}


public class HashmapNode : BaseNode<HashmapNodeData, HashmapNodePointer, HashmapNode>
{
    [Header("Hashmap Node Options")]
    [SerializeField] Transform m_NodeValuesQuerryTransform;
    [SerializeField] Transform m_BoxInsertTransform;
    [SerializeField] float m_ToQuerryPosDur = 1.0f;
    [SerializeField] float m_ToInsertPosDur = 0.75f;
    [SerializeField] Vector3 m_KeyValueSeperation = Vector3.forward * 0.1f;
    [SerializeField] float m_BoxOpenDur = 1.0f;
    [SerializeField] float m_InsertPauseDur = 0.5f;

    [SerializeField] GameObject m_KeyValueIndicator;

    public void Start()
    {
        Debug.Assert(m_KeyValueIndicator != null);
        m_KeyValueIndicator.SetActive(false);
    }

    public override HashmapNodeData Data {
        get => m_Data;
        set => m_Data = value; 
    }

    public IEnumerator SetDataAnimated(HashmapNodeData data)
    { 
        Debug.Assert(data != null);
        Debug.Assert(data.key != null);                                         
        Debug.Assert(data.value != null);
        if (m_Data != null)
        {
            if (m_Data.key != null)
            {
                Destroy(m_Data.key.gameObject);
                m_Data.key = null;
            }
            if (m_Data.value != null)
            {
                Destroy(m_Data.value.gameObject);
                m_Data.value = null;
            }
            m_Data = null;
        }

        // Open Box
        {
            m_KeyValueIndicator.SetActive(true);
            m_Controller.Open();
            yield return new WaitForSeconds(m_BoxOpenDur);
        }

        // Move to Querry Pos Dur
        {
            Vector3 keyPos = m_NodeValuesQuerryTransform.position + m_KeyValueSeperation;
            Vector3 valPos = m_NodeValuesQuerryTransform.position - m_KeyValueSeperation;

            data.key.transform.rotation = m_NodeValuesQuerryTransform.rotation;
            data.value.transform.rotation = m_NodeValuesQuerryTransform.rotation;

            data.key.transform.LeanMove(keyPos, m_ToQuerryPosDur);
            data.value.transform.LeanMove(valPos, m_ToQuerryPosDur);
            yield return new WaitForSeconds(m_ToQuerryPosDur);
        }

        yield return new WaitForSeconds(m_InsertPauseDur);

        // Move To Insert Pos
        {
            Vector3 keyPos = m_BoxInsertTransform.position + m_KeyValueSeperation;
            Vector3 valPos = m_BoxInsertTransform.position - m_KeyValueSeperation;

            data.key.transform.LeanMove(keyPos, m_ToInsertPosDur);
            data.value.transform.LeanMove(valPos, m_ToInsertPosDur);
            yield return new WaitForSeconds (m_ToInsertPosDur);
        }

        // Close Box
        {
            m_Controller.Close();
            yield return new WaitForSeconds(m_BoxOpenDur);
        }

        m_KeyValueIndicator.SetActive(false);
        m_Data = data;
    }
}
