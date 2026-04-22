using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Concepto.HashMap
{
    public class HashFuncDevice : MonoBehaviour
    {
        [Header("Sockets")]
        [SerializeField]
        private XRSocketInteractor inputSocketInteractor;
        [SerializeField]
        private XRSocketInteractor outputSocketInteractor;

        [Header("Positional Markers")]
        [SerializeField]
        private Transform inputPaperStartPos;

        [SerializeField]
        private Transform inputPaperFinalPos;

        [SerializeField]
        private Transform outputPaperStartPos;
        [SerializeField]
        private Transform outputPaperEndPos;

        [Header("Timings")]
        [SerializeField] private float inputSlideDuration = 2.0f;

        [Header("External Devices")]
        [SerializeField]
        private Printer resultPrinter;

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
            resultPrinter.OnPaperPrinted.AddListener(
            (Paper paper) =>
            {
                Debug.Log("HashFuncDev: PaperPrinted");
                OnPaperPrinted.Invoke(paper);

            });
        }

        public void PrintPaperScripted(Paper paper)
        {
            XRBaseInteractable interactable = paper.GetComponent<XRBaseInteractable>();

            Print(interactable);
        }

        public void OnInputEntered()
        {
            if (inputSocketInteractor == null || outputSocketInteractor == null)
                return;

            if (!inputSocketInteractor.hasSelection)
                return;


            // Get interactable object
            XRBaseInteractable interactable =
                (XRBaseInteractable)inputSocketInteractor.firstInteractableSelected;

            // Remove paper from  input socket
            inputSocketInteractor.interactionManager.SelectExit((IXRSelectInteractor)inputSocketInteractor, interactable);

            Print(interactable);
        }

        string GetScriptEquivalent(string input, int hash)
        {
            return string.Format(m_ScriptTemplate, input, hash);
        }

        GameObject m_CurrentlyPrinting = null;

        private void Print(XRBaseInteractable interactable)
        {
            if (interactable == null)
                return;


            m_CurrentlyPrinting = interactable.gameObject;


            string paperData;
            {
                Paper insertedPaper = m_CurrentlyPrinting.GetComponent<Paper>();
                paperData = new string(insertedPaper.data);
                if (insertedPaper != null)
                {
                    Destroy(insertedPaper);
                }

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


            m_CurrentlyPrinting.transform.position = inputPaperStartPos.position;
            m_CurrentlyPrinting.transform.rotation = inputPaperStartPos.rotation;

            // Animate sliding into machine
            LeanTween.move(m_CurrentlyPrinting, inputPaperFinalPos.position, inputSlideDuration)
                .setEase(LeanTweenType.linear)
                .setOnComplete(() =>
                {
                    if (m_CurrentlyPrinting != null)
                        Destroy(m_CurrentlyPrinting);
                    
                    resultPrinter.PrintHashkey(hashkey.ToString());
                });

            if (m_Visualizer != null)
                m_Visualizer.SetCodeWithNotif(GetScriptEquivalent(paperData, hashkey));
        }

        private void OnDestroy()
        {
            if (m_CurrentlyPrinting != null)
            {
                if (LeanTween.isTweening(m_CurrentlyPrinting))
                {
                    LeanTween.cancel(m_CurrentlyPrinting);
                }
            }
            

            if (m_CurrentlyPrinting != null)
            {
                Destroy(m_CurrentlyPrinting);
            }
        }
    }
}
    