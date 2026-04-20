using Concepto;
using UnityEngine;

public abstract class HashmapControls : MonoBehaviour
{
    [SerializeField] protected AudioSource m_ErrorAudioSource;
    [SerializeField] protected ErrorPanel m_ErrorPanel;
    [SerializeField] protected SpatialHashmap m_Hashmap;


    protected void Error(string message = "LinkedListsInsert: Error")
    {
        m_ErrorPanel.ShowError(message);
        m_ErrorAudioSource.Play();
    }
}
