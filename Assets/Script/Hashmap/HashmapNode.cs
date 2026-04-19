using NUnit.Framework.Constraints;
using System;
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
    [SerializeField] float m_PaperShrinkDur = 0.5f;
    [SerializeField] float m_ReplaceAndInsertWait = 1.0f;

    [SerializeField] GameObject m_KeyValueIndicator;

    [Header("Equality Check Options")]
    [SerializeField] Color m_NormalColor = new Color(1.0f, 1.0f, 1.0f, 0.0f);
    [SerializeField] Color m_ErrorColor = Color.red;
    [SerializeField] Color m_SuccessColor = Color.green;
    [SerializeField] float m_FlashDur = 0.2f;
    [SerializeField] int m_NumOfFlash = 3;




    float m_TotalFlashDur = 0.0f;
    Coroutine m_KeyErrCoroutine = null;
    Coroutine m_OtherKeyCoroutine = null;

    public void Start()
    {
        m_TotalFlashDur = m_FlashDur * m_NumOfFlash * 2.0f;

        Debug.Assert(m_KeyValueIndicator != null);
        m_KeyValueIndicator.SetActive(false);
    }

    public override HashmapNodeData Data {
        get => m_Data;
        set => m_Data = value; 
    }


    public IEnumerator Close()
    {
        m_Controller.Close();
        yield return new WaitForSeconds(m_BoxOpenDur);
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

        m_KeyValueIndicator.SetActive(true);
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

    void StartErrRoutine(Coroutine coroutine, Outline outline)
    {

        if (coroutine != null)
            StopCoroutine(coroutine);

        coroutine = StartCoroutine(OnErrorRoutine(outline));
    }

    IEnumerator OnErrorRoutine(Outline outline)
    {
        outline.enabled = true;
        LeanTween.cancel(outline.gameObject);



        for (int i = 0; i < m_NumOfFlash; i++)
        {
            // Flash to error color
            LeanTween.value(outline.gameObject, m_NormalColor, m_ErrorColor, m_FlashDur)
                .setOnUpdate((Color c) =>
                {
                    outline.OutlineColor = c;
                });

            yield return new WaitForSeconds(m_FlashDur);

            // Flash back to normal color
            LeanTween.value(outline.gameObject, m_ErrorColor, m_NormalColor, m_FlashDur)
                .setOnUpdate((Color c) =>
                {
                    outline.OutlineColor = c;
                });

            yield return new WaitForSeconds(m_FlashDur);
        }

        outline.enabled = false;
    }

    void StartSuccessRoutine(Coroutine coroutine, Outline outline)
    {

        if (coroutine != null)
            StopCoroutine(coroutine);

        coroutine = StartCoroutine(OnSuccessRoutine(outline));
    }

    IEnumerator OnSuccessRoutine(Outline outline)
    {
        outline.enabled = true;
        LeanTween.cancel(outline.gameObject);



        for (int i = 0; i < m_NumOfFlash; i++)
        {
            // Flash to error color
            LeanTween.value(outline.gameObject, m_NormalColor, m_SuccessColor, m_FlashDur)
                .setOnUpdate((Color c) =>
                {
                    outline.OutlineColor = c;
                });

            yield return new WaitForSeconds(m_FlashDur);

            // Flash back to normal color
            LeanTween.value(outline.gameObject, m_SuccessColor, m_NormalColor, m_FlashDur)
                .setOnUpdate((Color c) =>
                {
                    outline.OutlineColor = c;
                });

            yield return new WaitForSeconds(m_FlashDur);
        }

        outline.enabled = false;
    }


    public IEnumerator AnimatedCheckIfEqual(Paper otherKey, Action<bool> onFinish)
    {
        Outline keyOutline;
        if (!m_Data.key.gameObject.TryGetComponent<Outline>(out keyOutline))
        {
            keyOutline = m_Data.key.gameObject.AddComponent<Outline>();
        }

        Outline otherOutline;
        if (!otherKey.gameObject.TryGetComponent<Outline>(out otherOutline))
        {
            otherOutline = otherKey.gameObject.AddComponent<Outline>();
        }


        otherKey.transform.rotation = m_NodeValuesQuerryTransform.rotation;

        bool isEqual = false;

        // Open Box
        {
            m_Controller.Open();
            yield return new WaitForSeconds(m_BoxOpenDur);
        }

        // Move to Querry Pos
        {
            Vector3 keyPos = m_NodeValuesQuerryTransform.position + m_KeyValueSeperation;
            Vector3 otherKeyPos = m_NodeValuesQuerryTransform.position - m_KeyValueSeperation;


            m_Data.key.transform.LeanMove(keyPos, m_ToInsertPosDur);
            otherKey.transform.LeanMove(otherKeyPos, m_ToInsertPosDur);
            yield return new WaitForSeconds(m_ToInsertPosDur);
        }

        // Check if they are equal
        {
            isEqual = m_Data.key.data == otherKey.data;
            if (!isEqual)
            {
                StartErrRoutine(m_KeyErrCoroutine, keyOutline);
                StartErrRoutine(m_OtherKeyCoroutine, otherOutline);
            }
            else
            {
                StartSuccessRoutine(m_KeyErrCoroutine, keyOutline);
                StartSuccessRoutine(m_OtherKeyCoroutine, otherOutline);
            }
            yield return new WaitForSeconds(m_TotalFlashDur);

        }
        
        m_Data.key.transform.position = m_BoxInsertTransform.position + m_KeyValueSeperation;


        onFinish.Invoke(isEqual);
    }

    public IEnumerator AnimatedReplaceValue(Paper newKey, Paper newValue)
    {
        Debug.Assert(newValue != null);
        Debug.Assert(m_Data.value != null);

        newKey.gameObject.SetActive(false);
        Vector3 valuePos = m_NodeValuesQuerryTransform.position - m_KeyValueSeperation;

        // Move to Querry Pos
        {
            m_Data.value.transform.LeanMove(valuePos, m_ToInsertPosDur);
            yield return new WaitForSeconds(m_ToInsertPosDur);
        }

        // Hide Old Value
        m_Data.value.gameObject.SetActive(false);
        
        yield return new WaitForSeconds(m_ReplaceAndInsertWait);

        // Insert Value
        {
            newValue.transform.position = valuePos;
            newValue.transform.rotation = m_NodeValuesQuerryTransform.rotation;

            Vector3 insertedPos = m_BoxInsertTransform.position - m_KeyValueSeperation;
            newValue.transform.LeanMove(insertedPos, m_ToInsertPosDur);
            yield return new WaitForSeconds(m_ToInsertPosDur);
        }

        // Update internal values
        {
            Destroy(m_Data.value.gameObject);
            m_Data.value = newValue;
        }

        m_Controller.Close();
        yield return new WaitForSeconds(m_BoxOpenDur);
    }
}
