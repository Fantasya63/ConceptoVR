using UnityEngine;

public class ToggleButton : MonoBehaviour
{
    [SerializeField]
    bool m_IsToggled = false;

    [SerializeField] GameObject m_Letters;
    [SerializeField] GameObject m_Numbers;

    private void Start()
    {
        UpdateState();
    }

    private bool SetToggle
    {
        set 
        { 
            m_IsToggled = value;
            UpdateState();
        }
        get { return m_IsToggled; }
    }

    void UpdateState()
    {
        m_Numbers.SetActive(!m_IsToggled);
        m_Letters.SetActive(m_IsToggled);
    }

    public void Toggle()
    {
        SetToggle = !SetToggle;
    }
}
