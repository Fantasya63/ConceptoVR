using UnityEngine;

namespace Canvas
{
    public class PlaySoundStep : Step
    {
        [SerializeField]
        private AudioSource m_AudioSource;

        public override void Activate()
        {
            Debug.Assert(m_AudioSource != null);
            m_AudioSource.Play();
            Complete();
        }

        public override void Deactivate()
        {
        }

        public override void OnSlideExit()
        {
            if (m_AudioSource != null)
            {
                m_AudioSource.Stop();
            }
        }

    }

}