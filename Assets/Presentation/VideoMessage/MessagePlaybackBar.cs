using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MessagePlaybackBar : MonoBehaviour
{
    [SerializeField]
    private Slider m_Slider;

    [SerializeField]
    private TMPro.TMP_Text m_CurrentText;

    [SerializeField]
    private TMPro.TMP_Text m_EndText;

    private Coroutine m_TimerRoutine;

    public void StartPlayback(float time, float current = 0.0f)
    {
        Debug.Assert(time >= 0.0f);

        m_Slider.minValue = 0.0f;
        m_Slider.maxValue = time;
        m_Slider.value = current;

        m_EndText.text = TimeToString(time);
        m_CurrentText.text = TimeToString(current);

        if (m_TimerRoutine != null)
            StopCoroutine(m_TimerRoutine);

        m_TimerRoutine = StartCoroutine(CountRoutine());
    }

    string TimeToString(float time)
    {
        int totalMinutes = (int)(time / 60);
        int totalSeconds = (int)(time % 60);
        return $"{totalMinutes}:{totalSeconds:00}";
    }

    private IEnumerator CountRoutine()
    {
        while (m_Slider.value < m_Slider.maxValue)
        {
            m_Slider.value += Time.deltaTime;
            m_CurrentText.text = TimeToString(m_Slider.value);
            yield return null;
        }

        m_Slider.value = m_Slider.maxValue;
        m_CurrentText.text = TimeToString(m_Slider.maxValue);
    }
}