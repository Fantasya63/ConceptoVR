using UnityEngine;

public class SimpleVRBoxLock : MonoBehaviour
{
    public Animator boxAnimator;
    public string openParameter = "IsOpen";
    public Collider boxTrigger;

    private bool lastState;

    void Start()
    {
        lastState = boxAnimator.GetBool(openParameter);
    }

    void Update()
    {
        bool isOpen = boxAnimator.GetBool(openParameter);

        // Debug.Log("isOpen: " + isOpen + " | lastState: " + lastState);

        if (isOpen != lastState)
        {
            if (!isOpen)
                LockObjects();
            else
                UnlockObjects();

        }

    }

    void LockObjects()
    {
        Collider[] objectsInside = Physics.OverlapBox(
            boxTrigger.bounds.center,
            boxTrigger.bounds.extents,
            boxTrigger.transform.rotation,
            ~0,
            QueryTriggerInteraction.Collide
        );

        Debug.Log("LockObjects called! Objects found: " + objectsInside.Length);

        foreach (var col in objectsInside)
        {
            Debug.Log("Found object: " + col.gameObject.name);

            UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab = col.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            Rigidbody rb = col.GetComponent<Rigidbody>();
            if (grab != null)
            {
                Debug.Log("Disabling grab on: " + col.gameObject.name);
                if (grab.isSelected)
                    grab.interactionManager.SelectExit(grab.firstInteractorSelecting, grab);

                grab.enabled = false;
            }

            if (rb != null)
            {
                rb.isKinematic = true;
            }
        }
    }

    void UnlockObjects()
    {
        Collider[] objectsInside = Physics.OverlapBox(
            boxTrigger.bounds.center,
            boxTrigger.bounds.extents,
            boxTrigger.transform.rotation,
            ~0,
            QueryTriggerInteraction.Collide
        );

        foreach (var col in objectsInside)
        {
            UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab = col.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            Rigidbody rb = col.GetComponent<Rigidbody>();
            if (grab != null)
                grab.enabled = true;
            if (rb != null)
                rb.isKinematic = false;
        }
    }
}