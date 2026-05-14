using TMPro;
using UnityEngine;

[ExecuteAlways]
public class ScriptVisualizer : MonoBehaviour
{
    
    public string Code
    {
        set
        {
            code = value;
            UpdateText();
        }
    }

    public void SetCodeWithNotif(string _code)
    {
        Code = _code;
        m_VisualizerAudioSource.clip = m_NotifClip;
        m_VisualizerAudioSource.Play();
    }

    [SerializeField]
    [TextArea(5, 20)]
    private string code = "print(\"Hello World\");";

    [SerializeField] private TMP_Text codeUI;
    [SerializeField] private AudioSource m_VisualizerAudioSource;
    [SerializeField] private AudioClip m_NotifClip;

    private void OnEnable()
    {
        UpdateText();
    }

    private void OnValidate()
    {
        UpdateText();
    }

    private void UpdateText()
    {
        if (codeUI == null) return;

        codeUI.text = CodeHighlighter.Highlight(code);
    }
}