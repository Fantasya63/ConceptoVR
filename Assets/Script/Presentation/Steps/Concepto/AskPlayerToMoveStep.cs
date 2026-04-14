using UnityEngine;

namespace Canvas
{

    public class AskPlayerToMoveStep : Step
    {
        [SerializeField] GameObject m_LessonObjects;
        [SerializeField] GameObject m_FurnitureObjecst;
        [SerializeField] ScriptVisualizer m_ScriptVisualizer;
        [SerializeField] AudioSource m_VoiceSource;
        [SerializeField] AudioClip m_PleaseMoveToTheDesignated;



        public override void Activate()
        {
            m_FurnitureObjecst.SetActive(false);
            m_LessonObjects.SetActive(true);
            m_ScriptVisualizer.gameObject.SetActive(true);

            m_VoiceSource.clip = m_PleaseMoveToTheDesignated;
            m_VoiceSource.Play();
        }

        public override void Deactivate()
        {
        }

       

        public override void OnSlideExit()
        {
            m_FurnitureObjecst.SetActive(true);
            m_LessonObjects.SetActive(false);
            m_ScriptVisualizer.gameObject.SetActive(false);
        }
    }

}
