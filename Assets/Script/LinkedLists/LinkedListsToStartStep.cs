using Concepto;
using System.Collections;
using UnityEngine;

namespace Canvas
{
    public class LinkedListsToStartStep : Step
    {
        [Header("Anim References")]
        [SerializeField] SpatialPointer m_DisplayHeader;
        [SerializeField] float m_HeaderGrowDur = 1.0f;

        [Header("References")]
        [SerializeField] AudioSource m_VoiceSource;

        [Header("Voice Overs")]
        [SerializeField] AudioClip m_ToStartTheLinkedLists;

        Coroutine m_Coroutine;

        private void Awake()
        {
            Debug.Assert(m_VoiceSource != null);
            Debug.Assert(m_DisplayHeader != null);
        }

        public override void Activate()
        {
            if (m_Coroutine != null)
            {
                StopCoroutine(m_Coroutine);
            }

            m_Coroutine = StartCoroutine(OnRoutine());
        }

        IEnumerator OnRoutine()
        {
            PlayVoiceNoWait(m_VoiceSource, m_ToStartTheLinkedLists);

            m_DisplayHeader.gameObject.SetActive(true);
            m_DisplayHeader.transform.localScale = Vector3.zero;

            m_DisplayHeader.transform.LeanScale(Vector3.one, m_HeaderGrowDur);

            yield return new WaitForSeconds(m_HeaderGrowDur);


            yield return new WaitUntil(() => !m_VoiceSource.isPlaying);

            Complete();
        }

        void _Reset()
        {
            if (m_VoiceSource.isPlaying)
            {
                m_VoiceSource.Stop();
            }
            m_DisplayHeader.gameObject.SetActive(false);
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