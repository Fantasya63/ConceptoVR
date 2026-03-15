using System.Collections;
using UnityEngine;
using Concepto.HashMap;
using System.Collections.Generic;

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
        
        private string m_KeyData = "56";
        private string m_PaperData = "RON";

        [Header("Voice Overs")]
        [SerializeField] private AudioClip m_ToStoreAData;
        [SerializeField] private AudioClip m_PerfectNowPlace;
        [SerializeField] private AudioClip m_YouCanNowPlace;
        [SerializeField] private AudioClip m_CongratsInsert;

        [Header("Animation")]
        private float m_PaperShowdelay = 1.0f;
        [SerializeField] private float m_BoxGrowSpeed = 1.0f;
        [SerializeField] private float m_BoxGrowDelay = 0.1f;

        private Paper m_DataPaperInstance;
        private Paper m_KeyPaperInstance;
        bool m_HasHashedPaper = false;
        bool m_HasPlacedIndex = false;

        private Coroutine m_SlideRoutine;
        private List<GameObject> m_SpawnedPlayerBoxes = new List<GameObject>(HashFunc.NumBoxes);
        private Vector3 m_BoxesInitialScale = Vector3.one;

        void Start()
        {
            Debug.Assert(m_PaperDataStartTransform != null);
            Debug.Assert(m_PaperKeyStartTransform != null);

            if (m_ScriptPrinter == null)
            {
                GameObject go = GameObject.FindGameObjectWithTag("ScriptPrinter");
                if (go != null)
                {
                    m_ScriptPrinter = go.GetComponent<Printer>();
                }
            }

            Debug.Assert(m_ScriptPrinter != null);

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

            m_ScriptPrinter.PrintNoAnim(m_PaperData, p =>
            {
                m_DataPaperInstance = p;

                Rigidbody rbody = p.GetComponent<Rigidbody>();
                Debug.Assert(rbody != null);

                rbody.isKinematic = true;

                m_DataPaperInstance.gameObject.SetActive(false);
                
            }, Paper.PAPER_TYPE.Data);


            yield return new WaitForSeconds(m_PaperShowdelay);

            m_ScriptPrinter.PrintNoAnim(m_KeyData, p =>
            {
                m_KeyPaperInstance = p;

                Rigidbody rbody = p.GetComponent<Rigidbody>();
                Debug.Assert(rbody != null);
                rbody.isKinematic = true;


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

                m_SpawnedPlayerBoxes.Add(box);

                box.transform.LeanScale(m_BoxesInitialScale, m_BoxGrowSpeed);

                yield return new WaitForSeconds(m_BoxGrowDelay);
            }

            yield return new WaitUntil(() => m_HasHashedPaper);

            PlayVoiceNoWait(m_VoiceSource, m_PerfectNowPlace);

            yield return new WaitUntil(() => m_HasPlacedIndex);
        }

        public override void Deactivate()
        {
            if (m_SlideRoutine != null)
                StopCoroutine(m_SlideRoutine);

            m_PlayerHashFuncDev.OnPaperPrinted.RemoveListener(OnHashFuncDevPrinted);
            StopCoroutine(m_SlideRoutine);
        }

        public override void OnSlideExit()
        {
        }

        public void OnDestroy()
        {
            Destroy(m_KeyPaperInstance);
            Destroy(m_KeyPaperInstance);
        }
    }

}