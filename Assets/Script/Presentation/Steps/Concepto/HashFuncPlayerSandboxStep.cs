using Concepto.HashMap;
using System.Collections;
using UnityEngine;

namespace Canvas
{
    public class HashFuncPlayerSandboxStep : Step
    {
        [Header("References")]
        [SerializeField] private AudioSource m_VOSource;
        [SerializeField] private KeyboardController m_KeyboardController;
        [SerializeField] private HashFuncDevice m_HashFuncDevice;

        [Header("Starting state References")]
        [SerializeField] private KeyboardController m_Keypad;
        [SerializeField] private HashFuncDevice m_DemoHashFuncDev;
        [SerializeField] private HashFuncDevice m_PlayerHashFuncDev;


        [Header("Voice Overs")]
        [SerializeField] private AudioClip m_TryItKeypadClip;
        [SerializeField] private AudioClip m_ShowCodeViz;

        bool m_HasToldHowToViz = false;


        private void Awake()
        {
            Debug.Assert(m_VOSource != null);
            m_KeyboardController.OnSubmit.AddListener(OnKeypadSubmit);
        }



        Coroutine m_StepCoroutine;
        public override void Activate()
        {
            if (m_StepCoroutine != null)
                StopCoroutine(m_StepCoroutine);

            m_HasToldHowToViz = false;

            m_DemoHashFuncDev.gameObject.SetActive(false);
            m_PlayerHashFuncDev.gameObject.SetActive(true);
            m_KeyboardController.gameObject.SetActive(true);

            m_StepCoroutine = StartCoroutine(StepRoutine());
        }

        public override void Deactivate()
        {
            
        }

        public override void OnSlideExit()
        {
            
        }

        IEnumerator StepRoutine()
        {
            if (m_VOSource.isPlaying)
                m_VOSource.Stop();

            yield return PlayAndWaitVoice(m_VOSource, m_TryItKeypadClip);

            Debug.Log("Sandbox");
        }

        private void OnKeypadSubmit()
        {
            if (slide.manager.CurrentSlide == slide && slide.steps[slide.CurrentStep] == this && !m_HasToldHowToViz)
                PlayVoiceNoWait(m_VOSource, m_ShowCodeViz);

            m_HasToldHowToViz = true;
        }
    }
}
