using Concepto;
using UnityEngine;

public abstract class LinkedListsControls : MonoBehaviour
{
    [SerializeField] protected AudioSource m_ErrorAudioSource;
    [SerializeField] protected ErrorPanel m_ErrorPanel;
    [SerializeField] protected SpatialLinkedLists m_LinkedLists;


    protected void Error(string message = "LinkedListsInsert: Error")
    {
        m_ErrorPanel.ShowError(message);
        m_ErrorAudioSource.Play();
    }
}
