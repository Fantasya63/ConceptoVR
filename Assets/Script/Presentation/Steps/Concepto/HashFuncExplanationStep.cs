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



        private HashFuncDevice m_HashFuncDevInstance;
        private Vector3 m_HashFuncDevInitLocalScale;
        private Paper m_PaperInstance;

        public void Awake()
        {
            Debug.Assert(m_HashFuncDevPrefab != null);
            Debug.Assert(m_HashFuncDevSpawnTransform != null);

            m_HashFuncDevInstance = Instantiate(m_HashFuncDevPrefab);
            m_HashFuncDevInitLocalScale = m_HashFuncDevInstance.transform.localScale;
            m_HashFuncDevInstance.gameObject.SetActive(false);

            m_Printer.PrintNoAnim("ABC", p =>
            {
                m_PaperInstance = p;
                m_PaperInstance.gameObject.SetActive(false);
            });
        }



        public override void Activate()
        {
            StartCoroutine(OnExplanationRoutine());
        }

        IEnumerator OnExplanationRoutine()
        {
            m_HashFuncDevInstance.transform.localScale = Vector3.zero;
            m_HashFuncDevInstance.transform.position = m_HashFuncDevSpawnTransform.position;
            m_HashFuncDevInstance.transform.rotation = m_HashFuncDevSpawnTransform.rotation;

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

            }


        }

        public override void Deactivate()
        {
            
        }

        public override void OnSlideExit()
        {
            
        }
    }

}
