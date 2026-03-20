using Concepto.HashMap;
using System.Collections;
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
        struct PaperKeyValue
        {
            public Paper key;
            public Paper value;

            public bool IsValid => key != null && value != null;
        }


        [Header("References")]
        [SerializeField] private AudioSource m_VoiceSource;
        [SerializeField] private Printer m_ScriptPrinter;
        [SerializeField] private Transform m_ScriptHashFuncDevStartTransform;
        [SerializeField] private Vector3 m_HashFuncDevOffset = Vector3.forward;
        [SerializeField] private Transform m_KeyValueStartTransform;
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
                instance.gameObject.SetActive(false);
                
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

            
            m_SlideCoroutine = StartCoroutine(SlideRoutine());
        }

        IEnumerator SlideRoutine()
        {
            // Init Papers
            for (int i = 0; i < m_PaperKeyValuesInstances.Length; i++)
            {
                PaperKeyValue paperKeyValue = m_PaperKeyValuesInstances[i];

                // Print Key
                m_ScriptPrinter.PrintNoAnim(m_KeyValue[i].key,
                    (p) => {
                        paperKeyValue.key = p;
                        paperKeyValue.key.gameObject.SetActive(false);
                    }, Paper.PAPER_TYPE.Hashkey);

                // Print Value
                m_ScriptPrinter.PrintNoAnim(m_KeyValue[i].value,
                    (p) => { 
                        paperKeyValue.value = p;
                        paperKeyValue.value.gameObject.SetActive(false);
                    }, Paper.PAPER_TYPE.Data);
            }


            yield return PlayAndWaitVoice(m_VoiceSource, m_YouMightHave);

            yield return new WaitUntil(() =>
            {
                return m_PaperKeyValuesInstances[0].IsValid && m_PaperKeyValuesInstances[1].IsValid;
            });

            {

                bool[] hasPrinted = new bool[m_PaperKeyValuesInstances.Length];
                for (int i = 0; i < m_PaperKeyValuesInstances.Length; ++i)
                {
                    PaperKeyValue paperKeyValue = m_PaperKeyValuesInstances[i];
                    paperKeyValue.key.transform.position = m_KeyValueStartTransform.position + m_HashFuncDevOffset * i;
                    paperKeyValue.value.transform.position = paperKeyValue.key.transform.position + m_KeyValueSeperation;

                    paperKeyValue.key.gameObject.SetActive(true);
                    paperKeyValue.value.gameObject.SetActive(true);

                    // HashFuncCallback
                    m_ScriptHashFuncDevInstances[i].OnPaperPrinted.AddListener((p) =>
                    {
                        hasPrinted[i] = true;
                    });
                }

                // Wait to finish hashing
                yield return new WaitUntil(() => 
                {
                    bool finished = true;
                    foreach(bool printed in hasPrinted)
                    {
                        finished = finished && printed;
                    }

                    return finished;
                    
                });
            }

            // Narrate, spawn boxes, while move resulting index to corresponding box
            {
                PlayVoiceNoWait(m_VoiceSource, m_WhenTwoKeys);

                yield return WaitForArray<BoxScriptController>(m_BoxArrayPrefab, HashFunc.NumBoxes, m_SpawnDelay, m_ArrayOffset, null, (arr) => {
                    BoxScriptinstances = arr;
                });
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
