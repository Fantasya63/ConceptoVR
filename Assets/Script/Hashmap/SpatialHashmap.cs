using Concepto;
using Concepto.HashMap;
using System.Collections;
using UnityEngine;

public class SpatialHashmap : MonoBehaviour
{
    [Header("Options")]
    [SerializeField] ScriptedHashFuncDev m_HashFuncDev;
    [SerializeField] SpatialLinkedLists m_LinkedListsPrefab;
    [SerializeField] int m_NumOfLists = 5;
    [SerializeField] Transform m_ArrStartTransform;
    [SerializeField] Vector3 m_ArrOffset;

    [SerializeField] Printer m_ScriptedPrinter;

    // Type command in Inspector 
    [SerializeField] string[] m_Commands;

    [Header("Anim Options")]
    [SerializeField] float m_GrowDur = 0.5f;
    [SerializeField] Transform m_PaperMovementHeightTransform;

    //SpatialLinkedLists m_LinkedListsInstance;
    SpatialLinkedLists[] m_LinkedListsArr;

    Coroutine m_InitRoutine;
    Coroutine m_CommandCoroutine;

    bool m_Initialized = false;
    private void Start()
    {
        Debug.Assert(m_HashFuncDev != null);
        Debug.Assert(m_LinkedListsPrefab != null);
        Debug.Assert(m_NumOfLists > 0);
        Debug.Assert(m_ArrStartTransform != null);
        Debug.Assert(m_ArrOffset.sqrMagnitude > 0);

        m_LinkedListsArr = new SpatialLinkedLists[m_NumOfLists];

        if (m_InitRoutine != null)
            StopCoroutine(m_InitRoutine);

        m_InitRoutine = StartCoroutine(Init());
    }

    
    public IEnumerator Init()
    {
        if (m_Initialized)
        {
            Debug.LogError($"Spatial Hashmap: {name} is already initialized");
            yield break;
        }

        for (int i = 0; i < m_NumOfLists; i++)
        {
            Vector3 pos = m_ArrStartTransform.position + m_ArrOffset * i;
            SpatialLinkedLists instance = Instantiate(m_LinkedListsPrefab, pos, m_ArrStartTransform.rotation, transform);
            instance.transform.localScale = Vector3.zero;

            instance.transform.LeanScale(Vector3.one, m_GrowDur);
            yield return new WaitForSeconds(m_GrowDur);
        }

        if (m_CommandCoroutine != null)
            StopCoroutine (m_CommandCoroutine);

        m_CommandCoroutine = StartCoroutine(RunCommand());
    }

    public IEnumerator RunCommand()
    {
        foreach (string _command in m_Commands)
        {
            Debug.Log("Command: " + _command);

            string[] parts = _command.Split(' ');

            string command = parts[0].ToLower();

            if (command == "insert")
            {
                string key = parts[1];
                string value = parts[0];

                Paper keyPaper = null;
                {
                    bool accepted = false;
                    bool finished = false;
                    while (!accepted)
                    {
                        accepted = m_ScriptedPrinter.PrintNoAnim(key, (Paper p) =>
                        {
                            keyPaper = p;
                            p.transform.SetParent(transform);
                            finished = true;
                        }, Paper.PAPER_TYPE.Data);

                        // wait for next frame
                        yield return null;
                    }

                    yield return new WaitUntil(() => finished);
                }
             
                Paper valuePaper = null;
                {
                    bool accepted = false;
                    bool finished = false;
                    while (!accepted)
                    {
                        accepted = m_ScriptedPrinter.PrintNoAnim(key, (Paper p) =>
                        {
                            valuePaper = p;
                            p.transform.SetParent(transform);
                            finished = true;
                        }, Paper.PAPER_TYPE.Data);

                        // wait for next frame
                        yield return null;
                    }

                    yield return new WaitUntil(() => finished);
                }

                yield return Insert(keyPaper, valuePaper);
            }
           
            else
            {
                Debug.Log($"Invalid command: {command}");
            }

            yield return new WaitForSeconds(0.5f);
        }

        m_CommandCoroutine = null;
        yield break;

    }

    public IEnumerator Insert(Paper key, Paper value)
    {
        Debug.Assert(key != null);
        Debug.Assert(value != null);

        // Hash the key
        Paper index = null;

        m_HashFuncDev.OnPaperPrinted.AddListener((Paper p) =>
        {
            index = p;
        });

        yield return m_HashFuncDev.Hash(key);

        Debug.Log($"Paper is: {index.name}");
    }

    //public IEnumerator Retrieve(Paper key)
    //{
    //    //
    //}

    //public IEnumerator Remove(Paper key)
    //{
    //    //
    //}
}
