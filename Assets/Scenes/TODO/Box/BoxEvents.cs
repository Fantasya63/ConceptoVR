using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class BoxEvents : MonoBehaviour
{
    enum ACTION_TYPE
    {
        Insertion = 0,
        Retrieval,
        RetriveAndReplace
    }

    [SerializeField]
    [TextArea(5, 20)]
    private string[] m_CodeEquivalents;


    [Header("References")]
    [SerializeField] private XRSocketInteractor m_PaperDataSocket;
    [SerializeField] private XRSocketInteractor m_PaperIndexSocket;
    [SerializeField] private TMP_Text m_IndexLabel;
    [SerializeField] private BoxKeyLock m_KeyLock;

    [Header("Events")]
    public UnityEvent<Paper> OnIndexInserted;
    public UnityEvent<Paper> OnIndexRemoved;

    public UnityEvent<Paper> OnDataInserted;
    public UnityEvent<Paper> OnDataRemoved;


    [Header("Visualization")]
    public ScriptVisualizer Visualizer;
    
    bool m_ActionStarted = false;
    Paper m_BeginPaper = null;


    private void Start()
    {
        m_PaperDataSocket.selectEntered.AddListener(EmitDataInserted);

        m_PaperDataSocket.selectEntered.AddListener(EmitDataInserted);
        m_PaperDataSocket.selectExited.AddListener(EmitDataRemoved);

        m_PaperIndexSocket.selectEntered.AddListener(EmitIndexInserted);
        m_PaperIndexSocket.selectExited.AddListener(EmitIndexRemoved);

        Debug.Assert(m_KeyLock != null);
        Debug.Assert(m_IndexLabel != null);
        
    }

    public void SetIndex(int index)
    {
        string indexText = index.ToString();
        m_IndexLabel.text = indexText;
        m_KeyLock.index = indexText;
    }

    void EmitIndexInserted(SelectEnterEventArgs arg0)
    {
        m_ActionStarted = true;
        m_BeginPaper = Utils.GetInsertedPaper(m_PaperDataSocket);
        OnIndexInserted.Invoke(Utils.GetInsertedPaper(m_PaperIndexSocket));
        Debug.Log($"Box Index Inserted: Starting paper: {m_BeginPaper?.data}");
    }


    void EmitIndexRemoved(SelectExitEventArgs arg0)
    {
        Paper indexPaper = ((XRBaseInteractable)arg0.interactableObject).GetComponent<Paper>();
        Paper dataPaper = Utils.GetInsertedPaper(m_PaperIndexSocket);


        DetermineAction(indexPaper.data, dataPaper.data);

        OnIndexRemoved.Invoke(indexPaper);
        
        
        m_ActionStarted = false;

        m_BeginPaper = null;
    }

    void EmitDataInserted(SelectEnterEventArgs arg0)
    {
        OnDataInserted.Invoke(Utils.GetInsertedPaper(m_PaperDataSocket));
    }

    void EmitDataRemoved(SelectExitEventArgs arg0)
    {
        Paper removedData = ((XRBaseInteractable)arg0.interactableObject).GetComponent<Paper>();
        OnDataRemoved.Invoke(removedData);
    }


    void DetermineAction(string index, string currData)
    {
        Paper endPaper = Utils.GetInsertedPaper(m_PaperDataSocket);

        // None. Nothing was done
        if (m_BeginPaper == endPaper)
            return;

        // Insertion
        if (m_BeginPaper == null && endPaper != null)
        {
            ShowCodeEquivalent(ACTION_TYPE.Insertion, index, currData);
            return;
        }

        // Retrieval
        if (m_BeginPaper != null && endPaper == null)
        {
            ShowCodeEquivalent(ACTION_TYPE.Retrieval, index, m_BeginPaper.data);
            return;
        }

        // RetriveAndReplace
        if (m_BeginPaper != null && endPaper != null)
        {
            ShowCodeEquivalent(ACTION_TYPE.RetriveAndReplace, index, currData);
        }

    }

    void ShowCodeEquivalent(ACTION_TYPE type, string index, string data)
    {
        Visualizer.SetCodeWithNotif(string.Format(m_CodeEquivalents[(int)type], index, data));
    }
}
