using Concepto.HashMap;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
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


        public void OnInputEntered()
        {
            if (inputSocketInteractor == null || outputSocketInteractor == null)
                return;

            if (!inputSocketInteractor.hasSelection)
                return;

            // Get interactable object
            XRBaseInteractable interactable =
                (XRBaseInteractable)inputSocketInteractor.firstInteractableSelected;

            if (interactable == null)
                return;

            GameObject paperObject = interactable.gameObject;
            

            // Remove paper from  input socket
            inputSocketInteractor.interactionManager.SelectExit((IXRSelectInteractor)inputSocketInteractor, interactable);

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
            

            paperObject.transform.position = inputPaperStartPos.position;
            paperObject.transform.rotation = inputPaperStartPos.rotation;

            // Animate sliding into machine
            LeanTween.move(paperObject, inputPaperFinalPos.position, inputSlideDuration)
                .setEase(LeanTweenType.linear)
                .setOnComplete(() =>
                {
                    Destroy(paperObject);
                    resultPrinter.PrintHashkey(hashkey.ToString());
                });
        }
    }

}
