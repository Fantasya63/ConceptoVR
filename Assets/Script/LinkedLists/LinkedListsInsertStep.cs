using Concepto;
using System.Collections;
using UnityEngine;

namespace Canvas
{
    public class LinkedListsInsertStep : Step
    {
        [Header("Animation References")]
        [SerializeField] float m_CurrentPtrGrowSpeed = 0.5f;

        [Header("References")]
        [SerializeField] AudioSource m_VoiceSource;
        [SerializeField] SpatialLinkedLists m_LinkedListsPrefab;
        [SerializeField] Transform m_LinkedListsStartTransform;
        [SerializeField] GameObject m_PositionalMarkersHolder;

        [Header("Voice Overs")]
        [SerializeField] AudioClip m_ToInsert;
        [SerializeField] AudioClip m_WeWillAlsoNeedA;
        [SerializeField] AudioClip m_NowWeCanJustInsertNodes;
        [SerializeField] AudioClip m_HoweverSuppose;
        [SerializeField] AudioClip m_ToInsertANodeBet;
        [SerializeField] AudioClip m_ThenWeCreateANewNode;
        [SerializeField] AudioClip m_FinallyWeCanSet;
        [SerializeField] AudioClip m_WeHaveJustPreformed;


        Coroutine m_Coroutine;
        SpatialLinkedLists m_LinkedListsInstance = null;

        bool m_NextPressed = false;

        private void Awake()
        {
            m_PositionalMarkersHolder.SetActive(false);

            AllowNext = false;
            m_NextPressed = false;

            Debug.Assert(m_VoiceSource != null);
            Debug.Assert(m_ToInsert != null);
            Debug.Assert(m_WeWillAlsoNeedA != null);
            Debug.Assert(m_NowWeCanJustInsertNodes != null);
            Debug.Assert(m_PositionalMarkersHolder != null);
            Debug.Assert(m_HoweverSuppose != null);
            Debug.Assert(m_ToInsertANodeBet != null);
            Debug.Assert(m_ThenWeCreateANewNode != null);
            Debug.Assert(m_WeHaveJustPreformed != null);
            Debug.Assert(m_FinallyWeCanSet != null);
        }

        public override bool OnNextStep()
        {
            m_NextPressed = true;
            return base.OnNextStep();
        }

        public override void Activate()
        {
            AllowNext = false;

            if (m_Coroutine != null)
            {
                StopCoroutine(m_Coroutine);
            }

            if (m_LinkedListsInstance != null)
                Destroy(m_LinkedListsInstance.gameObject);

            m_LinkedListsInstance = Instantiate(m_LinkedListsPrefab);
            m_LinkedListsInstance.transform.position = m_LinkedListsStartTransform.position;
            //m_LinkedListsInstance.CurrentPointer.gameObject.SetActive(false);


            m_LinkedListsInstance.CurrentPointer.transform.localScale = Vector3.zero;

            m_NextPressed = false;

            m_Coroutine = StartCoroutine(OnRoutine());
        }

        IEnumerator OnRoutine()
        {
            PlayVoiceNoWait(m_VoiceSource, m_ToInsert);

            yield return WaitForAudioToFinish(m_VoiceSource);

            yield return m_LinkedListsInstance.Insert("23");

            yield return new WaitUntil(() => m_NextPressed);
            m_NextPressed = false;

            PlayVoiceNoWait(m_VoiceSource, m_WeWillAlsoNeedA);

            m_LinkedListsInstance.CurrentPointer.transform.localScale = Vector3.one;
            yield return GrowAndWait(m_LinkedListsInstance.CurrentPointer.gameObject, m_CurrentPtrGrowSpeed);

            yield return WaitForAudioToFinish(m_VoiceSource);

            PlayVoiceNoWait(m_VoiceSource, m_NowWeCanJustInsertNodes);

            yield return m_LinkedListsInstance.Insert("54");

            yield return m_LinkedListsInstance.Insert("36");

            yield return WaitForAudioToFinish(m_VoiceSource);

            m_PositionalMarkersHolder.gameObject.SetActive(true);
            yield return PlayAndWaitVoice(m_VoiceSource, m_HoweverSuppose);

            PlayVoiceNoWait(m_VoiceSource, m_ToInsertANodeBet);
            //yield return m_LinkedListsInstance.Insert(345, 1);
            yield return m_LinkedListsInstance.Traverse(1, false);

            yield return WaitForAudioToFinish(m_VoiceSource);

            PlayVoiceNoWait(m_VoiceSource, m_ThenWeCreateANewNode);

            yield return m_LinkedListsInstance.InsertAtPosNarrate(345, m_VoiceSource, m_FinallyWeCanSet);


            yield return PlayAndWaitVoice(m_VoiceSource, m_WeHaveJustPreformed);


            Complete();
        }

        public override void Deactivate()
        {
            if (m_Coroutine != null)
            {
                StopCoroutine(m_Coroutine);
                m_Coroutine = null;
            }

            m_PositionalMarkersHolder.SetActive(false);
        }

        public override void OnSlideExit()
        {
            Deactivate();
        }
    }
}