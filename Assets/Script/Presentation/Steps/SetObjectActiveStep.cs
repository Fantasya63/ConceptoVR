using UnityEngine;

namespace Canvas 
{
    public class SetObjectActiveStep : Step
    {
        [SerializeField]
        private GameObject m_gameObject;

        public override void Activate()
        {
            Debug.Assert(m_gameObject != null);
            m_gameObject.SetActive(true);
            Debug.Log($"Set GameObject: {m_gameObject.name} to Active");
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

