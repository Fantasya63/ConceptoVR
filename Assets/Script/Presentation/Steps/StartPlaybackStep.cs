using UnityEngine;

namespace Canvas
{
    public class StartPlaybackStep : Step
    {
        [SerializeField]
        private MessagePlaybackBar m_PlaybackBar;

        [SerializeField]
        private AudioClip m_audioClip;

        public override void Activate()
        {
            Debug.Assert(m_PlaybackBar != null);
            Debug.Assert(m_audioClip != null);

            m_PlaybackBar.StartPlayback(m_audioClip.length);
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
