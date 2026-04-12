using System.Collections;
using UnityEngine;

namespace Canvas 
{
    public class LinkedListsGameplayStep : Step
    {
        [Header("References")]
        [SerializeField] private GameObject m_GameplayObjectsPrefab;
        [SerializeField] Transform m_TeleportAnchorTransform;
        [SerializeField] Transform m_TeleportAnchorNewPos;
        [SerializeField] AudioSource m_AudioSource;
        [SerializeField] AudioClip m_AudioClip;

        GameObject m_GameplayObjectsInstance = null;
        Vector3 m_TeleportAnchorStartPos = Vector3.zero;
      
        private void Start()
        {
            Debug.Assert(m_GameplayObjectsPrefab != null);       
            Debug.Assert(m_TeleportAnchorTransform != null);
            Debug.Assert(m_TeleportAnchorNewPos != null);

            m_TeleportAnchorStartPos = m_TeleportAnchorTransform.position;
        }

        public override void Activate()
        {
            if (m_GameplayObjectsInstance != null)
            {
                Destroy(m_GameplayObjectsInstance);
            }



            PlayVoiceNoWait(m_AudioSource, m_AudioClip);

            m_TeleportAnchorTransform.position = m_TeleportAnchorNewPos.position;
            m_GameplayObjectsInstance = Instantiate(m_GameplayObjectsPrefab);
            m_GameplayObjectsInstance.transform.position = Vector3.zero;
        }

        private void Reset()
        {
            if (m_GameplayObjectsInstance != null)
            {
                Destroy(m_GameplayObjectsInstance.gameObject);
            }
            m_TeleportAnchorTransform.position = m_TeleportAnchorStartPos;
        }

        public override void Deactivate()
        {
            Reset();
        }

        public override void OnSlideExit()
        {
            Reset();
        }

    }

}
