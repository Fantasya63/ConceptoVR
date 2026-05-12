using Canvas;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StepsButton : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler
{
    [Header("References")]
    //[SerializeField] SlidesManager slideManager;
    [SerializeField] Step step;
    [SerializeField] private Image panelBg;
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private Image numImage;
    [SerializeField] private TMP_Text numText;

    [Header("Normal Colors")]
    [SerializeField] private Color normalBgColor = Color.white;
    [SerializeField] private Color normalTextColor = Color.black;
    [SerializeField] private Color normalNumTextColor = Color.white;
    [SerializeField] private Color normalNumImageColor = Color.black;

    [Header("Hover Colors")]
    [SerializeField] private Color hoverBgColor = Color.gray;
    [SerializeField] private Color hoverTextColor = Color.white;

    [Header("Pressed Colors")]
    [SerializeField] private Color pressedBgColor = Color.black;
    [SerializeField] private Color pressedTextColor = Color.yellow;

    [Header("Current Colors")]
    [SerializeField] private Color currentBgColor = Color.white;
    [SerializeField] private Color currentTextColor = Color.black;
    [SerializeField] private Color currentNumTextColor = Color.white;
    [SerializeField] private Color currentNumImageColor = Color.black;

    [Header("Events")]
    
    public UnityEvent onPressed;
    bool isCurrent = false;

    void Start()
    {
        //Debug.Assert(slideManager != null);
        Debug.Assert(buttonText != null);
        Debug.Assert(numImage != null);
        Debug.Assert(numText != null);
        Debug.Assert(panelBg != null);
        Debug.Assert(step != null);

        step.slide.manager.OnNextStepEvent.AddListener(OnManagerNextStep);

        //slideManager.OnNextStepEvent.AddListener(OnManagerNextStep);
        CheckIfCurrent();
        ResetToDefault();
    }

    void CheckIfCurrent()
    {
        Step _currentStep = null;
        _currentStep = step.slide.manager.CurrentSlide?.CurrentStep;
        if (_currentStep == null)
        {
            isCurrent = false;
            return;
        }

        isCurrent = step == _currentStep;
    }

    void OnManagerNextStep(Step _step)
    {
        isCurrent = step == _step;
        ResetToDefault();

        Debug.Log($"OnManagerNextStep: step: {step}, _step: {_step}");
    }

    void ResetToDefault()
    {
        numText.text = (step.GetIndex() + 1).ToString();
        buttonText.text = step.name;

        if (isCurrent)
        {
            panelBg.color = currentBgColor;
            buttonText.color = currentTextColor;
            numImage.color = currentNumImageColor;
            numText.color = currentNumTextColor;
        }
        else
        {
            panelBg.color = normalBgColor;
            buttonText.color = normalTextColor;

            numImage.color = normalNumImageColor;
            numText.color = normalNumTextColor;
        }
       
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        panelBg.color = hoverBgColor;
        buttonText.color = hoverTextColor;

        numImage.color = hoverBgColor;
        numText.color = hoverTextColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
       ResetToDefault();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        panelBg.color = pressedBgColor;
        buttonText.color = pressedTextColor;

        numImage.color = pressedBgColor;
        numText.color = pressedTextColor;

        onPressed?.Invoke();
        step.slide.manager.JumpToStep(step);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Return to hover state if still hovering
        panelBg.color = hoverBgColor;
        buttonText.color = hoverTextColor;

        numImage.color = hoverBgColor;
        numText.color = hoverTextColor;
    }
}