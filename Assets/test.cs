using UnityEngine;

public class test : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("ENter");
    }
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Exit");
    }

}
