using UnityEngine;

namespace Canvas
{
    public class StartPlaybackStep : Step
    {
        [SerializeField]
        private MessagePlaybackBar m_PlaybackBar;

        [SerializeField]
        private AudioClip m_AudioClip;

        public override void Activate()
        {
            Debug.Assert(m_PlaybackBar != null);
            Debug.Assert(m_AudioClip != null);

            m_PlaybackBar.StartPlayback(m_AudioClip.length);
            Complete();
        }

        public override void Deactivate()
        {
        }

        public override void OnSlideExit()
        {
        }
    }

}
