using UnityEngine;

namespace Canvas
{
    public class JumpToStepStep : Step
    {
        [SerializeField] Step m_DestinationStep;

        private void Start()
        {
            Debug.Assert(m_DestinationStep);
        }

        public override void Activate()
        {
            slide.JumpToStep(m_DestinationStep);
        }

        public override void Deactivate()
        {
        }

        public override void OnSlideExit()
        {
        }
    }
}
