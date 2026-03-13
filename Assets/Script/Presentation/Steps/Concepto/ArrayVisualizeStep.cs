using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Canvas
{
    public class ArrayVisualizeStep : Step
    {
        [Header("References")]
        [SerializeField] private AudioSource m_VoiceSource;
        [SerializeField] private AudioClip m_HashmapExplanation;
        [SerializeField] private AudioClip m_ArrayComparison;
        [SerializeField] private AudioClip m_AHashmapLets;
        [SerializeField] private AudioClip m_Buthow;

        
        [SerializeField] private BoxScriptController m_CardboardPrefab;
        [SerializeField] private Paper m_PaperPrefab;
        [SerializeField] private ScriptVisualizer m_ScriptVisualizerPrefab;

        [SerializeField] private Printer m_PaperPrinter;



        [Header("Options")]
        [SerializeField] private string[] m_HashmapExampleKeys;
        [SerializeField] private Transform m_StartTransform;
        [SerializeField] private float m_PaperStartOffset = 1.0f;


        [SerializeField] private int m_BoxesAmount = 5;
        [SerializeField] private Vector3 m_InstanceOffset = Vector3.forward;


        [Header("Animations")]
        [SerializeField] private float m_PaperTransSpeedVertical = 2.0f;
        [SerializeField] private float m_PaperTransSpeedHorizontal = 2.0f;
        [SerializeField] private float m_BoxGrowSpeed = 1.0f;
        [SerializeField] private float m_BoxGrowDelay = 0.1f;
        [SerializeField] private LeanTweenType m_PaperVerticalTweenType = LeanTweenType.easeInOutQuad;
        [SerializeField] private LeanTweenType m_PaperHorizontalTweenType = LeanTweenType.easeInOutQuad;
        [SerializeField] private LeanTweenType m_BoxShrinkTweenType = LeanTweenType.easeInOutQuad;
        [SerializeField] private LeanTweenType m_BoxGrowTweenType = LeanTweenType.easeInOutQuad;


        [SerializeField] private float m_CloseOpenWaitTime = 0.5f;

        // Instances
        private List<BoxScriptController> m_SpawnedBoxes;
        private GameObject m_SpawnedPaper;
        private ScriptVisualizer m_SpawnedScriptVisualizer;

        
        private Coroutine m_SlideCoroutine;
        private int m_BoxIndexExample;




        private void Awake()
        {
            m_SpawnedBoxes = new List<BoxScriptController>(m_BoxesAmount);
            m_BoxIndexExample = m_BoxesAmount / 2 + (m_BoxesAmount % 2);

            m_SpawnedScriptVisualizer = Instantiate(m_ScriptVisualizerPrefab);
            m_SpawnedScriptVisualizer.gameObject.SetActive(false);
        }

        private Vector3 localBoxSize = Vector3.one;
        public override void Activate()
        {
            // Spawn boxes
            for (int i = 0; i < m_BoxesAmount; i++)
            {
                BoxScriptController instance = Instantiate(m_CardboardPrefab);

                instance.transform.position =
                    m_StartTransform.position + m_InstanceOffset * i;

                instance.gameObject.SetActive(false);

                m_SpawnedBoxes.Add(instance);
            }

            localBoxSize = m_SpawnedBoxes[0].transform.localScale;

            if (m_SlideCoroutine != null)
                StopCoroutine(m_SlideCoroutine);

            m_SlideCoroutine = StartCoroutine(SlideRoutine());
        }

        private IEnumerator SlideRoutine()
        {
            // Play explanation
            yield return PlayAndWaitVoice(m_VoiceSource, m_HashmapExplanation);

            // Animate boxes appearing
            int index = 0;
            foreach (BoxScriptController _box in m_SpawnedBoxes)
            {
                _box.gameObject.SetActive(true);
                _box.SetLabel(index.ToString());
                _box.transform.localScale = Vector3.zero;

                _box.transform.LeanScale(localBoxSize, m_BoxGrowSpeed);
                
                yield return new WaitForSeconds(m_BoxGrowDelay);
                index++;
            }

            yield return PlayAndWaitVoice(m_VoiceSource, m_ArrayComparison);



            // Animate storing data
            BoxScriptController box = m_SpawnedBoxes[m_BoxIndexExample];
            Animator boxAnimator = box.GetAnimator();
            box.Open();
            yield return WaitForAnimator(boxAnimator);
            

            // Create new paper
            if ( m_SpawnedPaper !=  null)
                Destroy(m_SpawnedPaper);
            
            m_SpawnedPaper = null;
            m_PaperPrinter.PrintNoAnim("63", p => m_SpawnedPaper = p.gameObject, Paper.PAPER_TYPE.Data);
            yield return new WaitUntil(() => m_SpawnedPaper != null);


            {
                Paper paper = m_SpawnedPaper.GetComponent<Paper>();
                Destroy(paper);

                XRBaseInteractable interactable = m_SpawnedPaper.GetComponent<XRBaseInteractable>();
                Destroy(interactable);

                Rigidbody rbody = m_SpawnedPaper.GetComponent<Rigidbody>();
                Destroy(rbody);
            }
            m_SpawnedPaper.transform.rotation = box.transform.rotation * Quaternion.Euler(0.0f, -90.0f, 0.0f);

            Vector3 startPos = box.transform.position + Vector3.up * m_PaperStartOffset;
            m_SpawnedPaper.transform.position = startPos;

            // Target position (inside the box)
            Vector3 targetPos = box.transform.position;

            // Tween the paper into the box
            bool tweenFinished = false;

            yield return null;

            LeanTween.move(m_SpawnedPaper.gameObject, targetPos, m_PaperTransSpeedVertical)
                .setEase(m_PaperVerticalTweenType)
                .setOnComplete(() => tweenFinished = true);


            // Wait until tween finishes
            yield return new WaitUntil(() => tweenFinished);
            

            box.Close();
            yield return WaitForAnimator(boxAnimator);

            yield return new WaitForSeconds(m_CloseOpenWaitTime);

            box.Open();
            yield return WaitForAnimator(boxAnimator);


            // Take Paper Out
            tweenFinished = false;
            LeanTween.move(m_SpawnedPaper, startPos, m_PaperTransSpeedVertical)
                .setEase(m_PaperVerticalTweenType)
                .setOnComplete(() => tweenFinished = true);
            yield return new WaitUntil(() => tweenFinished);

            box.Close();

            // Remove all boxes
            foreach (BoxScriptController _box in m_SpawnedBoxes)
            {
                _box.gameObject.SetActive(false);
                //_box.transform.LeanScale(Vector3.zero, 0.2f);
                yield return new WaitForSeconds(0.1f);
            }

            // Animate boxes appearing
            index = 0;

            m_VoiceSource.clip = m_AHashmapLets;
            m_VoiceSource.Play();

            foreach (BoxScriptController _box in m_SpawnedBoxes)
            {
                _box.gameObject.SetActive(true);
                _box.SetLabel(m_HashmapExampleKeys[index % m_HashmapExampleKeys.Length]);
                //_box.transform.LeanScale(localBoxSize, 0.2f);
                yield return new WaitForSeconds(0.1f);
                index++;
            }




            // -------------------------- Paper Hashmap Insert --------------------------------------
            {
                // Move horizontally
                int hashmapIndex = (m_BoxIndexExample + 1) % m_SpawnedBoxes.Count;
                tweenFinished = false;
                Vector3 target = m_SpawnedBoxes[hashmapIndex].transform.position
                    + Vector3.up * m_PaperStartOffset;
                LeanTween.move(m_SpawnedPaper.gameObject, target, m_PaperTransSpeedHorizontal)
                    .setEase(m_PaperHorizontalTweenType)
                    .setOnComplete(() => tweenFinished = true);
                yield return new WaitUntil(() => tweenFinished);

                BoxScriptController hashmapBox = m_SpawnedBoxes[hashmapIndex];

                hashmapBox.Open();
                yield return new WaitUntil(() =>
                {
                    AnimatorStateInfo state = boxAnimator.GetCurrentAnimatorStateInfo(0);
                    return state.normalizedTime >= 1f && !boxAnimator.IsInTransition(0);
                });

                // Insert
                tweenFinished = false;
                LeanTween.move(m_SpawnedPaper.gameObject, m_SpawnedBoxes[hashmapIndex].transform.position,
                    m_PaperTransSpeedVertical)
                    .setEase(m_PaperVerticalTweenType)
                    .setOnComplete(() => tweenFinished = true);
                // Wait until tween finishes
                yield return new WaitUntil(() => tweenFinished);


                hashmapBox.Close();
                yield return new WaitUntil(() =>
                {
                    AnimatorStateInfo state = boxAnimator.GetCurrentAnimatorStateInfo(0);
                    return state.normalizedTime >= 1f && !boxAnimator.IsInTransition(0);
                });

                yield return new WaitForSeconds(m_CloseOpenWaitTime);


                hashmapBox.Open();
                yield return new WaitUntil(() =>
                {
                    AnimatorStateInfo state = boxAnimator.GetCurrentAnimatorStateInfo(0);
                    return state.normalizedTime >= 1f && !boxAnimator.IsInTransition(0);
                });



                // Take Paper Out
                tweenFinished = false;
                LeanTween.move(m_SpawnedPaper, target, m_PaperTransSpeedVertical)
                    .setEase(m_PaperVerticalTweenType)
                    .setOnComplete(() => tweenFinished = true);
                yield return new WaitUntil(() => tweenFinished);

                m_SpawnedPaper.gameObject.SetActive(false);
                hashmapBox.Close();
            }

            yield return new WaitWhile(() => m_VoiceSource.isPlaying);


            {
                for (int i = 0; i < m_SpawnedBoxes.Count; i++)
                {
                    BoxScriptController _box = m_SpawnedBoxes[i];
                    LeanTween.scale(_box.gameObject, Vector3.zero, m_BoxGrowSpeed)
                        .setEase(m_BoxGrowTweenType);
                    
                    yield return new WaitForSeconds(m_BoxGrowDelay);
                }
            }



            m_VoiceSource.clip = m_Buthow;
            m_VoiceSource.Play();

            yield return new WaitWhile(() => m_VoiceSource.isPlaying);

            // Step finished
            Complete();
        }


        public override void Deactivate()
        {
            if (m_SlideCoroutine != null)
            {
                StopCoroutine(m_SlideCoroutine);
                m_SlideCoroutine = null;
            }

            foreach (BoxScriptController instance in m_SpawnedBoxes)
            {
                if (instance != null)
                    Destroy(instance.gameObject);
            }

            m_SpawnedBoxes.Clear();
            Destroy(m_SpawnedPaper);
            m_SpawnedPaper = null;
            
        }

        public override void OnSlideExit()
        {
        }

    }
}