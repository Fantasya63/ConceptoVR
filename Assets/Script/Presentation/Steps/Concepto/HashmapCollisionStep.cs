using Concepto.HashMap;
using System.Collections;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.UIElements;

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
        struct PaperKeyValue
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


        [Header("References")]
        [SerializeField] private AudioSource m_VoiceSource;
        [SerializeField] private Printer m_ScriptPrinter;
        [SerializeField] private Transform m_ScriptHashFuncDevStartTransform;
        [SerializeField] private Transform m_KeyValueStartTransform;
        //[SerializeField] private Transform m_BoxArrStartTransform;

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


        BoxScriptController[] BoxScriptinstances = null;
        private bool[] m_HasPrintedArr;

        private void Awake()
        {
            m_HasPrintedArr = new bool[m_PaperKeyValuesInstances.Length];

            for (int i = 0; i < 2; i++)
            {
                m_PaperKeyValuesInstances[i] = new PaperKeyValue(null, null);
            }
        }

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
                HashFuncDevice instance = Instantiate(m_ScriptHashFuncDevPrefab);
                instance.transform.position = m_ScriptHashFuncDevStartTransform.position + m_HashFuncDevOffset * i;
                instance.transform.rotation = m_ScriptHashFuncDevStartTransform.rotation;
                instance.gameObject.SetActive(false);
                instance.OnPaperPrinted.AddListener((p) =>
                {
                    Debug.Log("Start");
                });
                m_ScriptHashFuncDevInstances[i] = instance;
            }

        }

        public override void Activate()
        {
            if (m_SlideCoroutine != null)
                StopCoroutine(m_SlideCoroutine);

            if (BoxScriptinstances != null)
            {
                foreach (var instance in BoxScriptinstances)
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
            PlayVoiceNoWait(m_VoiceSource, m_YouMightHave);

            // Init Papers
            for (int i = 0; i < 2; i++)
            {
                int index = i;

                // Print Key
                yield return m_ScriptPrinter.PrintNoAnimEnumarator(m_KeyValue[index].key,
                    (p) =>
                    {
                        m_PaperKeyValuesInstances[index].key = p;
                        p.transform.rotation = m_KeyValueStartTransform.rotation;
                        
                        m_PaperKeyValuesInstances[index].key.gameObject.SetActive(true);
                        m_PaperKeyValuesInstances[index].key.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                    }, Paper.PAPER_TYPE.Data);

                // Print Value
                yield return m_ScriptPrinter.PrintNoAnimEnumarator(m_KeyValue[i].value,
                    (p) =>
                    {
                        m_PaperKeyValuesInstances[index].value = p;
                        p.transform.rotation = m_KeyValueStartTransform.rotation;

                        m_PaperKeyValuesInstances[index].value.gameObject.SetActive(true);
                        m_PaperKeyValuesInstances[index].value.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                    }, Paper.PAPER_TYPE.Data);

                m_ScriptHashFuncDevInstances[index].gameObject.SetActive(true);
            }

            yield return new WaitUntil(() => !m_VoiceSource.isPlaying);

            {
                Debug.Log("PaperColisionTest");

                Debug.Log($"Length: {m_PaperKeyValuesInstances.Length}");

                for (int i = 0; i < m_PaperKeyValuesInstances.Length; ++i)
                {
                    int currIndex = i;

                    PaperKeyValue paperKeyValue = m_PaperKeyValuesInstances[i];
                    paperKeyValue.key.transform.position = m_KeyValueStartTransform.position + m_HashFuncDevOffset * i;
                    paperKeyValue.value.transform.position = paperKeyValue.key.transform.position + m_KeyValueSeperation;
                    paperKeyValue.key.gameObject.GetComponent<Rigidbody>().isKinematic = false;

                    Debug.Log($"CollisionStep: HashFuncDevInstance: {i} - {m_ScriptHashFuncDevInstances[i].name}");

                    // HashFuncCallback
                    m_ScriptHashFuncDevInstances[currIndex].OnPaperPrinted.AddListener((Paper p) =>
                    {
                        Debug.Log("Hash");
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
                    Vector3.zero,
                    m_ArrayOffset, 
                    (BoxScriptController instance) =>
                    {
                        Debug.Log("Instanceeee");
                        
                    }
                    , (arr) => {
                        BoxScriptinstances = arr;
                    }
                );

                Debug.Log("Spawned Boxes");

            }

            // Narrate Seperate chaining
            {

            }

            // Switch array to array of linked lists
            {

            }

            // Demonstrate insertion
            {

            }

            // Demonstrate resize
            {

            }

            // Summary
            {

            }

            // Sandbox
            Debug.Log("Finished");
            Complete();
        }


        public override void Deactivate()
        {
            if (m_SlideCoroutine != null)
            {
                StopCoroutine(m_SlideCoroutine);
                m_SlideCoroutine = null;
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
            if (BoxScriptinstances != null)
            {
                foreach (var instance in BoxScriptinstances)
                {
                    Destroy(instance);
                }
            }
        }

        public override void OnSlideExit()
        {
            throw new System.NotImplementedException();
        }
    }

}
