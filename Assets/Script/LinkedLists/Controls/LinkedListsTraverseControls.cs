using Concepto;
using System.Collections;
using UnityEngine;

public class LinkedListsTraverseControls : LinkedListsControls
{
    Coroutine m_Coroutine = null;

    private void Start()
    {
        Debug.Assert(m_LinkedLists != null);

    }

    public void Traverse()
    {
        if (m_Coroutine != null || !m_LinkedLists.CanTraverse())
        {
            Error("Please wait till the operation is finished.");
            return;
        }
        m_Coroutine = StartCoroutine(OnTraverseRoutine());
    }

    IEnumerator OnTraverseRoutine()
    {
        yield return m_LinkedLists.Traverse();
        m_Coroutine = null;
    }
}
