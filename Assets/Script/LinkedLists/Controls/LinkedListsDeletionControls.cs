using Concepto;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class LinkedListsDeletionControls : LinkedListsControls
{
    [Header("References")]
    [SerializeField] XRSocketInteractor m_PositionSocket;

    [Header("Error Options")]
    [SerializeField] Color m_NormalColor = Color.white;
    [SerializeField] Color m_ErrorColor = Color.red;
    [SerializeField] Color m_SuccessColor = Color.green;
    [SerializeField] float m_FlashDur = 0.2f;
    [SerializeField] int m_NumOfFlash = 3;


    private Outline m_PosOutline;
    Coroutine m_Coroutine;
    Coroutine m_PosErrCoroutine;

    private void Start()
    {
        Debug.Assert(m_LinkedLists != null);
        Debug.Assert(m_PositionSocket != null);
        Debug.Assert(m_ErrorPanel != null);

        m_PosOutline = GetOrAddOutline(m_PositionSocket.gameObject);

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
        if (m_Coroutine != null || !m_LinkedLists.CanTraverse())
        {
            Error("Please wait till the operation is finished.");
            return;
        }

#if UNITY_EDITOR
        if (useDebugValues)
        {
            Debug.Log("Starting Insert Routine using Debug Values");
            m_Coroutine = StartCoroutine(OnDeleteRoutine(-1));
            return;
        }
#endif

        Paper positionPaper = Utils.GetInsertedPaper(m_PositionSocket);

        if (!CheckPaperNotNull(m_PosErrCoroutine, positionPaper, m_PosOutline, "Please enter a paper in the position socket"))
            return;


        int position;
        if (int.TryParse(positionPaper.data, out position))
        {
            if (m_LinkedLists.CanDelete(position))
            {
                m_Coroutine = StartCoroutine(OnDeleteRoutine(position));
            }
            else
            {
                StartErrRoutine(m_PosErrCoroutine, m_PosOutline, $"Please enter a position in the range 0 to {m_LinkedLists.Size - 1}");
            }
        }
        else
        {
          
            StartErrRoutine(m_PosErrCoroutine, m_PosOutline, "Please enter a valid integer");
        }
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

    IEnumerator OnDeleteRoutine(int pos)
    {
        m_PosOutline.OutlineColor = m_SuccessColor;

        yield return m_LinkedLists.Delete(pos);
        Debug.Log("Insert Control Routine Finished");

        m_Coroutine = null;

        m_PosOutline.OutlineColor = m_NormalColor;

        yield break;
    }
}
