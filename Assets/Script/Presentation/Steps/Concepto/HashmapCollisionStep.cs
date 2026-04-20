using Concepto.HashMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Canvas
{
    public class HashmapCollisionStep : Step
    {
        [System.Serializable]
        struct KeyValue
        {
            public string key;
            public string value;
        }
        [System.Serializable]
        class PaperKeyValue
        {
            public Paper key;
            public Paper value;

            public PaperKeyValue(Paper _key, Paper _value)
            {
                key = _key;
                value = _value;
            }

            public bool IsValid => key != null && value != null;

            public override string ToString()
            {
               return $"Key: {key.data}, value: {value.data}";
            }
        }

        List<GameObject> m_TempObjects = new List<GameObject>();


        [Header("References")]
        [SerializeField] private AudioSource m_VoiceSource;
        [SerializeField] private Printer m_ScriptPrinter;
        [SerializeField] private Transform m_ScriptHashFuncDevStartTransform;
        [SerializeField] private Transform m_KeyValueStartTransform;
        [SerializeField] private Transform m_BoxArrStartTransform;
        [SerializeField] private Transform m_IndexShowErrorTransform;

        [SerializeField] private Vector3 m_HashFuncDevOffset = Vector3.forward;
        [SerializeField] private Vector3 m_KeyValueSeperation = Vector3.forward * 0.2f;

        [Header("Prefabs")]
        [SerializeField] private HashFuncDevice m_ScriptHashFuncDevPrefab;
        [SerializeField] private BoxScriptController m_BoxArrayPrefab;

        [Header("Array")]
        [SerializeField] private Vector3 m_ArrayOffset = Vector3.forward * 0.2f;
        int m_ArrayLength = HashFunc.NumBoxes;


        [Header("Animation")]
        [SerializeField] private float m_SpawnDelay = 0.1f;
        [SerializeField] private float m_IndexToBoxLerpDur = 1.0f;
        [SerializeField] private float m_ErrorOutlineDur = 3.0f;

        [Header("Voice Overs")]
        [SerializeField] private AudioClip m_YouMightHave;
        [SerializeField] private AudioClip m_WhenTwoKeys;
        [SerializeField] private AudioClip m_OneMethod;
        [SerializeField] private AudioClip m_Instead;

        [Header("Examples")]
        [SerializeField] private KeyValue[] m_KeyValue = new KeyValue[2];

        private PaperKeyValue[] m_PaperKeyValuesInstances = new PaperKeyValue[2];
        private HashFuncDevice[] m_ScriptHashFuncDevInstances = new HashFuncDevice[2];
        private Coroutine m_SlideCoroutine;


        BoxScriptController[] m_BoxScriptinstances = null;
        private bool[] m_HasPrintedArr;

        private void Awake()
        {
            m_HasPrintedArr = new bool[m_PaperKeyValuesInstances.Length];

            for (int i = 0; i < 2; i++)
            {
                m_PaperKeyValuesInstances[i] = new PaperKeyValue(null, null);
            }
        }
        Paper[] m_PrintedIndexes;

        private void Start()
        {
            Debug.Assert(m_VoiceSource != null);
            Debug.Assert(m_YouMightHave != null);
            Debug.Assert(m_WhenTwoKeys != null);
            Debug.Assert(m_OneMethod != null);
            Debug.Assert(m_Instead != null);

            Debug.Assert(m_ScriptHashFuncDevPrefab != null);

            Debug.Assert(
                m_PaperKeyValuesInstances.Length == m_KeyValue.Length 
                && m_PaperKeyValuesInstances.Length == m_ScriptHashFuncDevInstances.Length
            );

            for (int i = 0; i < 2; i++)
            {
                HashFuncDevice instance = Instantiate(m_ScriptHashFuncDevPrefab, transform);
                m_TempObjects.Add(instance.gameObject);

                instance.transform.position = m_ScriptHashFuncDevStartTransform.position + m_HashFuncDevOffset * i;
                instance.transform.rotation = m_ScriptHashFuncDevStartTransform.rotation;
                instance.gameObject.SetActive(false);
                instance.OnPaperPrinted.AddListener((p) =>
                {
                    Debug.Log("Start");
                });
                m_ScriptHashFuncDevInstances[i] = instance;
            }

            m_PrintedIndexes = new Paper[m_PaperKeyValuesInstances.Length];
        }

        public override void Activate()
        {
            if (m_SlideCoroutine != null)
                StopCoroutine(m_SlideCoroutine);

            if (m_BoxScriptinstances != null)
            {
                foreach (var instance in m_BoxScriptinstances)
                {
                    Destroy(instance);
                }
            }

            for (int i = 0; i < 2; i++)
            {
                m_HasPrintedArr[i] = false;
            }

            
            m_SlideCoroutine = StartCoroutine(SlideRoutine());
        }

        IEnumerator SlideRoutine()
        {

            // Init Papers
            for (int i = 0; i < 2; i++)
            {
                int index = i;

                // Print Key
                yield return m_ScriptPrinter.PrintNoAnimEnumarator(m_KeyValue[index].key,
                    (p) =>
                    {
                        m_PaperKeyValuesInstances[index].key = p;
                        m_PaperKeyValuesInstances[index].key.gameObject.GetComponent<Rigidbody>().isKinematic = true;

                        p.transform.rotation = m_KeyValueStartTransform.rotation;
                        p.transform.position = m_KeyValueStartTransform.position + m_HashFuncDevOffset * i;

                        m_PaperKeyValuesInstances[index].key.gameObject.SetActive(true);
                    }, Paper.PAPER_TYPE.Data);

                // Print Value
                yield return m_ScriptPrinter.PrintNoAnimEnumarator(m_KeyValue[i].value,
                    (p) =>
                    {
                        m_PaperKeyValuesInstances[index].value = p;
                        m_PaperKeyValuesInstances[index].value.gameObject.GetComponent<Rigidbody>().isKinematic = true;

                        p.transform.rotation = m_KeyValueStartTransform.rotation;
                        p.transform.position = m_PaperKeyValuesInstances[i].key.transform.position + m_KeyValueSeperation;
                        m_PaperKeyValuesInstances[index].value.gameObject.SetActive(true);
                    }, Paper.PAPER_TYPE.Data);

                m_ScriptHashFuncDevInstances[index].gameObject.SetActive(true);
            }

            PlayVoiceNoWait(m_VoiceSource, m_YouMightHave);

            yield return new WaitUntil(() => !m_VoiceSource.isPlaying);

            {
                Debug.Log("PaperColisionTest");
                Debug.Log($"Length: {m_PaperKeyValuesInstances.Length}");

                for (int i = 0; i < m_PaperKeyValuesInstances.Length; ++i)
                {
                    int currIndex = i;

                    PaperKeyValue paperKeyValue = m_PaperKeyValuesInstances[i];
                    paperKeyValue.key.gameObject.GetComponent<Rigidbody>().isKinematic = false;

                    Debug.Log($"CollisionStep: HashFuncDevInstance: {i} - {m_ScriptHashFuncDevInstances[i].name}");

                    // HashFuncCallback
                    m_ScriptHashFuncDevInstances[currIndex].OnPaperPrinted.AddListener((Paper p) =>
                    {
                        Debug.Log("Hash");
                        m_PrintedIndexes[currIndex] = p;
                        m_HasPrintedArr[currIndex] = true;
                    });

                }

                // Wait to finish hashing
                yield return new WaitUntil(() => 
                {
                    return m_HasPrintedArr[0] && m_HasPrintedArr[1];
                });
            }

            // Narrate, spawn boxes, while move resulting index to corresponding box
            {
                PlayVoiceNoWait(m_VoiceSource, m_WhenTwoKeys);
                Debug.Log("Spawn Boxes");
                yield return WaitForArray<BoxScriptController>(
                    m_BoxArrayPrefab, 
                    HashFunc.NumBoxes, 
                    m_SpawnDelay,
                    m_BoxArrStartTransform.position,
                    m_ArrayOffset, 
                    (BoxScriptController instance, int index) =>
                    {
                        Debug.Log("Instanceeee");
                        instance.SetLabel(index.ToString());
                    }
                    , (arr) => {
                        m_BoxScriptinstances = arr;
                    }
                );

                Debug.Log("Spawned Boxes");

            }

            // Move Paper into key position:
            {
                Outline[] outlines = new Outline[m_PrintedIndexes.Length];
                for (int i = 0; i < m_PrintedIndexes.Length; i++) 
                {
                    Paper indexPaper = m_PrintedIndexes[i];
                    Rigidbody rbody = indexPaper.GetComponent<Rigidbody>();
                    rbody.isKinematic = true;

                    Quaternion startRot = indexPaper.transform.rotation;

                    Vector3 _pos = m_IndexShowErrorTransform.position + m_KeyValueSeperation * i;
                    indexPaper.transform.LeanMove(_pos, m_IndexToBoxLerpDur);
                    indexPaper.transform.rotation = Quaternion.Lerp(startRot, m_IndexShowErrorTransform.rotation, m_IndexToBoxLerpDur);

                    // Flash Error
                    Outline outline = indexPaper.GetComponent<Outline>();
                    outlines[i] = outline;
                }

                yield return new WaitForSeconds(m_IndexToBoxLerpDur);

                foreach (var outline in outlines)
                {
                    outline.enabled = true;
                    outline.OutlineColor = Color.red;
                }

                yield return new WaitForSeconds(m_ErrorOutlineDur);

                yield return new WaitUntil(() => !m_VoiceSource.isPlaying);

                for (int i = 0; i < outlines.Length; i++)
                {
                    outlines[i].enabled = false;
                }
            }

            // Sandbox
            Debug.Log("Finished");
            Complete();
        }


        public override void Deactivate()
        {
            for (int i = 0; i < m_TempObjects.Count; i++)
            {
                Destroy(m_TempObjects[i]);
            }
            m_TempObjects.Clear();

            if (m_SlideCoroutine != null)
            {
                StopCoroutine(m_SlideCoroutine);
                m_SlideCoroutine = null;
            }

            for (int i = 0; i < m_PrintedIndexes.Length; i++)
            {
                Paper p = m_PrintedIndexes[i];
                if (p != null)
                    Destroy(p.gameObject);
            }

            for (int i = 0; i < m_PaperKeyValuesInstances.Length; i++)
            {
                PaperKeyValue paperKeyValue = m_PaperKeyValuesInstances[i];

                if (paperKeyValue.key != null)
                {
                    Destroy(paperKeyValue.key.gameObject);
                    paperKeyValue.key = null;
                }

                if (paperKeyValue.value != null)
                {
                    Destroy(paperKeyValue.value.gameObject);
                    paperKeyValue.value = null;
                }
            }
            if (m_BoxScriptinstances != null)
            {
                foreach (var instance in m_BoxScriptinstances)
                {
                    Destroy(instance.gameObject);
                }
            }
        }

        public override void OnSlideExit()
        {
           
        }
    }

}
