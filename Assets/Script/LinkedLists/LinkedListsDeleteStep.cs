using Concepto;
using System.Collections;
using UnityEngine;

namespace Canvas
{
    public class LinkedListsDeleteStep : Step
    {
        [Header("References")]
        [SerializeField] AudioSource m_VoiceSource;
        [SerializeField] SpatialLinkedLists m_LinkedListsPrefab;
        [SerializeField] Transform m_LinkedListsStartTransform;

        [Header("Voice Overs")]
        [SerializeField] AudioClip m_NowSupposeWeWantToDel;
        [SerializeField] AudioClip m_NextWeCreateATemp;
        [SerializeField] AudioClip m_AfterThatWeSetTheCur;
        [SerializeField] AudioClip m_ThenWeCanSafelyDel;

        [Header("Anim")]
        [SerializeField] float m_GrowDur = 0.5f;

        [Header("Code Samples")]
        [SerializeField] ScriptVisualizer m_ScriptVisualizer;

        [SerializeField]
        [TextArea(5, 20)]
        string m_DeleteTraverseCode;

        [SerializeField]
        [TextArea(5, 20)]
        string m_TempToNodeToDeleteCode;

        [SerializeField]
        [TextArea(5, 20)]
        string m_CurrNodeNextToNodetoRepCode;

        [SerializeField]
        [TextArea(5, 20)]
        string m_DeleteNodeStep;

        [Header("Options")]
        [SerializeField] int[] m_StartingValues = { 23, 54, 345, 36 };

        SpatialLinkedLists m_LinkedListsInstance = null;
        Coroutine m_Coroutine = null;

        private void Start()
        {
            Debug.Assert(m_LinkedListsStartTransform != null);
            Debug.Assert(m_StartingValues.Length >= 4);
            Debug.Assert(m_VoiceSource != null);
            Debug.Assert( m_NowSupposeWeWantToDel != null);
            Debug.Assert( m_NextWeCreateATemp != null);
            Debug.Assert( m_AfterThatWeSetTheCur != null);
            Debug.Assert( m_ThenWeCanSafelyDel != null);
            Debug.Assert(m_ScriptVisualizer != null);
        }

        public override void Activate()
        {
            CleanUP();

            m_LinkedListsInstance = Instantiate(m_LinkedListsPrefab, m_LinkedListsStartTransform.position, m_LinkedListsStartTransform.rotation, transform );
            m_LinkedListsInstance.InitWithValues(m_StartingValues);

            m_Coroutine = StartCoroutine(OnRoutine());

        }

        IEnumerator OnRoutine()
        {
            yield return m_LinkedListsInstance.DeleteWithNarration(
                1,
                m_VoiceSource,
                m_NowSupposeWeWantToDel,
                m_NextWeCreateATemp,
                m_AfterThatWeSetTheCur,
                m_ThenWeCanSafelyDel,
                m_ScriptVisualizer,
                m_DeleteTraverseCode,
                m_TempToNodeToDeleteCode,
                m_CurrNodeNextToNodetoRepCode,
                m_DeleteNodeStep
                );

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
