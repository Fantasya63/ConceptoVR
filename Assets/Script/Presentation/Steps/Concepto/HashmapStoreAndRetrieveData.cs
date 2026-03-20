using System.Collections;
using UnityEngine;
using Concepto.HashMap;
using System.Collections.Generic;
using NUnit.Framework.Constraints;

namespace Canvas
{
    public class HashmapStoreAndRetrieveData : Step
    {
        [Header("References")]
        [SerializeField]
        private GameObject m_BoxPrefab;
        [SerializeField]
        private Printer m_ScriptPrinter;
        [SerializeField]
        private AudioSource m_VoiceSource;

        [SerializeField] private HashFuncDevice m_PlayerHashFuncDev;

        [SerializeField] private Transform m_PaperKeyStartTransform;
        [SerializeField] private Transform m_PaperDataStartTransform;

        [Header("Box Array")]
        [SerializeField] private Transform m_BoxStartTransform;
        [SerializeField] private Vector3 m_BoxOffset = Vector3.forward * -0.1f;
        [SerializeField] private ScriptVisualizer m_ScriptVisualizer;
        
        private string m_PaperData = "56";
        private string m_PaperKey = "RON";

        [Header("Voice Overs")]
        [SerializeField] private AudioClip m_ToStoreAData;
        [SerializeField] private AudioClip m_PerfectNowPlace;
        [SerializeField] private AudioClip m_YouCanNowPlace;
        [SerializeField] private AudioClip m_CongratsInsert;
        [SerializeField] private AudioClip m_WhenYouWant_Next;

        [Header("Animation")]
        private float m_PaperShowdelay = 1.0f;
        [SerializeField] private float m_BoxGrowSpeed = 1.0f;
        [SerializeField] private float m_BoxGrowDelay = 0.1f;

        private Paper m_DataPaperInstance;
        private Paper m_KeyPaperInstance;
        bool m_HasHashedPaper = false;
        bool m_HasPlacedIndex = false;
        bool m_HasPlacedPaper = false;

        private Coroutine m_SlideRoutine;
        private List<GameObject> m_SpawnedPlayerBoxes = new List<GameObject>(HashFunc.NumBoxes);
        private Vector3 m_BoxesInitialScale = Vector3.one;
        private int m_KeyHash;

        void Start()
        {
            Debug.Assert(m_PaperDataStartTransform != null);
            Debug.Assert(m_PaperKeyStartTransform != null);
            Debug.Assert(m_ScriptVisualizer != null);

            if (m_ScriptPrinter == null)
            {
                GameObject go = GameObject.FindGameObjectWithTag("ScriptPrinter");
                if (go != null)
                {
                    m_ScriptPrinter = go.GetComponent<Printer>();
                }
            }

            Debug.Assert(m_ScriptPrinter != null);
            m_KeyHash = HashFunc.Hash(m_PaperKey, HashFunc.NumBoxes);

            m_BoxesInitialScale = m_BoxPrefab.transform.localScale;
        }


        public override void Activate()
        {
            Debug.Assert(m_BoxPrefab != null);
            Debug.Assert(m_VoiceSource != null);
            Debug.Assert(m_ToStoreAData != null);

            m_HasHashedPaper = false;
            m_HasPlacedIndex = false;

            m_PlayerHashFuncDev.OnPaperPrinted.AddListener(OnHashFuncDevPrinted);

            if (m_SlideRoutine != null)
                StopCoroutine(m_SlideRoutine);

            m_SlideRoutine = StartCoroutine(SlideRoutine());
        }


        void OnHashFuncDevPrinted(Paper paper)
        {
            if (m_HasHashedPaper)
                return;

            m_HasHashedPaper = true;
        }

