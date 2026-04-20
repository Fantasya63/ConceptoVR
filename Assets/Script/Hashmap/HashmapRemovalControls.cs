using Concepto;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class HashmapRemovalControls : HashmapControls
{
    [Header("References")]
    [SerializeField] XRSocketInteractor m_KeySocket;
    [SerializeField] Transform m_RisePosTransform;

    [Header("Error Options")]
    [SerializeField] Color m_NormalColor = Color.white;
    [SerializeField] Color m_ErrorColor = Color.red;
    [SerializeField] Color m_SuccessColor = Color.green;
    [SerializeField] float m_FlashDur = 0.2f;
    [SerializeField] int m_NumOfFlash = 3;

    [Header("Animation")]
    [SerializeField] float m_ToRisePosDur = 1.0f;
    [SerializeField] float m_RisePosWaitDur = 1.0f;


    private Outline m_KeyOutline;
    Coroutine m_Coroutine;
    Coroutine m_KeyErrCoroutine;

    private void Start()
    {
        Debug.Assert(m_Hashmap != null);
        Debug.Assert(m_KeySocket != null);
        Debug.Assert(m_ErrorPanel != null);
        Debug.Assert(m_RisePosTransform != null);

        m_KeyOutline = GetOrAddOutline(m_KeySocket.gameObject);

        m_KeyOutline.OutlineColor = m_NormalColor;
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
            Error("Please wait till the operation is finished.");
            return;
        }

        Paper keyPaper = Utils.GetInsertedPaper(m_KeySocket);

        if (!CheckPaperNotNull(m_KeyErrCoroutine, keyPaper, m_KeyOutline, "Please enter a paper in the key socket"))
            return;

        m_Coroutine = StartCoroutine(OnRemoveRoutine(keyPaper));

    }


    IEnumerator OnErrorRoutine(Outline outline, string message)
    {
        Error(message);
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
    }

    IEnumerator OnRemoveRoutine(Paper _key)
    {
        Paper key = Utils.CopyPaper(_key, transform);


        // Move Upwards
        {
            Vector3 keyRisePos = key.transform.position;
            keyRisePos.y = m_RisePosTransform.position.y;


            key.transform.LeanMove(keyRisePos, m_ToRisePosDur);

            yield return new WaitForSeconds(m_ToRisePosDur);
            yield return new WaitForSeconds(m_RisePosWaitDur);

        }

        m_KeyOutline.OutlineColor = m_SuccessColor;

        bool success = false;
        string message = string.Empty;

        yield return m_Hashmap.Remove(key, 
            (bool _success, string _message) => {
                success = _success;
                message = _message;
            });
        

        if (!success)
        {
            Error($"Key value of {_key.data} is not found in the hashmap.");
        }

        if (key != null)
            Destroy(key.gameObject);
        Debug.Log("Remove Control Routine Finished");
        m_Coroutine = null;
        m_KeyOutline.OutlineColor = m_NormalColor;
        yield break;
    }
}
