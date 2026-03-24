using System.Collections;
using UnityEngine;

namespace Concepto
{
    // TODO: MOVE CURRENT TO THE VACANT SPACE INSTEAD
    public class SpatialLinkedLists : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] SpatialPointer m_Head;
        [SerializeField] SpatialPointer m_Current;

        [Header("Prefabs")]
        [SerializeField] SpatialNode m_SpatialNodePrefab;

        // Type command in Inspector 
        public string[] commands;

        Coroutine m_CommandCoroutine;

        void Start()
        {
            Debug.Assert(m_Head != null);
            
            if (m_CommandCoroutine != null)
                StopCoroutine(m_CommandCoroutine);

            m_CommandCoroutine = StartCoroutine(RunCommand());
        }

        // Button
        public IEnumerator RunCommand()
        {
            foreach (string _command in commands)
            {
                Debug.Log("Command: " + _command);

                string[] parts = _command.Split(' ');

                string command = parts[0].ToLower();

                if (command == "insert")
                {
                    int value = int.Parse(parts[1]);
                    yield return Insert(value);
                }
                //else if (command == "delete")
                //{
                //    int value = int.Parse(parts[1]);
                //    Delete(value);
                //}
                //else if (command == "traverse")
                //{
                //    Traverse();
                //}
                else
                {
                    Debug.Log("Invalid command.");
                }

                yield return new WaitForSeconds(0.5f);
            }
            
        }

        //void Insert(int value)
        //{
        //    SpatialNode newNode = Instantiate(m_SpatialNodePrefab);
        //    newNode.m_Data = value.ToString();

        //    //Node newNode = new Node(value);

        //    if (m_Head.GetData() == null)
        //    {
        //        m_Head.PointTo(newNode);
        //        Debug.Log(value + " is inserted. (Head)");
        //        return;
        //    }

        //    m_Current.PointTo(m_Head.GetData());

        //    while (m_Current.GetData() != null)
        //    {
        //        SpatialNode currentNode = m_Current.GetData();
        //        if (currentNode.m_NextPointer.GetData() == null)
        //        {
        //            currentNode.m_NextPointer.PointTo(newNode);
        //            break;
        //        }
        //        m_Current.PointTo(m_Current.GetData().m_NextPointer.GetData());

        //        //temp = temp.next;
        //    }

        //    m_Current.GetData().m_NextPointer.PointTo(newNode);

        //    //m_Current.GetData().m_NextPointer.PointTo(newNode);
        //    ////temp.next = newNode;
        //    Debug.Log(value + " is inserted.");
        //}

        public IEnumerator Insert(int value)
        {

            SpatialNode newNode = Instantiate(m_SpatialNodePrefab);
            newNode.gameObject.SetActive(false);
            newNode.m_Data = value.ToString();

            // If head is empty
            if (m_Head.GetData() == null)
            {
                newNode.gameObject.SetActive(true);
                yield return (m_Head.PointTo(newNode));
                Debug.Log(value + " is inserted. (Head)");
                yield break;
            }

            // Move current pointer to head
            m_Current.PointToNoAnim(m_Head.GetData());
            //yield return StartCoroutine(m_Current.PointTo(m_Head.GetData()));

            while (m_Current.GetData() != null)
            {
                SpatialNode currentNode = m_Current.GetData();

                // If next is null  insert here
                if (currentNode.m_NextPointer.GetData() == null)
                {
                    newNode.gameObject.SetActive(true);
                    yield return (currentNode.m_NextPointer.PointTo(newNode));
                    Debug.Log(value + " is inserted.");
                    yield break;
                }

                // Move to next node
                yield return 
                    m_Current.PointTo(currentNode.m_NextPointer.GetData())
                ;

                Debug.Log($"Moved to: {currentNode.m_Data}");
            }


            m_Current.PointToNoAnim(m_Head.GetData());

            Debug.Log(value + " is inserted.");
        }

        //void Delete(int value)
        //{
        //    if (head == null)
        //    {
        //        Debug.Log("List is empty.");
        //        return;
        //    }

        //    if (head.data == value)
        //    {
        //        head = head.next;

        //        if (head == null)
        //            Debug.Log(value + " is deleted. List is now empty.");
        //        else
        //            Debug.Log(value + " is deleted.");

        //        return;
        //    }

        //    Node temp = head;

        //    while (temp.next != null && temp.next.data != value)
        //    {
        //        temp = temp.next;
        //    }

        //    if (temp.next == null)
        //    {
        //        Debug.Log(value + " not found.");
        //    }
        //    else
        //    {
        //        temp.next = temp.next.next;

        //        if (head == null)
        //            Debug.Log(value + " is deleted. List is now empty.");
        //        else
        //            Debug.Log(value + " is deleted.");
        //    }
        //}

        //void Traverse()
        //{
        //    if (head == null)
        //    {
        //        Debug.Log("List is empty.");
        //        return;
        //    }

        //    Node temp = head;
        //    string output = "List: ";

        //    while (temp != null)
        //    {
        //        output += temp.data + " -> ";
        //        temp = temp.next;
        //    }

        //    output += "NULL";

        //    Debug.Log(output);
        //}
    }
}
