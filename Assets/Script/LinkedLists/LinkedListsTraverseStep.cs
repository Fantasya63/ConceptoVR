using Concepto;
using NUnit.Framework.Constraints;
using System.Collections;
using UnityEngine;

namespace Canvas
{

    public class LinkedListsTraverseStep : Step
    {
        [Header("References")]
        [SerializeField] AudioSource m_VoiceSource;
        [SerializeField] SpatialLinkedLists m_LinkedListsPrefab;
        [SerializeField] Transform m_LinkedListsStartTransform;

        [Header("Voice Overs")]
        [SerializeField] AudioClip m_ToTraverse;
        [SerializeField] AudioClip m_ThisConcludes;

        [Header("Code Equivalents")]
        [SerializeField] ScriptVisualizer m_ScriptVisualizer;
        [SerializeField] float m_ScriptGrowSpeed = 1.0f;

        [SerializeField]
        [TextArea(5, 20)]
        string m_TraverseCode;


        [Header("Options")]
        [SerializeField] int[] m_StartingValues = { 23, 54, 36 };
        [SerializeField] float m_DelayDurTraverse = 1.0f;

        SpatialLinkedLists m_LinkedListsInstance = null;
        Coroutine m_Coroutine = null;
        public override void Activate()
        {
            CleanUP();

            m_LinkedListsInstance = Instantiate(m_LinkedListsPrefab, m_LinkedListsStartTransform.position, m_LinkedListsStartTransform.rotation, transform);
            m_LinkedListsInstance.InitWithValues(m_StartingValues);

            m_Coroutine = StartCoroutine(OnRoutine());

            m_ScriptVisualizer.gameObject.SetActive(false);
        }

        IEnumerator OnRoutine()
        {
            m_ScriptVisualizer.gameObject.SetActive(true);
            PlayVoiceNoWait(m_VoiceSource, m_ToTraverse);

            yield return new WaitForSeconds(0.1f);
            m_ScriptVisualizer.Code = m_TraverseCode;
            yield return GrowAndWait(m_ScriptVisualizer.gameObject, m_ScriptGrowSpeed);

            //yield return new WaitForSeconds(m_DelayDurTraverse);

            yield return m_LinkedListsInstance.Traverse();

            yield return WaitForAudioToFinish(m_VoiceSource);

            yield return PlayAndWaitVoice(m_VoiceSource, m_ThisConcludes);

            Complete();
        }


        void CleanUP()
        {
            if (m_Coroutine != null)
            {
                StopCoroutine(m_Coroutine);
                m_Coroutine = null;
            }

            if (m_VoiceSource.isPlaying)
            {
                m_VoiceSource.Stop();
            }

            if (m_LinkedListsInstance != null)
            {
                Destroy(m_LinkedListsInstance.gameObject);
                m_LinkedListsInstance = null;
            }

            m_ScriptVisualizer.gameObject.SetActive(false);
        }

        public override void Deactivate()
        {
            CleanUP();
        }

        public override void OnSlideExit()
        {
            CleanUP();
        }
    }

}