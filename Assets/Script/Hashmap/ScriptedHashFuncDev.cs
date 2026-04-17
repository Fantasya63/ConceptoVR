using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using static UnityEditor.U2D.ScriptablePacker;

namespace Concepto.HashMap
{

    public class ScriptedHashFuncDev : MonoBehaviour
    {
        [Header("Audio")]
        [SerializeField] AudioSource m_PaperFeedAudioSource;

        [Header("Positional Markers")]
        [SerializeField] Transform m_InputDropPos;

        [SerializeField]
        private Transform m_InputPaperStartPos;

        [SerializeField]
        private Transform m_InputPaperFinalPos;

        [SerializeField]
        private Transform m_OutputPaperStartPos;
        [SerializeField]
        private Transform m_OutputPaperEndPos;

        [SerializeField] Vector3 m_PaperIndexFinalPosOffset = Vector3.up * 0.3f;

        [Header("Timings")]
        [SerializeField] private float m_SlideDurToDropSite = 2.0f;
        [SerializeField] private float m_InputDropDuration = 0.5f;
        [SerializeField] private float m_InputSlideDuration = 2.0f;

        [Header("External Devices")]
        [SerializeField]
        private Printer m_ResultPrinter;

        public UnityEvent<Paper> OnPaperPrinted;

        [Header("Script Visualization")]
        [SerializeField]
        private ScriptVisualizer m_Visualizer;

        [SerializeField]
        [TextArea(5, 20)]
        private string m_ScriptTemplate;

        public void Awake()
        {
            if (m_Visualizer == null)
                Debug.LogWarning($"{name} has no Script Visualizer attached to it.");
            //m_ResultPrinter.OnPaperPrinted.AddListener(
            //(Paper paper) => {
            //    Debug.Log("HashFuncDev: PaperPrinted");
            //    OnPaperPrinted.Invoke(paper);

            //});
        }

        public void PrintPaperScripted(Paper paper)
        {
            XRBaseInteractable interactable = paper.GetComponent<XRBaseInteractable>();

            Print(interactable);
        }

        string GetScriptEquivalent(string input, int hash)
        {
            return string.Format(m_ScriptTemplate, input, hash);
        }

        public IEnumerator Hash(Paper _key)
        {
            Paper key = Instantiate(_key);
            key.GetComponent<Renderer>().material = _key.GetComponent<Renderer>().material;
            key.transform.position = _key.transform.position;
            key.transform.rotation = _key.transform.rotation;

            Debug.Assert(key != null);
            if (key.IsInteractive)
                key.RemoveInteractivity();

            LeanTween.cancel(key.gameObject);

            // Slide to Move
            {

                bool isDone = false;
                LeanTween.move(key.gameObject, m_InputDropPos.position, m_SlideDurToDropSite);
                LeanTween.rotate(key.gameObject, m_InputDropPos.eulerAngles, m_SlideDurToDropSite)
                    .setOnComplete(() => isDone = true);

                yield return new WaitUntil(() => isDone);
            }


            // Drop
            {
                key.transform.LeanMove(m_InputPaperStartPos.position, m_InputDropDuration);
                yield return new WaitForSeconds(m_InputDropDuration);
            }

            // Slide Input
            {
                m_PaperFeedAudioSource.Play();
                key.transform.rotation = m_InputPaperStartPos.rotation;
                key.transform.LeanMove(m_InputPaperFinalPos.position, m_InputSlideDuration);
                yield return new WaitForSeconds(m_InputSlideDuration);

                Destroy(key.gameObject);
            }

            // Hash The key
            Paper indexPaper = null;
            {
                int hashkey = HashMap.HashFunc.Hash(key.data, HashMap.HashFunc.NumBoxes);
                // Make sure we are not subscribing morethan once
                m_ResultPrinter.OnPaperPrinted.AddListener((Paper paper) => { 
                   indexPaper = paper; 
                });

                yield return m_ResultPrinter.PrintHashkeyRoutine(hashkey.ToString(), false);

                Debug.Assert(indexPaper != null);
            }

            OnPaperPrinted.Invoke(indexPaper);
        }

        private void Print(XRBaseInteractable interactable)
        {
            if (interactable == null)
                return;

            GameObject paperObject = interactable.gameObject;



            string paperData;
            {
                Paper insertedPaper = paperObject.GetComponent<Paper>();
                paperData = new string(insertedPaper.data);
                if (insertedPaper != null)
                    Destroy(insertedPaper);

                // Disable interaction while animating
                XRGrabInteractable grab = interactable.gameObject.GetComponent<XRGrabInteractable>();
                if (grab != null)
                    Destroy(grab);

                // Disable physics
                Rigidbody paperRigidBody = interactable.gameObject.gameObject.GetComponent<Rigidbody>();
                if (paperRigidBody != null)
                    Destroy(paperRigidBody);

                BoxCollider boxCollider = interactable.gameObject.gameObject.GetComponent<BoxCollider>();
                if (boxCollider != null)
                    Destroy(boxCollider);
            }

            int hashkey = HashMap.HashFunc.Hash(paperData, HashMap.HashFunc.NumBoxes);

            paperObject.transform.position = m_InputPaperStartPos.position;
            paperObject.transform.rotation = m_InputPaperStartPos.rotation;

            // Animate sliding into machine
            LeanTween.move(paperObject, m_InputPaperFinalPos.position, m_InputSlideDuration)
                .setEase(LeanTweenType.linear)
                .setOnComplete(() =>
                {
                    Destroy(paperObject);
                    m_ResultPrinter.PrintHashkey(hashkey.ToString());
                });

            if (m_Visualizer != null)
                m_Visualizer.SetCodeWithNotif(GetScriptEquivalent(paperData, hashkey));
        }
    }
}