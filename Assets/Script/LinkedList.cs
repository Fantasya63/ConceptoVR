using UnityEngine;

public class Node
{
    public int data; 
    public Node next;

    public Node(int value)
    {
        data = value;
        next = null;
    }
}

public class LinkedList : MonoBehaviour
{
    Node head = null;

    // Type command in Inspector 
    public string userInput;

    void Start()
    {
        
    }

    // Button
    public void RunCommand()
    {
        Debug.Log("Command: " + userInput);

        string[] parts = userInput.Split(' ');

        string command = parts[0].ToLower();

        if (command == "insert")
        {
            int value = int.Parse(parts[1]);
            Insert(value);
        }
        else if (command == "delete")
        {
            int value = int.Parse(parts[1]);
            Delete(value);
        }
        else if (command == "traverse")
        {
            Traverse();
        }
        else
        {
            Debug.Log("Invalid command.");
        }
    }

    void Insert(int value)
    {
        Node newNode = new Node(value);

        if (head == null)
        {
            head = newNode;
            Debug.Log(value + " is inserted. (Head)");
            return;
        }

        Node temp = head;

        while (temp.next != null)
        {
            temp = temp.next;
        }

        temp.next = newNode;
        Debug.Log(value + " is inserted.");
    }

    void Delete(int value)
    {
        if (head == null)
        {
            Debug.Log("List is empty.");
            return;
        }

        if (head.data == value)
        {
            head = head.next;

            if (head == null)
                Debug.Log(value + " is deleted. List is now empty.");
            else
                Debug.Log(value + " is deleted.");

            return;
        }

        Node temp = head;

        while (temp.next != null && temp.next.data != value)
        {
            temp = temp.next;
        }

        if (temp.next == null)
        {
            Debug.Log(value + " not found.");
        }
        else
        {
            temp.next = temp.next.next;

            if (head == null)
                Debug.Log(value + " is deleted. List is now empty.");
            else
                Debug.Log(value + " is deleted.");
        }
    }

    void Traverse()
    {
        if (head == null)
        {
            Debug.Log("List is empty.");
            return;
        }

        Node temp = head;
        string output = "List: ";

        while (temp != null)
        {
            output += temp.data + " -> ";
            temp = temp.next;
        }

        output += "NULL";

        Debug.Log(output);
    }
}