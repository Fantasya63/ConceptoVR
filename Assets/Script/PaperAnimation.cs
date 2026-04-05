using UnityEngine;
using System.Collections.Generic;

public class PaperAnimation : MonoBehaviour
{
    public GameObject paper1;
    public GameObject paper2;

    public Transform paper1Start;
    public Transform paper1End;

    public Transform paper2Start;
    public Transform paper2End;

    public float duration = 1.5f;

    private bool isUp = false;

    private List<string> nodeData = new List<string>();

    public void TogglePaper()
    {
        Debug.Log("BUTTON CLICKED");

        if (!isUp)
        {
            LeanTween.move(paper1, paper1End.position, duration)
                .setEaseOutBack();

            LeanTween.move(paper2, paper2End.position, duration)
                .setEaseOutBack();

            nodeData.Clear();
            nodeData.Add("Paper 1 Value");
            nodeData.Add("Paper 2 Position");

            Debug.Log("Inserted node");
        }
        else
        {
            LeanTween.move(paper1, paper1Start.position, duration)
                .setEaseInBack();

            LeanTween.move(paper2, paper2Start.position, duration)
                .setEaseInBack();

            Debug.Log("Returned papers");
        }

        isUp = !isUp;
    }

    public void DeleteNode()
    {
        Debug.Log("DELETE BUTTON CLICKED");

        LeanTween.move(paper1, paper1Start.position, duration)
            .setEaseInBack();

        LeanTween.move(paper2, paper2Start.position, duration)
            .setEaseInBack();

        nodeData.Clear();

        isUp = false;

        Debug.Log("Node deleted");
    }

    public void TraverseNode()
    {
        Debug.Log("TRAVERSE BUTTON CLICKED");
        Debug.Log("Traversing Linked List:");

        if (nodeData.Count == 0)
        {
            Debug.Log("No data found");
            return;
        }

        foreach (string item in nodeData)
        {
            Debug.Log(item);
        }
    }
}