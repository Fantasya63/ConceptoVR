using Canvas;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ControlsButton : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler
{
    [Header("References")]
    //[SerializeField] SlidesManager slideManager;
    [SerializeField] private Image iconsImage;
    [SerializeField] private Image bgImage;

    [Header("Normal Colors")]
    [SerializeField] private Color normalBgColor = Color.white;
    [SerializeField] private Color normalIconColor = Color.black;

    [Header("Hover Colors")]
    [SerializeField] private Color hoverBgColor = Color.gray;
    [SerializeField] private Color hoverIconColor = Color.white;

    [Header("Pressed Colors")]
    [SerializeField] private Color pressedBgColor = Color.black;
    [SerializeField] private Color pressedIconColor = Color.yellow;


    [Header("Events")]

    public UnityEvent onPressed;

    void Start()
    {
        //Debug.Assert(slideManager != null);
        Debug.Assert(bgImage != null);
        Debug.Assert(iconsImage != null);
        
    }

    void ResetToDefault()
    {
        iconsImage.color = normalIconColor;
        bgImage.color = normalBgColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        iconsImage.color = hoverIconColor;
        bgImage.color = hoverBgColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ResetToDefault();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        iconsImage.color = pressedIconColor;
        bgImage.color = pressedBgColor;

        onPressed?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Return to hover state if still hovering
        iconsImage.color = hoverIconColor;
        bgImage.color = hoverBgColor;
    }
}