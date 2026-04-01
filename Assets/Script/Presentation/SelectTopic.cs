using UnityEngine;

using Canvas;

public class SelectTopic : MonoBehaviour
{
    [SerializeField]
    private Slides m_HashmapSlide;

    [SerializeField]
    private Slides m_LinkedListsSlide;

    public void GoToHashmap()
    {
        m_HashmapSlide.manager.JumpToSlide(m_HashmapSlide.SlideName, true);
    }

    public void GoToLinkedLists()
    {
        m_LinkedListsSlide.manager.JumpToSlide(m_LinkedListsSlide.SlideName, true);
    }
}
