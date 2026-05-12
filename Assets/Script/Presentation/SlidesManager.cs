using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.InputSystem;


namespace Canvas
{
    [System.Serializable]
    public class StepEvent : UnityEvent<Step> { }

    public class SlidesManager : MonoBehaviour
    {
        public StepEvent OnNextStepEvent;

        [Header("Slides to Manage")]
        [SerializeField] private Slides[] slides;
        [SerializeField, HideInInspector]
        private int currentSlideIndex = -1;

        [Header("Input Actions")]
        [SerializeField] private bool autoStart = true;
        [SerializeField] private InputActionReference nextStepAction;
        [SerializeField] private InputActionReference prevStepAction;
        [SerializeField] private InputActionReference nextSlideAction;
        [SerializeField] private InputActionReference prevSlideAction;
        [SerializeField] private InputActionReference toBeginningAction;
        [SerializeField] private InputActionReference toEndAction;


        [Header("Options")]
        [SerializeField] private Slides m_StartSlide = null;

        private Dictionary<string, int> m_SlideNameToIndexTable = new Dictionary<string, int>();


        public void Setup()
        {
            Debug.Assert(slides != null && slides.Length > 0,
                $"[SlidesManager] No Slides assigned on {name}");

            int index = 0;
            foreach (var slide in slides)
            {
                slide.manager = this;

                if (slide.SlideName != null)
                {
                    // Check for Duplicates
                    string slideName = slide.SlideName;
                    if (m_SlideNameToIndexTable.ContainsKey(slideName))
                    {
                        Debug.Log($"Warning! Duplicate SlideNames Detected! Name: {slide}.");
                    }
                    else
                    {
                        m_SlideNameToIndexTable[slideName] = index;
                        Debug.Log($"SlideManager: Added {slideName} to SlideNameToIndex table.");
                    }

                }
                index++;
            }

            if (m_StartSlide != null)
            {
                currentSlideIndex = m_SlideNameToIndexTable[m_StartSlide.SlideName];
            }
            else
            {
                currentSlideIndex = 0;
            }
            ShowCurrentSlide();
        }

        public void NextSlide()
        {
            if (!IsValidState()) return;
            if (currentSlideIndex == slides.Length - 1)
                return;

            int nextIndex = currentSlideIndex + 1;
            GoToSlide(nextIndex);
        }

        public void PrevSlide()
        {
            if (!IsValidState()) return;
            if (currentSlideIndex == 0)
                return;


            int prevIndex = currentSlideIndex - 1;
            GoToSlide(prevIndex);
        }


        // Advances the current step of the slide and moves to the next slide if we reached the end limit of the steps
        public void NextStep()
        {
            if (!IsValidState()) return;



            bool endReached = slides[currentSlideIndex].Next();
            OnNextStepEvent?.Invoke(slides[currentSlideIndex].CurrentStep);
            if (endReached)
                NextSlide();
        }


        // Moves the current step of the slide back, and moves to the prev slide if we reached the beginning limit of the steps
        public void PrevStep()
        {
            if (!IsValidState())
            {
                Debug.Log("Prev Step Aborted. System is in invalid state");
                return; 
            }
            bool startReached = slides[currentSlideIndex].Previous();
            OnNextStepEvent?.Invoke(slides[currentSlideIndex].CurrentStep);
            if (startReached)
                PrevSlide();
        }

        public void ReplayStep()
        {
            if (!IsValidState())
            {
                Debug.Log("Replay Step Aborted. System is in invalid state");
                return;
            }

            slides[currentSlideIndex].ReplayStep();
            OnNextStepEvent?.Invoke(slides[currentSlideIndex].CurrentStep);
        }

        public Slides CurrentSlide => IsValidState() ? slides[currentSlideIndex] : null;
        public int CurrentSlideIndex => currentSlideIndex;
        public int SlideCount => slides?.Length ?? 0;


        // Private
        private void Start()
        {
            //Debug.Log(SystemInfo.graphicsDeviceName);

            if (autoStart)
                Setup();

            SetupInputActions();

        }

        public void JumpToStep(Step _step)
        {
            if (_step == null)
                return;

            if (_step.slide != CurrentSlide)
                return;

            CurrentSlide.JumpToStep(_step);
            OnNextStepEvent?.Invoke(CurrentSlide.CurrentStep);
        }

        public void JumpToSlide(Slides destination, bool restart = false)
        {
            JumpToSlide(destination.SlideName, restart);
        }

        public void JumpToSlide(string name, bool restart = false)
        {
            JumpToSlide(m_SlideNameToIndexTable[name], restart);
        }

        public void JumpToSlide(int index, bool restart = false)
        {
            if (!IsValidState()) return;

            GoToSlide(index, restart);
        }

        private void GoToSlide(int index, bool restart = false)
        {
            if (currentSlideIndex == index && !restart) return;

            // Cleanup current
            if (IsValidState())
                slides[currentSlideIndex].Cleanup();

            currentSlideIndex = index;

            // Setup new
            slides[currentSlideIndex].Setup();
            OnNextStepEvent?.Invoke(CurrentSlide.CurrentStep);
        }

        private void ShowCurrentSlide()
        {
            if (IsValidState())
                slides[currentSlideIndex].Setup();
        }

        private bool IsValidState()
        {
            return slides != null &&
                   slides.Length > 0 &&
                   currentSlideIndex >= 0 &&
                   currentSlideIndex < slides.Length &&
                   slides[currentSlideIndex] != null;
        }

        public void GoToBeginning()
        {
            if (!IsValidState()) return;
            GoToSlide(0);
        }

        public void GoToEnd()
        {
            if (!IsValidState()) return;
            GoToSlide(slides.Length - 1);
        }


        private void SetupInputActions()
        {
            if (nextStepAction != null) nextStepAction.action.performed += ctx => NextStep();
            if (prevStepAction != null) prevStepAction.action.performed += ctx => PrevStep();
            if (nextSlideAction != null) nextSlideAction.action.performed += ctx => NextSlide();
            if (prevSlideAction != null) prevSlideAction.action.performed += ctx => PrevSlide();
            if (toBeginningAction != null) toBeginningAction.action.performed += ctx => GoToBeginning();
            if (toEndAction != null) toEndAction.action.performed += ctx => GoToEnd();
        }
    }
}