        IEnumerator SlideRoutine()
        {
            PlayVoiceNoWait(m_VoiceSource, m_ToStoreAData);
            Rigidbody datarbody = null;
            Rigidbody keyrbody = null;

            m_ScriptPrinter.PrintNoAnim(m_PaperKey, p =>
            {
                m_DataPaperInstance = p;

                datarbody = p.GetComponent<Rigidbody>();
                Debug.Assert(datarbody != null);

                datarbody.isKinematic = true;

                m_DataPaperInstance.gameObject.SetActive(false);
                
            }, Paper.PAPER_TYPE.Data);


            yield return new WaitForSeconds(m_PaperShowdelay);

            m_ScriptPrinter.PrintNoAnim(m_PaperData, p =>
            {
                m_KeyPaperInstance = p;

                keyrbody = p.GetComponent<Rigidbody>();
                Debug.Assert(keyrbody != null);
                keyrbody.isKinematic = true;


                m_KeyPaperInstance.gameObject.SetActive(false);

                Debug.Log("Init PaperKey");
            }, Paper.PAPER_TYPE.Data);

            yield return new WaitUntil(() => { return m_KeyPaperInstance != null && m_DataPaperInstance != null; });

            // show Papers
            m_KeyPaperInstance.transform.position = m_PaperKeyStartTransform.position;
            m_DataPaperInstance.transform.position = m_PaperDataStartTransform.position;
            
            m_KeyPaperInstance.transform.rotation = m_PaperKeyStartTransform.rotation;
            m_DataPaperInstance.transform.rotation = m_PaperDataStartTransform.rotation;

            m_KeyPaperInstance.gameObject.SetActive(true);
            m_DataPaperInstance.gameObject.SetActive(true);

            Debug.Log("Showed Papers");

            for (int i = 0; i < HashFunc.NumBoxes; i++)
            {
                GameObject box = Instantiate(m_BoxPrefab);
                box.transform.position = m_BoxStartTransform.position + m_BoxOffset * i;
                box.transform.localScale = Vector3.zero;

                BoxEvents boxEvents = box.GetComponent<BoxEvents>();
                Debug.Assert(boxEvents != null);

                boxEvents.Visualizer = m_ScriptVisualizer;
                boxEvents.SetIndex(i);
                
                if (i == m_KeyHash)
                {
                    boxEvents.OnDataInserted.AddListener(OnDataInserted);
                    boxEvents.OnDataRemoved.AddListener(OnDataRemoved);

                    boxEvents.OnIndexInserted.AddListener(OnIndexInserted);
                    boxEvents.OnIndexRemoved.AddListener(OnIndexRemoved);
                }
                m_SpawnedPlayerBoxes.Add(box);

                box.transform.LeanScale(m_BoxesInitialScale, m_BoxGrowSpeed);

                yield return new WaitForSeconds(m_BoxGrowDelay);
            }

            yield return new WaitUntil(() => { return m_VoiceSource.isPlaying == false; });
            
            datarbody.isKinematic = false;
            keyrbody.isKinematic = false;

            yield return new WaitUntil(() => m_HasHashedPaper);

            PlayVoiceNoWait(m_VoiceSource, m_PerfectNowPlace);

            yield return new WaitUntil(() => m_HasPlacedIndex);

            yield return PlayAndWaitVoice(m_VoiceSource, m_YouCanNowPlace);

            yield return new WaitUntil(() => {return m_HasPlacedPaper == true && m_HasPlacedIndex == false;});

            yield return PlayAndWaitVoice(m_VoiceSource, m_CongratsInsert);

            PlayVoiceNoWait(m_VoiceSource, m_WhenYouWant_Next);
        }


        public override void Deactivate()
        {
            if (m_SlideRoutine != null)
                StopCoroutine(m_SlideRoutine);

            m_PlayerHashFuncDev.OnPaperPrinted.RemoveListener(OnHashFuncDevPrinted);
            
            CleanUp();
        }

        public override void OnSlideExit()
        {
        }

        void CleanUp()
        {
            Destroy(m_DataPaperInstance.gameObject);
            Destroy(m_KeyPaperInstance.gameObject);

            if (m_SpawnedPlayerBoxes != null)
            {
                for (int i = 0; i < m_SpawnedPlayerBoxes.Count; i++)
                {
                    Destroy(m_SpawnedPlayerBoxes[i].gameObject);
                }
            }

            m_SpawnedPlayerBoxes.Clear();
        }

        public void OnDestroy()
        {
            CleanUp();
        }

        void OnIndexInserted(Paper paper)
        {
            Debug.Log("Index Paper Inserted");
            m_HasPlacedIndex = true;
        }

        void OnDataInserted(Paper paper)
        {
            m_HasPlacedPaper = true;
        }

        void OnDataRemoved(Paper paper)
        {
            m_HasPlacedPaper = false;
        }

        void OnIndexRemoved(Paper paper)
        {
            Debug.Log("Index Paper Removed");
            m_HasPlacedIndex = false;
        }

    }

}