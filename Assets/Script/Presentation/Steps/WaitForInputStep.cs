using UnityEngine;

namespace Canvas
{
    public class WaitForInputStep : Step
    {
      
        // Call this function to trigger an input and move to the next step
        public void TriggerInput()
        {
            Complete();
        }

        [SerializeField] Slides m_PrevSlide;
        public void GoToCustomPrevSlide()
        {
            if (m_PrevSlide == null)
            {
                Debug.LogError("Prev Slide is null");
                return;
            }

            m_PrevSlide.manager.JumpToSlide(m_PrevSlide, true);
        }

        public override void Activate()
        {
        }

        public override void Deactivate()
        {
        }

        public override void OnSlideExit()
        {
           
        }
    }

}