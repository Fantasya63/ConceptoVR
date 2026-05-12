using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

namespace Canvas
{
    public class  Slides : MonoBehaviour
    {
        [Header("Configuration")]
        public Step[] steps;

        [HideInInspector]
        public SlidesManager manager;

#if UNITY_EDITOR
        [SerializeField] private int m_DebugStartStepIndex = 0;
#endif

        private Dictionary<string, int> m_StepNameToIndexTable = new Dictionary<string, int>();

        public int StepNameToIndex(string stepName)
        {
            return m_StepNameToIndexTable[stepName];
        }

        public string SlideName
        {
            get { return gameObject.name; }
        }

        private void Awake()
        {
            Debug.Assert(steps != null, $"[Slides] Steps array is null on {name}");
            Debug.Assert(steps.Length > 0, $"[Slides] Steps array is empty on {name}");


            int index = 0;
            foreach (Step step in steps)
            {
                step.slide = this;
                if (m_StepNameToIndexTable.ContainsKey(step.name))
                {
                    Debug.LogError($"Warning! Duplicate Step Names Detected! Name: {step.name}.");
                }
                else
                {
                    m_StepNameToIndexTable[step.name] = index;
                    Debug.Log($"{SlideName} Slide: Added {step.name} step.");
                }


                step.OnCompleted.AddListener(OnStepComplete);
                index++;
            }
        }

        public void Setup()
        {
            
            if (currentStepIndex != -1)
                Debug.LogWarning($"[Slides] Setup() called multiple times on {name}. Previous state will be overridden.");

            // Ensure current step is valid before doing anything

            currentStepIndex = 0;
#if UNITY_EDITOR
            currentStepIndex = m_DebugStartStepIndex;
#endif

            // Activate first step
            steps[currentStepIndex].Activate();
        }



        public void Cleanup()
        {
            if (currentStepIndex < 0 || steps == null || currentStepIndex >= steps.Length)
                return;

            steps[currentStepIndex].Deactivate();
            currentStepIndex = -1;

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
            bool allowNext = steps[currentStepIndex].OnNextStep();
           


            if (currentStepIndex == steps.Length - 1)
                return true;

            steps[currentStepIndex].Deactivate();

            currentStepIndex += 1;

            AssertStepIsNotNull(currentStepIndex);
            steps[currentStepIndex].Activate();
            return false;

        }



        public bool Previous()
        {
            AssertIsValidState();
            if (currentStepIndex == 0)
                return true;

            steps[currentStepIndex].Deactivate();

            currentStepIndex -= 1;
            
            AssertStepIsNotNull(currentStepIndex);
            steps[currentStepIndex].Activate();
            return false;
        }

        public void JumpToStep(Step destination)
        {
            if (!m_StepNameToIndexTable.ContainsKey(destination.name))
            {
                Debug.LogError("Aborting Jump Step. Step is not part of the slide");
                return;
            }

            int index = m_StepNameToIndexTable[destination.name];
            steps[currentStepIndex].Deactivate();
            currentStepIndex = index;

            AssertStepIsNotNull(currentStepIndex);
            steps[currentStepIndex].Activate();

        }

        private void OnStepComplete()
        {
            if (manager.CurrentSlide == this)
                manager.NextStep();
        }

        public int CurrentStepIndex { get { return currentStepIndex; } }
        public Step CurrentStep
        { get { return steps[currentStepIndex]; } }

        [SerializeField, HideInInspector]
        private int currentStepIndex = -1; // -1 = not initialized

        // Helper Methods

        private void AssertIsValidState()
        {
            Debug.Assert(steps != null, $"[Slides] Steps array is null on {name}");
            Debug.Assert(steps.Length > 0, $"[Slides] Steps array is empty on {name}");
            Debug.Assert(currentStepIndex >= 0 && currentStepIndex < steps.Length,
                $"[Slides] currentStep is invalid: {currentStepIndex}. Valid range: 0–{steps.Length - 1}");
        }

        private void AssertStepIsNotNull(int index)
        {
            Debug.Assert(steps[index] != null,
                $"[Slides] Step at index {index} is null on {name}!");
        }

        public void ReplayStep()
        {
            AssertIsValidState();
            AssertStepIsNotNull(currentStepIndex);

            steps[currentStepIndex].Deactivate();
            steps[currentStepIndex].Activate();
            
            Debug.Log($"Replaying step: {steps[currentStepIndex].name}");
            return;
        }


        public void Replay()
        {
            Debug.Log("Attempting to replay slide.");
            if (manager.CurrentSlide != this)
                return;

            Debug.Log("Replayed slide");
            manager.JumpToSlide(manager.CurrentSlideIndex, true);
        }

        public bool IsCurrentStep(Step step)
        {
            Debug.Assert(m_StepNameToIndexTable.ContainsKey(step.name), $"Step: {step.name} is not part of the slide");

            int index = m_StepNameToIndexTable[step.name];
            return index == currentStepIndex;
        }
    }
}