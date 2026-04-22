using Concepto.HashMap;
using NUnit;
using System.Collections;
using UnityEngine;

namespace Canvas
{
    public class HashFuncExplanationStep : Step
    {
        [Header("References")]
        [SerializeField] private HashFuncDevice m_HashFuncDevPrefab;
        [SerializeField] private HashFuncDevice m_PlayerHashFuncDev;
        [SerializeField] private GameObject m_PlayerKeypadObject;

        [SerializeField] private Transform m_HashFuncDevSpawnTransform;
        [SerializeField] private Transform m_PaperStartTransform;
        [SerializeField] private Transform m_PaperEndTransform;
        [SerializeField] private AudioSource m_VOSource;
        [SerializeField] private Printer m_Printer;

        [Header("Voice Overs")]
        [SerializeField] private AudioClip m_VOTheMagic;
        [SerializeField] private AudioClip m_GiveItAKey;

        [Header("Animation")]
        [SerializeField] private float m_HashFuncDevAppearTime = 1.0f;
        [SerializeField] private float m_PaperInsertDur = 1.0f;
        [SerializeField] private float m_GiveItAKeyDelay = 4.5f;

        [SerializeField] private LeanTweenType m_HashFuncDevTweenType = LeanTweenType.easeInOutQuad;
        [SerializeField] private LeanTweenType m_HashFuncDevMoveTweenType = LeanTweenType.easeInOutQuad;
        [SerializeField] private float m_HashFuncDevMoveDur = 1.0f;

        [SerializeField] private float m_BoxGrowSpeed = 1.0f;
        [SerializeField] private float m_BoxGrowDelay = 0.1f;


        private HashFuncDevice m_HashFuncDevInstance;
        private Vector3 m_HashFuncDevInitLocalScale;
        private Paper m_PaperInstance;
        private Paper m_OutputHashKeyPaper = null;
        private Coroutine m_SlideRoutine;

        public void Awake()
        {
            Debug.Assert(m_HashFuncDevPrefab != null);
            Debug.Assert(m_HashFuncDevSpawnTransform != null);
            Debug.Assert(m_PlayerHashFuncDev != null);

            m_HashFuncDevInstance = Instantiate(m_HashFuncDevPrefab);
            m_HashFuncDevInitLocalScale = m_HashFuncDevInstance.transform.localScale;
            m_HashFuncDevInstance.gameObject.SetActive(false);
            m_HashFuncDevInstance.OnPaperPrinted.AddListener((Paper paper) => { m_OutputHashKeyPaper = paper; });

            m_PlayerHashFuncDev.gameObject.SetActive(false);


           
        }



        public override void Activate()
        {
            if (m_HashFuncDevInstance != null)
                Destroy(m_HashFuncDevInstance.gameObject);

            m_HashFuncDevInstance = Instantiate(m_HashFuncDevPrefab);
            m_HashFuncDevInitLocalScale = m_HashFuncDevInstance.transform.localScale;
            m_HashFuncDevInstance.gameObject.SetActive(false);
            m_HashFuncDevInstance.OnPaperPrinted.AddListener((Paper paper) => { m_OutputHashKeyPaper = paper; });

            if (m_SlideRoutine != null)
                StopCoroutine(m_SlideRoutine);

            m_SlideRoutine = StartCoroutine(OnExplanationRoutine());
        }

        IEnumerator OnExplanationRoutine()
        {
            if (m_PaperInstance == null)
            {
                m_Printer.PrintNoAnim("ABC", p =>
                {
                    m_PaperInstance = p;
                    m_PaperInstance.gameObject.SetActive(false);
                }, Paper.PAPER_TYPE.Data);
                yield return new WaitUntil(() => { return m_PaperInstance != null; });
            }

            m_HashFuncDevInstance.transform.localScale = Vector3.zero;
            m_HashFuncDevInstance.transform.position = m_HashFuncDevSpawnTransform.position;
            m_HashFuncDevInstance.transform.rotation = m_HashFuncDevSpawnTransform.rotation;

            m_PlayerHashFuncDev.gameObject.SetActive(false);
            m_PlayerKeypadObject.SetActive(false);

            {
                bool lerpComplete = false;
                m_HashFuncDevInstance.gameObject.SetActive(true);
                m_HashFuncDevInstance.transform.LeanScale(m_HashFuncDevInitLocalScale, m_HashFuncDevAppearTime)
                    .setOnComplete(() => lerpComplete = true);

                //PlayVoiceNoWait(m_VOSource, m_VOTheMagic);
                yield return PlayAndWaitVoice(m_VOSource, m_VOTheMagic);
                yield return new WaitUntil(() => lerpComplete == true);

            }

            {
                m_PaperInstance.gameObject.SetActive(true);
               
                Rigidbody rbody = m_PaperInstance.GetComponent<Rigidbody>();
                rbody.MovePosition(m_PaperStartTransform.position);

                yield return new WaitForSeconds(m_GiveItAKeyDelay);
                yield return PlayAndWaitVoice(m_VOSource, m_GiveItAKey);

                yield return new WaitUntil(() => { return m_OutputHashKeyPaper != null; });
                Destroy(m_OutputHashKeyPaper.gameObject);
                m_OutputHashKeyPaper = null;
            }

            // Move Demo PlayerHashFuncDev
            {
                bool finished = false;
                bool isRotFinished = false;

                m_HashFuncDevInstance.transform.LeanMove(m_PlayerHashFuncDev.transform.position, m_HashFuncDevMoveDur)
                    .setEase(m_HashFuncDevMoveTweenType)
                    .setOnComplete(() => finished = true);

                m_HashFuncDevInstance.transform.LeanRotate(m_PlayerHashFuncDev.transform.rotation.eulerAngles, m_HashFuncDevMoveDur)
                    .setEase(m_HashFuncDevMoveTweenType)
                    .setOnComplete(() => isRotFinished = true);

                yield return new WaitUntil(() => finished && isRotFinished);
                m_HashFuncDevInstance.gameObject.SetActive(false);
                m_PlayerHashFuncDev.gameObject.SetActive(true);
                m_PlayerKeypadObject.gameObject.SetActive(true);
            }
            Complete();
        }

        void _Reset()
        {
            if (m_HashFuncDevInstance != null)
            {
                if (LeanTween.isTweening(m_HashFuncDevInstance.gameObject))
                {
                    // STop
                    LeanTween.cancel(m_HashFuncDevInstance.gameObject);
                }
            }
            

            // Hide Hash func object and paper
            if (m_HashFuncDevInstance != null)
                Destroy(m_HashFuncDevInstance.gameObject);

            if (m_PaperInstance != null)
                Destroy(m_PaperInstance.gameObject);
                //m_PaperInstance.gameObject.SetActive(false);

            if (m_SlideRoutine != null)
                StopCoroutine(m_SlideRoutine);
        }

        public override void Deactivate()
        {
           _Reset();
        }

        public override void OnSlideExit()
        {
            _Reset();
        }
    }

}
