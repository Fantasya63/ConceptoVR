using UnityEngine;
using UnityEngine.Events;

namespace Canvas
{
    public class Slides : MonoBehaviour
    {
        [Header("Configuration")]
        public Step[] steps;

        [HideInInspector]
        public SlidesManager manager;

#if UNITY_EDITOR
        [SerializeField] private int m_DebugStartStepIndex = 0;
#endif

        public string SlideName
        {
            get { return gameObject.name; }
        }


        public void Setup()
        {
            Debug.Assert(steps != null, $"[Slides] Steps array is null on {name}");
            Debug.Assert(steps.Length > 0, $"[Slides] Steps array is empty on {name}");

            foreach (Step step in steps)
            {
                step.slide = this;
                step.OnCompleted.AddListener(OnStepComplete);
            }

            if (currentStep != -1)
                Debug.LogWarning($"[Slides] Setup() called multiple times on {name}. Previous state will be overridden.");

            // Ensure current step is valid before doing anything

            currentStep = 0;
#if UNITY_EDITOR
            currentStep = m_DebugStartStepIndex;
#endif


            // Activate first step
            steps[currentStep].Activate();
        }



        public void Cleanup()
        {
            if (currentStep < 0 || steps == null || currentStep >= steps.Length)
                return;

            steps[currentStep].Deactivate();
            currentStep = -1;

            foreach (Step step in steps)
            {
                step.OnSlideExit();
                step.OnCompleted.RemoveListener(OnStepComplete);
            }
        }

        // returns true if want to go to the next slide
        public bool Next()
        {
            AssertIsValidState();
            bool allowNext = steps[currentStep].OnNextStep();
            if (!allowNext)
            {
                return false;
            }


            if (currentStep == steps.Length - 1)
                return true;

            steps[currentStep].Deactivate();

            currentStep += 1;

            AssertStepIsNotNull(currentStep);
            steps[currentStep].Activate();
            return false;

        }



        public bool Previous()
        {
            AssertIsValidState();
            if (currentStep == 0)
                return true;

            steps[currentStep].Deactivate();

            currentStep = (currentStep - 1 + steps.Length) % steps.Length;

            AssertStepIsNotNull(currentStep);
            steps[currentStep].Activate();
            return false;
        }

        private void OnStepComplete()
        {
            if (manager.CurrentSlide == this)
                manager.NextStep();
        }

        public int CurrentStep { get { return currentStep; } }

        [SerializeField, HideInInspector]
        private int currentStep = -1; // -1 = not initialized

        // Helper Methods

        private void AssertIsValidState()
        {
            Debug.Assert(steps != null, $"[Slides] Steps array is null on {name}");
            Debug.Assert(steps.Length > 0, $"[Slides] Steps array is empty on {name}");
            Debug.Assert(currentStep >= 0 && currentStep < steps.Length,
                $"[Slides] currentStep is invalid: {currentStep}. Valid range: 0–{steps.Length - 1}");
        }

        private void AssertStepIsNotNull(int index)
        {
            Debug.Assert(steps[index] != null,
                $"[Slides] Step at index {index} is null on {name}!");
        }

        public void Replay()
        {
            Debug.Log("Attempting to replay slide.");
            if (manager.CurrentSlide != this)
                return;

            Debug.Log("Replayed slide");
            manager.JumpToSlide(manager.CurrentSlideIndex, true);
        }
    }
}