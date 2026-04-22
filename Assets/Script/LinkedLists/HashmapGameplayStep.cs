using UnityEngine;

namespace Canvas
{

    public class HashmapGameplayStep : Step
    {
        [Header("References")]
        //[SerializeField] AudioSource m_VoiceSource;
        [SerializeField] GameObject m_GameplayObjectsPrefab;

        [Header("Voice Overs")]
        //[SerializeField] AudioClip m_YouCanTry;

        GameObject m_GameplayObjectsInstance = null;
        Vector3 m_TeleportAnchorStartPos = Vector3.zero;
        [SerializeField] Transform m_TeleportAnchorTransform;
        [SerializeField] Transform m_TeleportAnchorNewPos;

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


            //PlayVoiceNoWait(m_VoiceSource, m_YouCanTry);

            m_GameplayObjectsInstance = Instantiate(m_GameplayObjectsPrefab);
            m_GameplayObjectsInstance.transform.position = Vector3.zero;

            m_TeleportAnchorTransform.position = m_TeleportAnchorNewPos.position;

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