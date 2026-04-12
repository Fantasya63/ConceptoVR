using UnityEngine;

namespace Canvas
{
    public class JumpToSlideStep : Step
    {
        [SerializeField] Slides m_DestinationSlide;

        public override void Activate()
        {
            slide.manager.JumpToSlide(m_DestinationSlide);
        }

        public override void Deactivate()
        {
            
        }

        public override void OnSlideExit()
        {
            
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Debug.Assert(m_DestinationSlide != null);
        }

    }

}