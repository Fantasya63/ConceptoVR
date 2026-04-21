using Concepto;
using UnityEngine;

public abstract class LinkedListsControls : MonoBehaviour
{
    [SerializeField] protected AudioSource m_ErrorAudioSource;
    [SerializeField] protected ErrorPanel m_ErrorPanel;
    [SerializeField] protected SpatialLinkedLists m_LinkedLists;


    protected void Error(string message = "Error", AudioSource audioSource = null)
    {
        m_ErrorPanel.ShowError(message);

        if (audioSource != null)
            audioSource.Play();
        else
            m_ErrorAudioSource.Play();
    }
}
