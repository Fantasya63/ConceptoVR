using Concepto;
using Concepto.HashMap;
using System.Collections;
using UnityEngine;

public class SpatialHashmap : MonoBehaviour
{
    [Header("Options")]
    [SerializeField] HashFuncDevice m_HashFuncDev;
    [SerializeField] SpatialLinkedLists m_LinkedListsPrefab;
    [SerializeField] int m_NumOfLists = 5;
    [SerializeField] Transform m_ArrStartTransform;
    [SerializeField] Vector3 m_ArrOffset;

    [Header("Anim Options")]
    [SerializeField] float m_GrowDur = 0.5f;

    //SpatialLinkedLists m_LinkedListsInstance;
    SpatialLinkedLists[] m_LinkedListsArr;

    private void Start()
    {
        Debug.Assert(m_HashFuncDev != null);
        Debug.Assert(m_LinkedListsPrefab != null);
        Debug.Assert(m_NumOfLists > 0);
        Debug.Assert(m_ArrStartTransform != null);
        Debug.Assert(m_ArrOffset.sqrMagnitude > 0);

        m_LinkedListsArr = new SpatialLinkedLists[m_NumOfLists];
    }

    
    public IEnumerator Init()
    {
        for (int i = 0; i < m_NumOfLists; i++)
        {
            Vector3 pos = m_ArrStartTransform.position + m_ArrOffset * i;
            SpatialLinkedLists instance = Instantiate(m_LinkedListsPrefab, pos, m_ArrStartTransform.rotation, transform);
            instance.transform.localScale = Vector3.zero;

            instance.transform.LeanScale(Vector3.one, m_GrowDur);
            yield return new WaitForSeconds(m_GrowDur);
        }
    }

    void Insert(Paper key, Paper value)
    {
        // Hash the key
    }

    void Retrieve(Paper key)
    {
        //
    }

    void Remove(Paper key)
    {
        //
    }
}
