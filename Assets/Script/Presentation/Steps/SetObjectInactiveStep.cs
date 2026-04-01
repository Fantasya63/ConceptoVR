using UnityEngine;

namespace Canvas 
{
    public class SetObjectInactiveStep : Step
    {
        [SerializeField]
        private GameObject m_gameObject;

        public override void Activate()
        {
            Debug.Assert(m_gameObject != null);
            m_gameObject.SetActive(false);
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

