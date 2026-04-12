using UnityEngine;

namespace Canvas
{
    public class JumpToSlideStep : Step
    {
        [SerializeField] Slides m_DestinationSlide;

        public override void Activate()
        {
            
        }

        public override void Deactivate()
        {
            throw new System.NotImplementedException();
        }

        public override void OnSlideExit()
        {
            throw new System.NotImplementedException();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}