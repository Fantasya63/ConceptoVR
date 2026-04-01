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
        [SerializeField] SpatialPointer m_TempPtr;
        [SerializeField] AudioSource m_AudioSource;
        [SerializeField] AudioClip m_ErrorClip;

        [Header("Options")]
        [SerializeField] Vector3 m_NewNodeSpawnOffset = Vector3.up * 0.25f;
        [SerializeField] Vector3 m_DelNodeMoveOffset = Vector3.up * 0.25f;
        [SerializeField] float m_AnimDownwardDur = 1.0f;
        [SerializeField] float m_PointerLookLerpDur = 0.5f;
        [SerializeField] float m_NodeMoveAnimDur = 0.5f;

        [Header("Prefabs")]
        [SerializeField] SpatialNode m_SpatialNodePrefab;

        // Type command in Inspector 
        [SerializeField] string[] m_Commands;

        public int Size { 
            get
            {
                return m_Size;
            }
        }

        Coroutine m_CommandCoroutine;
        int m_Size = 0;


        void Start()
        {
            Debug.Assert(m_Head != null);
            
            if (m_CommandCoroutine != null)
                StopCoroutine(m_CommandCoroutine);

            m_CommandCoroutine = StartCoroutine(RunCommand());
        }


        public IEnumerator RunCommand()
        {
            foreach (string _command in m_Commands)
            {
                Debug.Log("Command: " + _command);

                string[] parts = _command.Split(' ');

                string command = parts[0].ToLower();

                if (command == "insert")
                {
                    int value = int.Parse(parts[1]);
                    yield return Insert(value);
                }
                if (command == "insertat")
                {
                    int value = int.Parse(parts[1]);
                    int pos = int.Parse(parts[2]);
                    yield return Insert(value, pos);
                }
                else if (command == "delete")
                {
                    int pos = int.Parse(parts[1]);
                    yield return Delete(pos);
                }
                else if (command == "traverse")
                {
                    yield return Traverse();
                }
                else
                {
                    Debug.Log($"Invalid command: {command}");
                }

                yield return new WaitForSeconds(0.5f);
            }
            
        }

        public IEnumerator Insert(int value, int pos = -1)
        {
            if (pos < 0)
            {
                pos = m_Size;
                Debug.Log($"Negative Pos detected, retrying to insert at Pos: {pos}");
            }

            Debug.Log($"Inserting: {value} at {pos}.");
            SpatialNode newNode = Instantiate(m_SpatialNodePrefab);
            newNode.gameObject.SetActive(false);
            newNode.Data = value.ToString();

            // If head is empty
            if (m_Head.GetData() == null)
            {
                newNode.gameObject.SetActive(true);
                yield return (m_Head.PointTo(newNode));
                Debug.Log(value + " is inserted. (Head)");

                m_Size++;
                yield break;
            }

            // Check bounds
            if (pos < 0 || pos > m_Size)
            {
                Debug.LogWarning($"Aborting Insert of {value} at pos: {pos}. Pos is out of bounds with list size of: {m_Size}");

                if (m_AudioSource == null || m_ErrorClip == null)
                    yield break;
                      
                m_AudioSource.clip = m_ErrorClip; 
                m_AudioSource.Play();
                yield break;
            }

            // Move current pointer to head
            m_Current.PointToNoAnim(m_Head.GetData());

            SpatialPointer next;
            next = m_Head;
            int currentPos = -1;

            while (currentPos + 1 < pos)
            {
                SpatialNode currentNode = m_Current.GetData();

                // Move to next node
                yield return m_Current.PointTo(currentNode.m_NextPointer);

                Debug.Log($"Moved to: {currentNode.Data}");
                currentPos++;
                next = currentNode.m_NextPointer;
            }


            Debug.Log($"Next Pointer Val: {next.GetData()}");

            

            Vector3 spawnPos = next.GetPointedPosition();
            spawnPos += m_NewNodeSpawnOffset;

            // Prev Node
            SpatialNode lastNode = next.GetData();

            // Show New Node
            newNode.transform.position = spawnPos;
            newNode.gameObject.SetActive(true);
            yield return next.LookAt(newNode);

            if (lastNode != null)
            {
                yield return newNode.m_NextPointer.LookAt(lastNode);

                // Move old node
                Vector3 startPos = lastNode.transform.position;
                Vector3 endPos = lastNode.m_NextPointer.GetPointedPosition();

                //lastNode.transform
                lastNode.LeanMove(endPos, m_AnimDownwardDur);

                bool moved = false;
                LeanTween.move(newNode.gameObject, next.GetPointedPosition(), m_AnimDownwardDur)
                    .setOnUpdate((float val) =>
                    {
                        Debug.Log("Point Update");
                        next.LookAtNoAnim(newNode);
                        newNode.m_NextPointer.LookAtNoAnim(lastNode);

                    })
                    .setOnComplete(() => {
                        moved = true;
                    }
                );
                yield return new WaitUntil(() => moved);
            }
            else
            {

                bool moved = false;
                LeanTween.move(newNode.gameObject, next.GetPointedPosition(), m_AnimDownwardDur)
                    .setOnUpdate((float val) =>
                    {
                        Debug.Log("Point Update");
                        next.LookAtNoAnim(newNode);
                    })
                    .setOnComplete(() => { 
                        moved = true;
                    }
                );
                yield return new WaitUntil(() => moved);


                next.PointToNoAnim(newNode);
            }

            m_Size++;
            m_Current.PointToNoAnim(m_Head);

            Debug.Log(value + " is inserted.");
        }

        IEnumerator Delete(int pos)
        {
            m_TempPtr.gameObject.SetActive(false);

            if (m_Head.GetData() == null)
            {
                Debug.Log("List is empty.");
                yield break;
            }

            // Bounds Check
            if (pos < 0 || pos >= m_Size)
            {
                Debug.Log($"Aborting Deletion at pos: {pos}. Pos it out of bounds. Size: {m_Size}");
                yield break;
            }
            

            int currentPos = 0;
            m_Current.PointToNoAnim(m_Head);

            while (currentPos + 1 < pos)
            {
                yield return m_Current.PointTo(m_Current.GetData().m_NextPointer);
                currentPos++;
            }

            SpatialNode currentNode = m_Current.GetData();
            SpatialNode nodeToDelete = currentNode.m_NextPointer.GetData();
            
            Debug.Assert(currentNode != null);
            Debug.Assert(nodeToDelete != null);

            SpatialNode nodeToRep = nodeToDelete.m_NextPointer.GetData(); // Can be null if deleting the last node in the list

            {
                m_TempPtr.gameObject.SetActive(true);
                m_TempPtr.PointToNoAnim(m_Current.GetData().m_NextPointer);

                // Animate node upwards
                bool moved = false;
                nodeToDelete.gameObject.LeanMove(nodeToDelete.transform.position + m_DelNodeMoveOffset, m_NodeMoveAnimDur)
                    .setOnUpdate((float time) => {
                        currentNode.m_NextPointer.LookAtNoAnim(nodeToDelete);
                        m_TempPtr.PointToNoAnim(nodeToDelete);

                        if (nodeToRep != null)
                            nodeToDelete.m_NextPointer.LookAtNoAnim(nodeToRep);
                    })
                    .setOnComplete(() => moved = true);

                yield return new WaitUntil(() => moved);
            }

            {
                // Set current node's next to the nodeToRep
                yield return currentNode.m_NextPointer.LookAt(nodeToRep);

                // Delete Node to delete
                Destroy(nodeToDelete.gameObject);
                m_TempPtr.gameObject.SetActive(false);
                m_Size--;


                if (nodeToRep != null)
                {
                    Vector3 _pos = currentNode.m_NextPointer.GetPointedPosition();
                    nodeToRep.LeanMove(_pos, m_NodeMoveAnimDur);
                    yield return new WaitForSeconds(m_NodeMoveAnimDur);
                }

                yield return new WaitForSeconds(0.2f);
            }
        }


        IEnumerator Traverse()
        {
            if (m_Head.GetData() == null)
            {
                Debug.Log("List is empty.");
                yield break;
            }

            m_Current.PointToNoAnim(m_Head);

            //Node temp = head;

            string output = "List: ";

            while (m_Current.GetData() != null)
            {
                output += m_Current.GetData().Data + " -> ";
                //temp = temp.next;
                yield return m_Current.PointTo(m_Current.GetData().m_NextPointer);
            }

            output += "NULL";

            m_Current.PointToNoAnim(m_Head);
            Debug.Log(output);
        }
    }
}
