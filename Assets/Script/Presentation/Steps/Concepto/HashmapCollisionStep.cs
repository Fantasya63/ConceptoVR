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

        private HashFuncDevice[] m_ScriptHashFuncDevInstances = new HashFuncDevice[2];
        private Coroutine m_SlideCoroutine;


        BoxScriptController[] BoxScriptinstances = null;
        private Paper m_LKey = null;
        private Paper m_LValue = null;
        private Paper m_RKey = null;
        private Paper m_RValue = null;


        private void Start()
        {
            Debug.Assert(m_VoiceSource != null);

            Debug.Assert(m_YouMightHave != null);
            Debug.Assert(m_WhenTwoKeys != null);
            Debug.Assert(m_OneMethod != null);
            Debug.Assert(m_Instead != null);

            Debug.Assert(m_ScriptHashFuncDevPrefab != null);

            Debug.Assert(m_KeyValue.Length == m_ScriptHashFuncDevInstances.Length);

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
            //for (int i = 0; i < 2; i++)
            //{
            //    int index = i;

            //    // Print Key
            //    m_ScriptPrinter.PrintNoAnim(m_KeyValue[index].key,
            //        (p) => {
            //            m_PaperKeyValuesInstances[index].key = p;
            //            m_PaperKeyValuesInstances[index].key.gameObject.SetActive(false);
            //        }, Paper.PAPER_TYPE.Hashkey);

            //    // Print Value
            //    m_ScriptPrinter.PrintNoAnim(m_KeyValue[i].value,
            //        (p) => {
            //            m_PaperKeyValuesInstances[index].value = p;
            //            m_PaperKeyValuesInstances[index].value.gameObject.SetActive(false);
            //            Debug.Log($"ColisionTest: {m_PaperKeyValuesInstances[index].value.data}");
            //        }, Paper.PAPER_TYPE.Data);
            //}
            PlayVoiceNoWait(m_VoiceSource, m_YouMightHave);

            // ===== Left =====
            yield return m_ScriptPrinter.PrintNoAnimEnumarator(m_KeyValue[0].key,
                p => {
                    m_LKey= p;
                    m_LKey.gameObject.SetActive(false);
                }, Paper.PAPER_TYPE.Hashkey);

            yield return m_ScriptPrinter.PrintNoAnimEnumarator(m_KeyValue[0].value,
                (p) => {
                    m_LValue = p;
                    m_LValue.gameObject.SetActive(false);
                }, Paper.PAPER_TYPE.Data);

            // ===== Index 1 =====
            yield return m_ScriptPrinter.PrintNoAnimEnumarator(m_KeyValue[1].key,
                (p) => {
                    m_RKey = p;
                    m_RKey.gameObject.SetActive(false);
                }, Paper.PAPER_TYPE.Hashkey);

            yield return m_ScriptPrinter.PrintNoAnimEnumarator(m_KeyValue[1].value,
                (p) => {
                    m_RValue = p;
                    m_RValue.gameObject.SetActive(false);
                }, Paper.PAPER_TYPE.Data);


            yield return new WaitUntil(() => { return !m_VoiceSource.isPlaying; });



            yield return new WaitUntil(() => 
            {
                return m_LValue != null;
            });

            Debug.Log("adasjkdhaj");

            yield return new WaitUntil(() =>
            {
                return m_LKey != null;
            });

            Debug.Log("adasjkdhaj");

            yield return new WaitUntil(() =>
            {
                return m_RValue != null;
            });

            Debug.Log("adasjkdhaj");
            yield return new WaitUntil(() =>
            {
                return m_RKey != null;
            });


            Debug.Log("adasjkdhaj");


            {
                Debug.Log("PaperCollisionTest");

                bool leftPrinted = false;
                bool rightPrinted = false;

                // ===== LEFT =====
                m_LKey.transform.position = m_KeyValueStartTransform.position + m_HashFuncDevOffset * 0;
                m_LValue.transform.position = m_LKey.transform.position + m_KeyValueSeperation;

                m_LKey.gameObject.SetActive(true);
                m_LValue.gameObject.SetActive(true);

                m_ScriptHashFuncDevInstances[0].OnPaperPrinted.AddListener((p) =>
                {
                    leftPrinted = true;
                });


                // ===== RIGHT =====
                m_RKey.transform.position = m_KeyValueStartTransform.position + m_HashFuncDevOffset * 1;
                m_RValue.transform.position = m_RKey.transform.position + m_KeyValueSeperation;

                m_RKey.gameObject.SetActive(true);
                m_RValue.gameObject.SetActive(true);

                m_ScriptHashFuncDevInstances[1].OnPaperPrinted.AddListener((p) =>
                {
                    rightPrinted = true;
                });


                // Wait for both
                yield return new WaitUntil(() => leftPrinted && rightPrinted);
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

            // LEFT
            if (m_LKey != null)
            {
                Destroy(m_LKey.gameObject);
                m_LKey = null;
            }

            if (m_LValue != null)
            {
                Destroy(m_LValue.gameObject);
                m_LValue = null;
            }

            // RIGHT
            if (m_RKey != null)
            {
                Destroy(m_RKey.gameObject);
                m_RKey = null;
            }

            if (m_RValue != null)
            {
                Destroy(m_RValue.gameObject);
                m_RValue = null;
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
