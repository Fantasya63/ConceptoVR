using TMPro;
using UnityEngine;

public class ErrorPanel : MonoBehaviour
{
    [SerializeField] TMP_Text m_ErrorText;
    [SerializeField] float m_ShowDur = 5.0f;
    [SerializeField] float m_GrowDur = 0.5f;
    [SerializeField] GameObject m_UIHolder;


    private Vector3 m_OriginalScale;

    private void Start()
    {
        m_OriginalScale = m_UIHolder.transform.localScale;
        m_UIHolder.transform.localScale = Vector3.zero;
        m_UIHolder.SetActive(false);
    }

    public void ShowError(string message)
    {
        // Cancel any ongoing tweens to avoid overlap
        LeanTween.cancel(m_UIHolder);

        m_ErrorText.text = message;
        m_UIHolder.SetActive(true);

        // Reset scale before animating
        m_UIHolder.transform.localScale = Vector3.zero;

        // Grow animation
        LeanTween.scale(m_UIHolder, m_OriginalScale, m_GrowDur)
            .setEaseOutBack()
            .setOnComplete(() =>
            {
                // Wait, then hide
                LeanTween.delayedCall(m_UIHolder, m_ShowDur, () =>
                {
                    // Shrink back
                    LeanTween.scale(m_UIHolder, Vector3.zero, m_GrowDur)
                        .setEaseInBack()
                        .setOnComplete(() =>
                        {
                            m_UIHolder.SetActive(false);
                        });
                });
            });
    }
}