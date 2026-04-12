using Concepto;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class LinkedListsInsertControls : MonoBehaviour
{
    [Header("References")]
    [SerializeField] AudioSource m_ErrorAudioSource;

    [SerializeField] XRSocketInteractor m_DataSocket;
    [SerializeField] XRSocketInteractor m_PositionSocket;
    [SerializeField] SpatialLinkedLists m_LinkedLists;

    [Header("Error Options")]
    [SerializeField] Color m_NormalColor = Color.white;
    [SerializeField] Color m_ErrorColor = Color.red;
    [SerializeField] Color m_SuccessColor = Color.green;
    [SerializeField] float m_FlashDur = 0.2f;
    [SerializeField] int m_NumOfFlash = 3;
    [SerializeField] ErrorPanel m_ErrorPanel;


    private Outline m_DataOutline;
    private Outline m_PosOutline;
    Coroutine m_Coroutine;
    Coroutine m_DataErrCoroutine;
    Coroutine m_PosErrCoroutine;

    private void Start()
    {
        Debug.Assert(m_LinkedLists != null);
        Debug.Assert(m_DataSocket != null);
        Debug.Assert(m_PositionSocket != null);
        Debug.Assert(m_ErrorPanel != null);

        m_DataOutline = GetOrAddOutline(m_DataSocket.gameObject);
        m_PosOutline = GetOrAddOutline(m_PositionSocket.gameObject);

        m_DataOutline.OutlineColor = m_NormalColor;
        m_PosOutline.OutlineColor = m_NormalColor;
    }

    Outline GetOrAddOutline(GameObject go)
    {
        Outline outline = null;
        go.TryGetComponent<Outline>(out outline);
        if (!outline)
        {
            outline = go.AddComponent<Outline>();
            outline.OutlineWidth = 10.0f;
            outline.OutlineColor = m_NormalColor;
        }

        return outline;
    }


#if UNITY_EDITOR
    [SerializeField] bool useDebugValues = false;
#endif

    bool CheckPaperNotNull(Coroutine coroutine, Paper paper, Outline outline, string errMessage)
    {
        if (!paper)
        {
            StartErrRoutine(coroutine, outline, errMessage);
            return false;
        }

        return true;
    }

    void StartErrRoutine(Coroutine coroutine, Outline outline, string message) 
    {

        if (coroutine != null)
            StopCoroutine(coroutine);

            coroutine = StartCoroutine(OnErrorRoutine(outline, message));
    }

    public void OnSubmit()
    {
        if (m_Coroutine != null)
        {
            Error();
            return;
        }

#if UNITY_EDITOR
        if (useDebugValues)
        {
            Debug.Log("Starting Insert Routine using Debug Values");
            m_Coroutine = StartCoroutine(OnInsertRoutine("WWW", -1));
            return;
        }
#endif

        Paper dataPaper  = Utils.GetInsertedPaper(m_DataSocket);
        Paper positionPaper = Utils.GetInsertedPaper(m_PositionSocket);

        if (!CheckPaperNotNull(m_DataErrCoroutine, dataPaper, m_DataOutline, "Please enter a paper in the data socket"))
            return;

        if (!CheckPaperNotNull(m_PosErrCoroutine, positionPaper, m_PosOutline, "Please enter a paper in the position socket"))
            return;

        string dataValue = dataPaper.data;

        int position;
        if (int.TryParse(positionPaper.data, out position))
        {
            if (m_LinkedLists.CanInsert(dataValue, position))
            {
                m_Coroutine = StartCoroutine(OnInsertRoutine(dataValue, position));
            }
            else
            {
                StartErrRoutine(m_PosErrCoroutine, m_PosOutline, $"Please enter a position in the range 0 to {m_LinkedLists.Size}");
            }
        }
        else
        {
          
            StartErrRoutine(m_PosErrCoroutine, m_PosOutline, "Please enter a valid integer");
        }
    }

    void Error(string message = "LinkedListsInsert: Error")
    {
        m_ErrorPanel.ShowError(message);
        m_ErrorAudioSource.Play();
    }

    IEnumerator OnErrorRoutine(Outline outline, string message)
    {
        Error(message);
        LeanTween.cancel(outline.gameObject);

        int flashCount = 3;
        float duration = 0.2f;

        for (int i = 0; i < flashCount; i++)
        {
            // Flash to error color
            LeanTween.value(outline.gameObject, m_NormalColor, m_ErrorColor, duration)
                .setOnUpdate((Color c) =>
                {
                    outline.OutlineColor = c;
                });

            yield return new WaitForSeconds(duration);

            // Flash back to normal color
            LeanTween.value(outline.gameObject, m_ErrorColor, m_NormalColor, duration)
                .setOnUpdate((Color c) =>
                {
                    outline.OutlineColor = c;
                });

            yield return new WaitForSeconds(duration);
        }
    }

    IEnumerator OnInsertRoutine(string value, int pos)
    {
        yield return m_LinkedLists.Insert(value, pos);
        Debug.Log("Insert Control Routine Finished");

        m_Coroutine = null;
        yield break;
    }
}
