using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRSimpleInteractable))]
public class ButtonFollowVisual : MonoBehaviour
{
    [Header("References")]
    public Transform visualTarget;
    
    [Header("Settings")]
    public float resetSpeed = 5;
    public float followAngleThreshold = 45;
    public bool isToggleable = false;

    [Header("Positional Markers")]
    [SerializeField]
    private Transform unpressedHeight;
    [SerializeField]
    private Transform pressedHeight;
    
    

    [Header("Events")]
    public UnityEvent OnPressed;
    public UnityEvent OnReleased;

    private Transform pokeAttachTransform;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable interactable;
    private bool isPoked = false;

    
    private bool isPressed = false;
    private bool wasIsPressed = false;

    // Quaternion to rotate from model to buttonDir space
    private Quaternion buttonUPQuat = Quaternion.identity;
    
    // The direction from the origin to the unpressed height marker
    private Vector3 m_VectorToSurface = Vector3.zero;
    private Vector3 m_ButtonUpDirection = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        // Make sure unpressedHeight and pressed Height is assigned;
        Debug.Assert(unpressedHeight != null);
        Debug.Assert(pressedHeight != null);

        interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();
        interactable.hoverEntered.AddListener(OnPoke);
        interactable.hoverExited.AddListener(PokeExited);


        m_VectorToSurface = unpressedHeight.transform.localPosition - transform.localPosition;
        m_ButtonUpDirection = m_VectorToSurface.normalized;
        buttonUPQuat = Quaternion.FromToRotation(m_ButtonUpDirection, Vector3.up);
    }

    public void OnPoke(BaseInteractionEventArgs hover)
    {

        if(hover.interactorObject is UnityEngine.XR.Interaction.Toolkit.Interactors.XRPokeInteractor)
        {
            UnityEngine.XR.Interaction.Toolkit.Interactors.XRPokeInteractor interactor = (UnityEngine.XR.Interaction.Toolkit.Interactors.XRPokeInteractor)hover.interactorObject;
            isPoked = true;
         
            pokeAttachTransform = interactor.attachTransform;

        }
    }

    private Vector3 WorldPosToButtonPos(Vector3 point)
    {
        return LocalPosToButtonPos(transform.InverseTransformPoint(point));
    }

    private Vector3 LocalPosToButtonPos(Vector3 point)
    {
        return buttonUPQuat * point;
    }



    public void PokeExited(BaseInteractionEventArgs hover)
    {
        if (hover.interactorObject is UnityEngine.XR.Interaction.Toolkit.Interactors.XRPokeInteractor)
        {
            isPoked = false;
          
        }
    }


    public void CheckForPressed(Vector3 buttonPokePos, Vector3 buttonPressPos)
    {
        isPressed = buttonPokePos.y <= buttonPressPos.y;
        if (isPressed)
        {
            if (!wasIsPressed)
            {
                if (OnPressed != null)
                    OnPressed.Invoke();

            }
        }
        else
        {
            if (wasIsPressed)
            {
                if (OnReleased != null)
                    OnReleased.Invoke();
               
            }
        }
        wasIsPressed = isPressed;
    }

    // Takes the poke, pressed, and unpressed positions in the button space
    bool IsValid(Vector3 buttonPoke, Vector3 ButtonUnpressed)
    {
        Vector3 releasedPosButton = WorldPosToButtonPos(unpressedHeight.position);
        Vector3 pokePosButton = WorldPosToButtonPos(pokeAttachTransform.position);

        return (pokePosButton.y <= releasedPosButton.y);
    }    

    // Update is called once per frame
    void Update()
    {
        if (isPoked)
        {
            Vector3 buttonPokePos = WorldPosToButtonPos(pokeAttachTransform.position);
            Vector3 buttonUnpressedPos = WorldPosToButtonPos(unpressedHeight.position);


            if (!IsValid(buttonPokePos, buttonUnpressedPos))
                return;

            
            Vector3 buttonPressedPos = WorldPosToButtonPos(pressedHeight.position);
            Vector3 buttonVectorToSurface = LocalPosToButtonPos(m_VectorToSurface);

            CheckForPressed(buttonPokePos, buttonPressedPos);
           

            Vector3 visualLocalPos =  Vector3.Project(buttonPokePos - buttonVectorToSurface, Vector3.up);
            float maxButtonY = buttonUnpressedPos.y - buttonVectorToSurface.y;
            if (visualLocalPos.y > maxButtonY)
                visualLocalPos.y = maxButtonY;

            float minButtonY = buttonPressedPos.y - buttonVectorToSurface.y;
            if (visualLocalPos.y < minButtonY)
                visualLocalPos.y = minButtonY;

            visualLocalPos = Quaternion.Inverse(buttonUPQuat) * visualLocalPos;
           
            visualTarget.transform.position = transform.TransformPoint(visualLocalPos);  

        }
        else
        {
            visualTarget.transform.position = Vector3.Lerp(visualTarget.transform.position, transform.position, Time.deltaTime * resetSpeed);
        }
    }

    private void OnDrawGizmos()
    {
        if (isPoked)
        {
            float sphereRad = 0.001f;

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(pokeAttachTransform.position, sphereRad);

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(unpressedHeight.position, sphereRad);

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(pressedHeight.position, sphereRad);

            Ray buttonUP = new Ray(pressedHeight.position, transform.TransformDirection(m_ButtonUpDirection));
            Gizmos.DrawRay(buttonUP);

            Ray TestRay = new Ray(pressedHeight.position, transform.TransformDirection(Quaternion.Inverse(buttonUPQuat) * Vector3.up));
            Gizmos.color = Color.red;
            Gizmos.DrawRay(TestRay);

        }
    }
}