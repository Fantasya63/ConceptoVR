using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class KeyboardController : MonoBehaviour
{
    public float cycleTime = 1.5f;
    public int maxCharacters = 4;
    public List<Key> keys;

    public Printer printer;

    [SerializeField]
    private TextDisplay display;

    private Dictionary<char, Key> keyMap = new Dictionary<char, Key>();
    private Key lastKey = null;
    private float lastPressTime = 0f;
    private List<char> typed = new List<char>();

    void Start()
    {
        keyMap.Clear();

        foreach (var key in keys)
        {
            if (!keyMap.ContainsKey(key.keyChar))
                keyMap.Add(key.keyChar, key);
            else
                Debug.LogWarning("Duplicate key detected: " + key.keyChar);
        }

        UpdateDisplay();
    }

    public void PressKey(string keyValue)
    {
       

        if (string.IsNullOrEmpty(keyValue))
            return;

        char c = keyValue[0];

        if (!keyMap.ContainsKey(c))
        {
            Debug.LogWarning("Key not found: " + c);
            return;
        }

        HandleKeyPress(keyMap[c]);
    }

    public void PressBackspace()
    {
        HandleBackspace();
    }

    public void PressSubmit()
    {
        HandleSubmit();
    }

    void HandleKeyPress(Key key)
    {
        float timeSinceLast = Time.time - lastPressTime;

        if (key == lastKey && timeSinceLast < cycleTime && typed.Count > 0)
        {
            key.Cycle();
            typed[typed.Count - 1] = key.CurrentValue;
        }
        else
        {
            if (typed.Count >= maxCharacters)
            {
                Debug.LogWarning("Character limit reached.");
                return;
            }

            key.Reset();
            typed.Add(key.CurrentValue);
        }

        lastKey = key;
        lastPressTime = Time.time;

        UpdateDisplay();
    }

    void HandleBackspace()
    {
        if (typed.Count > 0)
        {
            typed.RemoveAt(typed.Count - 1);
            UpdateDisplay();
        }
        else
        {
            Debug.LogWarning("Nothing to delete!");
        }
    }

    void HandleSubmit()
    {
        if (typed.Count == 0)
        {
            Debug.LogError("Error: Empty input!");
            return;
        }

        string output = GetTypedString();
        Debug.Log("Submitted: " + output);

        if (printer != null)
        {
            printer.Print(output);
        }
        else
        {
            Debug.LogError("Printer not assigned!");
        }

        typed.Clear();
        lastKey = null;
        UpdateDisplay(); 
    }

    void UpdateDisplay()
    {
        if (display != null)
        {
            display.DisplayText = GetTypedString();
        }
        else
        {
            Debug.LogWarning("Display not assigned!");
        }
    }

    string GetTypedString()
    {
        return new string(typed.ToArray());
    }
}