using UnityEngine;
using UnityEngine.Video;

namespace Canvas
{
    public class LessonChoiceStep : Step
    {
        [SerializeField] GameObject m_ChoicesUIHolder;
        [SerializeField] VideoPlayer m_VideoPlayer;

        [SerializeField] Slides m_HashmapSlide;
        [SerializeField] Slides m_LinkedListsSlide;

        // Called externally via button
        public void ChooseHashmap()
        {
            if (!slide.IsCurrentStep(this))
                return;

            slide.manager.JumpToSlide(m_HashmapSlide);
        }

        // Called externally via button
        public void ChooseLinkedLists()
        {
            if (!slide.IsCurrentStep(this))
                return;

            slide.manager.JumpToSlide(m_LinkedListsSlide);
        }

        private void Start()
        {
            Debug.Assert(m_ChoicesUIHolder != null);
        }

        public override void Activate()
        {
            m_ChoicesUIHolder.SetActive(true);
            m_VideoPlayer.Play();
        }

        public override void Deactivate()
        {
            Reset();
        }

        private void Reset()
        {
            m_ChoicesUIHolder.SetActive(false);
        }

        public override void OnSlideExit()
        {
            Reset();
        }
    }
}