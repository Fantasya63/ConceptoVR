using System.Collections;
using UnityEngine;
using UnityEngine.Video;

namespace Canvas
{
    public class GoodMorningStep : Step
    {
        [Header("References")]
        [SerializeField] VideoPlayer m_TVPlayer;
        [SerializeField] GameObject m_MenuObject;
        [SerializeField] GameObject m_RepOrContinueControls;
        [SerializeField] MessagePlaybackBar m_PlaybackBar;

        [SerializeField] AudioSource m_VoiceSource;

        [Header("Audio Clip")]
        [SerializeField] AudioClip m_MessageClip;

        Coroutine m_Coroutine;
        bool m_ContinueBtnPressed = false;


        private void Start()
        {
            Debug.Assert(m_TVPlayer != null);
            Debug.Assert(m_MenuObject != null);
            Debug.Assert(m_PlaybackBar != null);
            Debug.Assert(m_RepOrContinueControls != null);
            Debug.Assert(m_VoiceSource != null);
            Debug.Assert(m_MessageClip != null);
        }

        public override void Activate()
        {
            if (m_Coroutine != null)
                StopCoroutine(m_Coroutine);

            m_ContinueBtnPressed = false;
            m_Coroutine = StartCoroutine(OnSlideRoutine());

        }

        public void ContinueSlide()
        {
            Debug.Log("Continue");
            m_ContinueBtnPressed = true;
        }

        public void ReplaySlide()
        {
            if (m_Coroutine != null)
                StopCoroutine(m_Coroutine);

            _Reset();

            m_Coroutine = StartCoroutine(OnSlideRoutine());
        }

        IEnumerator OnSlideRoutine()
        {
            m_RepOrContinueControls.SetActive(false);
            m_TVPlayer.Play();
            m_MenuObject.SetActive(true);
            m_PlaybackBar.StartPlayback(m_MessageClip.length);

            yield return PlayAndWaitVoice(m_VoiceSource, m_MessageClip);

            m_RepOrContinueControls.SetActive(true);
            yield return new WaitUntil(() =>
            {
                Debug.Log($"Waiting Until True: {m_ContinueBtnPressed}");
                return m_ContinueBtnPressed;
            });
            m_TVPlayer.Stop();
            m_RepOrContinueControls.SetActive(false);
            m_MenuObject.SetActive(false);
            Complete();
        }

        private void _Reset()
        {
            if (m_Coroutine != null)
                StopCoroutine(m_Coroutine);

            if (m_VoiceSource.isPlaying)
            {
                m_VoiceSource.Stop();
            }

            m_ContinueBtnPressed = false;
            m_TVPlayer.Stop();
            m_RepOrContinueControls.SetActive(false);
            m_MenuObject.SetActive(false);
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
