using System.Collections;
using UnityEngine;

public class LinkedListsTraverseControls : LinkedListsControls
{
    [SerializeField] string m_MessageTemplate = "Output: {0}";
    [SerializeField] ErrorPanel messagePanel;

    Coroutine m_Coroutine = null;

    private void Start()
    {
        Debug.Assert(m_LinkedLists != null);
        Debug.Assert(messagePanel != null);
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
        bool success = false;
        string message = null;
        yield return m_LinkedLists.Traverse(-1, true, (bool _success, string _message) =>
        {
            success = _success;
            message = _message;
        });

        if (success)
        {
            if (message != null)
            {
                messagePanel.ShowError(string.Format(m_MessageTemplate, message));
            }
        }
        else
        {
            Error("Operation is unsuccessful.");
        }
        m_Coroutine = null;
    }
}
