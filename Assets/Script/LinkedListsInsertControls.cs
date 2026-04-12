using Concepto;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class LinkedListsInsertControls : MonoBehaviour
{
    [Header("References")]
    [SerializeField] AudioSource m_ErrorAudioSource;

    [SerializeField] XRSocketInteractor m_DataSocket;
    [SerializeField] XRSocketInteractor m_PositionSocket;

    [SerializeField] SpatialLinkedLists m_LinkedLists;

    Coroutine m_Coroutine;

    public void OnSubmit()
    {
        Paper dataPaper  = Utils.GetInsertedPaper(m_DataSocket);
        Paper positionPaper = Utils.GetInsertedPaper(m_PositionSocket);

        if (m_Coroutine != null)
        {
            Error();
            return;
        }

        if (dataPaper != null && positionPaper != null)
        {
            string dataValue = dataPaper.data;

            int position;
            if (int.TryParse(positionPaper.data, out position))
            {
                if (m_LinkedLists.CanInsert(dataValue, position))
                {
                    m_Coroutine = StartCoroutine(OnInsertRoutine(dataValue, position));
                }
            }
        }
    }

    void Error()
    {
        m_ErrorAudioSource.Play();
    }

    IEnumerator OnInsertRoutine(string value, int pos)
    {
        yield return m_LinkedLists.Insert(value, pos);
    }
}
