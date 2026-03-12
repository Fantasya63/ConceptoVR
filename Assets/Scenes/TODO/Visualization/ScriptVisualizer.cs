using TMPro;
using UnityEngine;

[ExecuteAlways]
public class ScriptVisualizer : MonoBehaviour
{
    [TextArea(5, 20)]
    public string Code
    {
        set
        {
            code = value;
            UpdateText();
        }
    }

    [SerializeField]
    private string code = "print(\"Hello World\");";

    [SerializeField] private TMP_Text codeUI;

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