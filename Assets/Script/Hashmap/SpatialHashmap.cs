using Concepto;
using Concepto.HashMap;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class SpatialHashmap : MonoBehaviour
{
    [Header("Options")]
    [SerializeField] ScriptedHashFuncDev m_HashFuncDev;
    [SerializeField] HashmapLinkedLists m_LinkedListsPrefab;
    [SerializeField] int m_NumOfLists = Concepto.HashMap.HashFunc.NumBoxes;
    [SerializeField] Transform m_ArrStartTransform;
    [SerializeField] Vector3 m_ArrOffset;

    public Printer m_ScriptedPrinter;
    [SerializeField] Transform m_RetrievedValTransform;
    [SerializeField] float m_ToRetrievedValDur = 2.0f;
    [SerializeField] float m_ToRetrievedValShowDur = 3.0f;

    // Type command in Inspector 
    [SerializeField] string[] m_Commands;

    [Header("Anim Options")]
    [SerializeField] float m_GrowDur = 0.5f;
    [SerializeField] Transform m_TransitHeightTransform;
    [SerializeField] float m_ToTransitHeightDur = 1.0f;
    [SerializeField] float m_ToIndexPosDur = 2.0f;

    //SpatialLinkedLists m_LinkedListsInstance;
    HashmapLinkedLists[] m_LinkedListsArr;

    Coroutine m_InitRoutine;
    Coroutine m_CommandCoroutine;

    bool m_Initialized = false;

    List<GameObject> m_TempObjects = new List<GameObject>();

    Vector3 GetIndexPos(int index)
    {
        Debug.Assert(index >= 0 && index < m_LinkedListsArr.Length, $"Index: {index} is out of range. Curr range: 0 - {m_LinkedListsArr.Length}");
        Vector3 pos = m_LinkedListsArr[index].transform.position;
        pos.y = m_TransitHeightTransform.position.y;

        return pos;
    }

    private void Start()
    {
        Debug.Assert(m_HashFuncDev != null);
        Debug.Assert(m_LinkedListsPrefab != null);
        Debug.Assert(m_NumOfLists > 0);
        Debug.Assert(m_ArrStartTransform != null);
        Debug.Assert(m_ArrOffset.sqrMagnitude > 0);
        Debug.Assert(m_TransitHeightTransform != null);
        Debug.Assert(m_RetrievedValTransform != null);

        m_LinkedListsArr = new HashmapLinkedLists[m_NumOfLists];

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
            HashmapLinkedLists instance = Instantiate(m_LinkedListsPrefab, pos, m_ArrStartTransform.rotation, transform);
            instance.transform.localScale = Vector3.zero;
            instance.transform.LeanScale(Vector3.one, m_GrowDur);

            m_LinkedListsArr[i] = instance;
            yield return new WaitForSeconds(m_GrowDur);
        }

        //if (m_CommandCoroutine != null)
        //    StopCoroutine (m_CommandCoroutine);

        //m_CommandCoroutine = StartCoroutine(RunCommand());
    }

    public IEnumerator RunCommand()
    {
        if (m_Commands == null)
            yield break;

        foreach (string _command in m_Commands)
        {
            Debug.Log("Command: " + _command);

            string[] parts = _command.Split(' ');

            string command = parts[0].ToLower();

            if (command == "insert")
            {
                string key = parts[1];
                string value = parts[2];

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

                keyPaper.RemoveInteractivity();


                Paper valuePaper = null;
                {
                    bool accepted = false;
                    bool finished = false;
                    while (!accepted)
                    {
                        accepted = m_ScriptedPrinter.PrintNoAnim(value, (Paper p) =>
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
                valuePaper.RemoveInteractivity();


                yield return Insert(keyPaper, valuePaper);
            }
            else if (command == "remove")
            {
                string keyString = parts[1];

                Paper key = null;
                yield return StringToPaper(keyString, Paper.PAPER_TYPE.Hashkey, (Paper p) =>
                {
                    key = p;
                });

                Debug.Assert(key != null);
                yield return Remove(key, (bool success, string message) =>
                {
                    if (!success)
                    {
                        Debug.LogWarning(message);
                    }    
                });
            }
            else if (command == "retrieve")
            {
                string keyString = parts[1];

                Paper key = null;
                yield return StringToPaper(keyString, Paper.PAPER_TYPE.Hashkey, (Paper p) =>
                {
                    key = p;
                });

                Debug.Assert(key != null);
                yield return Retrieve(key, (bool success, string message)=>
                {
                    if (!success)
                        Debug.LogWarning(message);
                });
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

    public IEnumerator HashWithNaration(Paper _key, System.Action<Paper> onComplete)
    {
        Paper key = Instantiate(_key, transform);
        key.transform.position = _key.transform.position;
        key.transform.rotation = _key.transform.rotation;
        key.GetComponent<MeshRenderer>().material = _key.GetComponent<MeshRenderer>().material;
        key.RemoveInteractivity();

        key.RemoveInteractivity();
        // Hash the key
        Paper index = null;

        m_HashFuncDev.OnPaperPrinted.AddListener((Paper p) =>
        {
            index = p;
        });

        m_TempObjects.Add(index.gameObject);
        yield return m_HashFuncDev.Hash(key);

        onComplete.Invoke(index);

        yield break;
    }

    public IEnumerator PaperToIndexPos(Paper index)
    {

        int indexValue = -1;
        if (!int.TryParse(index.data, out indexValue))
        {
            Debug.LogError($"Paper Index has non integer value of : {index.data}");
            yield break;
        }

        // Move to transit height 
        {
            Quaternion startRot = index.transform.rotation;
            Quaternion endRot = m_TransitHeightTransform.rotation;
            Vector3 pos = index.transform.position;
            pos.y = m_TransitHeightTransform.position.y;
            index.transform.LeanMove(pos, m_ToTransitHeightDur)
                .setOnUpdate((float f) =>
                {
                    index.transform.rotation = Quaternion.Slerp(startRot, endRot, f / m_ToTransitHeightDur);
                });
            yield return new WaitForSeconds(m_ToTransitHeightDur);
        }

        // Move to index pos
        {
            Vector3 pos = GetIndexPos(indexValue);
            index.transform.LeanMove(pos, m_ToIndexPosDur);
            yield return new WaitForSeconds(m_ToIndexPosDur);
        }
    }

    public IEnumerator StringToPaper(string input, Paper.PAPER_TYPE paperType,  System.Action<Paper> onFinished)
    {
        Paper keyPaper = null;
        {
            bool accepted = false;
            bool finished = false;
            while (!accepted)
            {
                accepted = m_ScriptedPrinter.PrintNoAnim(input, (Paper p) =>
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

        onFinished.Invoke(keyPaper);
    }

    public IEnumerator Insert(Paper key, Paper value)
    {
        Debug.Assert(key != null);
        Debug.Assert(value != null);

        key.RemoveInteractivity();
        value.RemoveInteractivity();
       
        // Hash the key
        Paper index = null;

        m_HashFuncDev.OnPaperPrinted.AddListener((Paper p) =>
        {
            index = p;
        });


        yield return m_HashFuncDev.Hash(key);

        int indexValue = -1;
        if (!int.TryParse(index.data, out indexValue))
        {
            Debug.LogError($"Paper Index has non integer value of : {index.data}");
            yield break;
        }


        // Move to transit height 
        {
            Quaternion startRot = index.transform.rotation;
            Quaternion endRot = m_TransitHeightTransform.rotation;
            Vector3 pos = index.transform.position;
            pos.y = m_TransitHeightTransform.position.y;
            index.transform.LeanMove(pos, m_ToTransitHeightDur)
                .setOnUpdate((float f) =>
                {
                    index.transform.rotation = Quaternion.Slerp(startRot, endRot, f / m_ToTransitHeightDur);
                });
            yield return new WaitForSeconds(m_ToTransitHeightDur);
        }

        // Move to index pos
        {
            Vector3 pos = GetIndexPos(indexValue);
            index.transform.LeanMove(pos, m_ToIndexPosDur);
            yield return new WaitForSeconds(m_ToIndexPosDur);
        }

        // Linked Lists Insert
        {
            yield return m_LinkedListsArr[indexValue].Insert(key, value);
        }

        Destroy(index.gameObject);
        Debug.Log($"Paper is: {index.name}");
    }

    public IEnumerator TraverseAndReplaceOrCreateNaration(Paper key, Paper value, Paper index)
    {
        int indexValue;
        if (!int.TryParse(index.data, out indexValue)) 
        {
            Debug.LogError("Index has a non numeric data.");
            yield break;
        }

        // Linked Lists Insert
        {
            yield return m_LinkedListsArr[indexValue].Insert(key, value);
        }

        Destroy(index.gameObject);
        Debug.Log($"Paper is: {index.name}");
    }

    public IEnumerator Retrieve(Paper key, System.Action<bool, string> onFinished)
    {
        key.RemoveInteractivity();

        Paper index = null;
        m_HashFuncDev.OnPaperPrinted.AddListener((Paper p) =>
        {
            index = p;
        });


        // Hash
        yield return m_HashFuncDev.Hash(key);

        Debug.Assert(index != null);
        m_TempObjects.Add(index.gameObject);


        int indexValue;
        if (!int.TryParse(index.data, out indexValue))
        {
            Debug.LogError($"Paper Index has non integer value of : {index.data}");
            Destroy(index.gameObject);
            yield break;
        }


        // Move to transit height 
        {
            Quaternion startRot = index.transform.rotation;
            Quaternion endRot = m_TransitHeightTransform.rotation;
            Vector3 pos = index.transform.position;
            pos.y = m_TransitHeightTransform.position.y;
            index.transform.LeanMove(pos, m_ToTransitHeightDur)
                .setOnUpdate((float f) =>
                {
                    index.transform.rotation = Quaternion.Slerp(startRot, endRot, f / m_ToTransitHeightDur);
                });
            yield return new WaitForSeconds(m_ToTransitHeightDur);
        }

        // Move to index pos
        {
            Vector3 pos = GetIndexPos(indexValue);
            index.transform.LeanMove(pos, m_ToIndexPosDur);
            yield return new WaitForSeconds(m_ToIndexPosDur);
        }


        // Linked Lists Retrive
        {
            bool success = false;
            string message = null;
            Paper paper = null;

            yield return m_LinkedListsArr[indexValue].Retrieve(key,
                (bool _success, string _message, Paper _paper) =>
                {
                    success = _success;
                    message = _message;
                    paper = _paper;
                });


            if (!success)
            {
                onFinished.Invoke(false, message);

                if (index != null)
                {
                    m_TempObjects.Remove(index.gameObject);
                    Destroy(index.gameObject);
                }


                Debug.LogWarning(message);

                yield break;
            }

            Paper newPaper = Instantiate(paper, transform);
            newPaper.transform.position = paper.transform.position;
            newPaper.transform.rotation = paper.transform.rotation;
            newPaper.GetComponent<MeshRenderer>().material = paper.GetComponent<MeshRenderer>().material;
            newPaper.RemoveInteractivity();

            newPaper.transform.LeanMove(m_RetrievedValTransform.position, m_ToRetrievedValDur);
            yield return new WaitForSeconds(m_ToRetrievedValDur);

            yield return new WaitForSeconds(m_ToRetrievedValShowDur);

            if (key != null)
                Destroy(key.gameObject);
            
            if (index != null)
            {
                m_TempObjects.Remove(index.gameObject);
                Destroy(index.gameObject);
            }

            if (newPaper != null)
                Destroy(newPaper.gameObject);

            onFinished.Invoke(true, "Retrieval success");
        }

    }
    
    public IEnumerator Remove(Paper key, System.Action<bool, string> onFinished)
    {
        key.RemoveInteractivity();

        Paper index = null;
        m_HashFuncDev.OnPaperPrinted.AddListener((Paper p) =>
        {
            index = p;
        });

        
        // Hash
        yield return m_HashFuncDev.Hash(key);

        Debug.Assert(index != null);

        int indexValue;
        if (!int.TryParse(index.data, out indexValue))
        {
            //Debug.LogError($"Paper Index has non integer value of : {index.data}");
            onFinished.Invoke(false, $"Paper Index has non integer value of : {index.data}");
            Destroy(index.gameObject);

            yield break;
        }


        // Move to transit height 
        {
            Quaternion startRot = index.transform.rotation;
            Quaternion endRot = m_TransitHeightTransform.rotation;
            Vector3 pos = index.transform.position;
            pos.y = m_TransitHeightTransform.position.y;
            index.transform.LeanMove(pos, m_ToTransitHeightDur)
                .setOnUpdate((float f) =>
                {
                    index.transform.rotation = Quaternion.Slerp(startRot, endRot, f / m_ToTransitHeightDur);
                });
            yield return new WaitForSeconds(m_ToTransitHeightDur);
        }

        // Move to index pos
        {
            Vector3 pos = GetIndexPos(indexValue);
            index.transform.LeanMove(pos, m_ToIndexPosDur);
            yield return new WaitForSeconds(m_ToIndexPosDur);
        }

        // Linked Lists Remove
        {
            yield return m_LinkedListsArr[indexValue].Remove(key, 
                (bool success, string message) =>
                {
                    if (key != null) 
                    if (success)
                    {
                        Debug.Log($"Node with key: {key.data} is removed");   
                    }
                    else
                    {
                        Debug.LogWarning(message);
                    }

                    Destroy(index.gameObject);

                    onFinished.Invoke(success, message);
                });
        }

        if (index != null)
            Destroy(index.gameObject);

    }

    private void OnDestroy()
    {
        if (m_TempObjects == null)
            return;

        for (int i = 0; i < m_TempObjects.Count; i++)
        {
            GameObject go = m_TempObjects[i];

            if (go != null)
                Destroy(go);
        }

        m_TempObjects.Clear();
    }

    
}
