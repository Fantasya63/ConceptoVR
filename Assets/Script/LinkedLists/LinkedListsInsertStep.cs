using Concepto;
using NUnit.Framework.Constraints;
using System.Collections;
using UnityEngine;

namespace Canvas
{
    public class LinkedListsInsertStep : Step
    {
        [Header("Animation References")]
        [SerializeField] float m_CurrentPtrGrowSpeed = 0.5f;
        [SerializeField] float m_ScriptGrowSpeed = 1.0f;
        [SerializeField] float m_GrowDelaySpeed = 0.2f;

        [Header("References")]
        [SerializeField] AudioSource m_VoiceSource;
        [SerializeField] SpatialLinkedLists m_LinkedListsPrefab;
        [SerializeField] Transform m_LinkedListsStartTransform;
        [SerializeField] GameObject m_PositionalMarkersHolder;
        [SerializeField] ScriptVisualizer m_ScriptVisualizer;

        [Header("Code Samples")]
        [SerializeField]
        [TextArea(5, 20)]
        string m_InsertStartCode;

        [SerializeField]
        [TextArea(5, 20)]
        string m_CurrentPointerCode;

        [SerializeField]
        [TextArea(5, 20)]
        string m_InsertAtTheEndCode;

        [SerializeField]
        [TextArea(5, 20)]
        string m_InsertAtTheMiddleCode;

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
            Debug.Assert(m_ScriptVisualizer != null);
        }

        public override bool OnNextStep()
        {
            m_NextPressed = true;
            return base.OnNextStep();
        }

        public override void Activate()
        {

            if (m_Coroutine != null)
            {
                StopCoroutine(m_Coroutine);
            }

            if (m_LinkedListsInstance != null)
                Destroy(m_LinkedListsInstance.gameObject);

            m_LinkedListsInstance = Instantiate(m_LinkedListsPrefab);
            m_LinkedListsInstance.transform.position = m_LinkedListsStartTransform.position;
            //m_LinkedListsInstance.CurrentPointer.gameObject.SetActive(false);

            m_ScriptVisualizer.gameObject.SetActive(false);

            m_LinkedListsInstance.CurrentPointer.transform.localScale = Vector3.zero;

            m_NextPressed = false;

            m_Coroutine = StartCoroutine(OnRoutine());
        }

        IEnumerator OnRoutine()
        {
            PlayVoiceNoWait(m_VoiceSource, m_ToInsert);

            m_ScriptVisualizer.Code = m_InsertStartCode;
            yield return GrowAndWait(m_ScriptVisualizer.gameObject, m_ScriptGrowSpeed);

            yield return WaitForAudioToFinish(m_VoiceSource);

            yield return m_LinkedListsInstance.Insert("23");

            
            

            PlayVoiceNoWait(m_VoiceSource, m_WeWillAlsoNeedA);

            m_ScriptVisualizer.gameObject.SetActive(false);
            yield return new WaitForSeconds(m_GrowDelaySpeed);
            m_ScriptVisualizer.Code = m_CurrentPointerCode;
            GrowNoWait(m_ScriptVisualizer.gameObject, m_ScriptGrowSpeed);

            m_LinkedListsInstance.CurrentPointer.transform.localScale = Vector3.one;
            yield return GrowAndWait(m_LinkedListsInstance.CurrentPointer.gameObject, m_CurrentPtrGrowSpeed);


            yield return WaitForAudioToFinish(m_VoiceSource);


            PlayVoiceNoWait(m_VoiceSource, m_NowWeCanJustInsertNodes);
            
            m_ScriptVisualizer.gameObject.SetActive(false);
            m_ScriptVisualizer.Code = m_InsertAtTheEndCode;
            GrowNoWait(m_ScriptVisualizer.gameObject, m_ScriptGrowSpeed);


            yield return m_LinkedListsInstance.Insert("54");

            yield return m_LinkedListsInstance.Insert("36");

            yield return WaitForAudioToFinish(m_VoiceSource);

            m_PositionalMarkersHolder.gameObject.SetActive(true);

            m_ScriptVisualizer.gameObject.SetActive(false);
            yield return PlayAndWaitVoice(m_VoiceSource, m_HoweverSuppose);

            PlayVoiceNoWait(m_VoiceSource, m_ToInsertANodeBet);
            yield return new WaitForSeconds(m_GrowDelaySpeed);

            m_ScriptVisualizer.Code = m_InsertAtTheMiddleCode;
            GrowNoWait(m_ScriptVisualizer.gameObject, m_ScriptGrowSpeed);

            //yield return m_LinkedListsInstance.Insert(345, 1);
            yield return m_LinkedListsInstance.Traverse(1, false);

            yield return WaitForAudioToFinish(m_VoiceSource);

            PlayVoiceNoWait(m_VoiceSource, m_ThenWeCreateANewNode);

            yield return m_LinkedListsInstance.InsertAtPosNarrate(345, m_VoiceSource, m_FinallyWeCanSet);


            yield return PlayAndWaitVoice(m_VoiceSource, m_WeHaveJustPreformed);

            m_ScriptVisualizer.gameObject.SetActive(false);
            Complete();
        }

        void CleanUp()
        {
            if (m_Coroutine != null)
            {
                StopCoroutine(m_Coroutine);
                m_Coroutine = null;
            }

            m_PositionalMarkersHolder.SetActive(false);

            if (m_LinkedListsInstance != null)
                Destroy(m_LinkedListsInstance.gameObject);
        }

        public override void Deactivate()
        {
            CleanUp();
        }

        public override void OnSlideExit()
        {
            CleanUp();
        }
    }
}