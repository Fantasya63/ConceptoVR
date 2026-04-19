using System.Collections;
using UnityEngine;

namespace Concepto
{
    public class HashmapLinkedLists : BaseLinkedLists<HashmapNodeData, HashmapNodePointer, HashmapNode>
    {
       
        // Type command in Inspector 
        [SerializeField] string[] m_Commands;


        Coroutine m_CommandCoroutine;


        void Start()
        {
            m_Size = 0;
            m_TempPtr.gameObject.SetActive(false);
            Debug.Assert(m_Head != null);

            if (m_CommandCoroutine != null)
                StopCoroutine(m_CommandCoroutine);

            //m_CommandCoroutine = StartCoroutine(RunCommand());
        }


        //public IEnumerator RunCommand()
        //{
        //    foreach (string _command in m_Commands)
        //    {
        //        Debug.Log("Command: " + _command);

        //        string[] parts = _command.Split(' ');

        //        string command = parts[0].ToLower();

        //        if (command == "insert")
        //        {
        //            string value = parts[1];
        //            yield return Insert(value);
        //        }
        //        if (command == "insertat")
        //        {
        //            string value = parts[1];
        //            int pos = int.Parse(parts[2]);
        //            yield return Insert(value, pos);
        //        }
        //        else if (command == "delete")
        //        {
        //            int pos = int.Parse(parts[1]);
        //            yield return Delete(pos);
        //        }
        //        else if (command == "traverse")
        //        {
        //            yield return Traverse();
        //        }
        //        else if (command == "traverse_till")
        //        {
        //            int pos = int.Parse(parts[1]);
        //            yield return Traverse(pos);
        //        }
        //        else
        //        {
        //            Debug.Log($"Invalid command: {command}");
        //        }

        //        yield return new WaitForSeconds(0.5f);
        //    }

        //    m_CommandCoroutine = null;
        //    yield break;
        //}

        public IEnumerator Insert(Paper key, Paper value, int pos = -1)
        {
            if (pos < 0)
            {
                pos = m_Size;
                Debug.Log($"Negative Pos detected, retrying to insert at Pos: {pos}");
            }

            HashmapNodeData nodeData = new HashmapNodeData(key, value);


            Debug.Log($"Inserting: {value} at {pos}.");
            HashmapNode newNode = Instantiate(m_HashmapNodePrefab, transform);
            newNode.gameObject.SetActive(false);
            //newNode.Data = value;

            // If head is empty
            if (m_Head.GetData() == null)
            {
                newNode.gameObject.SetActive(true);
                yield return (m_Head.PointTo(newNode));


                yield return newNode.SetDataAnimated(nodeData);

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

            HashmapNodePointer next;
            next = m_Head;
            int currentPos = -1;

            while (currentPos + 1 < pos)
            {
                HashmapNode currentNode = m_Current.GetData();
                bool isEqual = false;

                // Check if they have the same key
                yield return currentNode.AnimatedCheckIfEqual(key,
                    (bool _isEqual) =>
                    {
                        isEqual = _isEqual;
                    }
                );

                if (isEqual)
                {
                    yield return currentNode.AnimatedReplaceValue(key, value);
                    Destroy(key.gameObject);
                    yield break;
                }
                else
                {
                    yield return currentNode.Close();
                }


                // Move to next node
                yield return m_Current.PointTo(currentNode.NextPointer);

                Debug.Log($"Moved to: {currentNode.Data}");
                currentPos++;
                next = currentNode.NextPointer;
            }


            Debug.Log($"Next Pointer Val: {next.GetData()}");



            Vector3 spawnPos = next.GetPointedPosition();
            spawnPos += m_NewNodeSpawnOffset;

            // Prev Node
            HashmapNode lastNode = next.GetData();

            // Show New Node
            newNode.transform.position = spawnPos;
            newNode.gameObject.SetActive(true);
            yield return next.LookAt(newNode);

            if (lastNode != null)
            {
                yield return newNode.NextPointer.LookAt(lastNode);

                // Move old node
                Vector3 startPos = lastNode.transform.position;
                Vector3 endPos = lastNode.NextPointer.GetPointedPosition();

                //lastNode.transform
                lastNode.LeanMoveWithData(endPos, m_AnimDownwardDur);

                bool moved = false;
                LeanTween.move(newNode.gameObject, next.GetPointedPosition(), m_AnimDownwardDur)
                    .setOnUpdate((float val) =>
                    {
                        Debug.Log("Point Update");
                        next.LookAtNoAnim(newNode);
                        newNode.NextPointer.LookAtNoAnim(lastNode);

                    })
                    .setOnComplete(() =>
                    {
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
                    .setOnComplete(() =>
                    {
                        moved = true;
                    }
                );
                yield return new WaitUntil(() => moved);


                next.PointToNoAnim(newNode);
            }

            yield return newNode.SetDataAnimated(nodeData);

            m_Size++;
            m_Current.PointToNoAnim(m_Head);

            Debug.Log(value + " is inserted.");
        }

        public IEnumerator Remove(Paper key, System.Action<bool, string> onFinished)
        {
            if (m_Head.GetData() == null)
            {
                Debug.Log("List is empty.");
                yield break;
            }

            HashmapNode nodeToRemove = null;
            HashmapNodePointer currPointer = null;

            // Check if we are removing at the start of the lists
            {
                HashmapNode node = m_Head.GetData();
                bool isEqual = false;

                // Check if they have the same key
                yield return node.AnimatedCheckIfEqual(key,
                    (bool _isEqual) =>
                    {
                        isEqual = _isEqual;
                    }
                );

                if (isEqual)
                {
                    nodeToRemove = node;
                    currPointer = m_Head;

                    yield return m_Head.GetData().Close();
                }
                else
                {
                    yield return m_Head.GetData().Close();
                    m_Current.PointToNoAnim(m_Head.GetData());

                    bool foundKey = false;

                    while (!foundKey)
                    {
                        // Bounds Check
                        if (m_Current.GetData().NextPointer.GetData() == null)
                        {
                            string message = $"Error: Key is not found: {key.data}";
                            onFinished.Invoke(false, message);
                            yield break;
                        }

                        // Check if they have the same key
                        yield return m_Current.GetData().NextPointer.GetData().AnimatedCheckIfEqual(key,
                            (bool _isEqual) =>
                            {
                                foundKey = _isEqual;
                            }
                        );
                        yield return m_Current.GetData().NextPointer.GetData().Close();


                        if (foundKey)
                        {
                            // Set variables then break the loop
                            currPointer = m_Current.GetData().NextPointer;
                            nodeToRemove = m_Current.GetData().NextPointer.GetData();
                        }
                        else
                        {

                            
                           
                                yield return m_Current.GetData().NextPointer.GetData().Close();
                            
                            // Continue moving
                            yield return m_Current.PointTo(m_Current.GetData().NextPointer);
                        }

                    }
                }
            }

            Debug.Assert(currPointer != null);
            Debug.Assert(nodeToRemove != null);


            HashmapNode nodeToRep = nodeToRemove.NextPointer.GetData(); // Can be null if deleting the last node in the list
            {
                m_TempPtr.gameObject.SetActive(true);
                m_TempPtr.PointToNoAnim(m_Current.GetData().NextPointer);

                // Animate node upwards
                bool moved = false;
                nodeToRemove.gameObject.LeanMove(nodeToRemove.transform.position + m_DelNodeMoveOffset, m_NodeMoveAnimDur)
                    .setOnUpdate((float time) =>
                    {
                        currPointer.LookAtNoAnim(nodeToRemove);
                        m_TempPtr.PointToNoAnim(nodeToRemove);

                        if (nodeToRep != null)
                            nodeToRemove.NextPointer.LookAtNoAnim(nodeToRep);
                    })
                    .setOnComplete(() => moved = true);

                yield return new WaitUntil(() => moved);
            }

            {
                // Set current node's next to the nodeToRep
                yield return currPointer.LookAt(nodeToRep);

                // Delete Node to delete
                Destroy(nodeToRemove.Data.key.gameObject);
                Destroy(nodeToRemove.Data.value.gameObject);
                Destroy(nodeToRemove.gameObject);
                m_TempPtr.gameObject.SetActive(false);
                m_Size--;


                if (nodeToRep != null)
                {
                    Vector3 _pos = currPointer.GetPointedPosition();
                    nodeToRep.LeanMoveWithData(_pos, m_NodeMoveAnimDur);
                    yield return new WaitForSeconds(m_NodeMoveAnimDur);
                }

                yield return new WaitForSeconds(0.2f);
            }

            onFinished.Invoke(true, "Operation is successful");

        }


        public IEnumerator Retrieve(Paper key, System.Action<bool, string, Paper> onFinished)
        {
            if (m_Head.GetData() == null)
            {
                Debug.Log("List is empty.");
                yield break;
            }

            // Check if we are removing at the start of the lists
            {
                HashmapNode node = m_Head.GetData();
                bool isEqual = false;

                // Check if they have the same key
                yield return node.AnimatedCheckIfEqual(key,
                    (bool _isEqual) =>
                    {
                        isEqual = _isEqual;
                    }
                );
                yield return m_Head.GetData().Close();

                if (isEqual)
                {
                    onFinished.Invoke(true, "Operation is successful", node.Data.value);
                    yield break;
                }
                else
                {
                    m_Current.PointToNoAnim(m_Head.GetData());

                    bool foundKey = false;

                    while (!foundKey)
                    {
                        // Bounds Check
                        if (m_Current.GetData().NextPointer.GetData() == null)
                        {
                            string message = $"Error: Key is not found: {key.data}";
                            onFinished.Invoke(false, message, null);
                            yield break;
                        }

                        // Check if they have the same key
                        yield return m_Current.GetData().NextPointer.GetData().AnimatedCheckIfEqual(key,
                            (bool _isEqual) =>
                            {
                                foundKey = _isEqual;
                            }
                        );
                        yield return m_Current.GetData().NextPointer.GetData().Close();


                        if (foundKey)
                        {
                            onFinished.Invoke(true, "Operation is successful", m_Current.GetData().NextPointer.GetData().Data.value);
                            yield break;
                        }
                        else
                        {



                            yield return m_Current.GetData().NextPointer.GetData().Close();

                            // Continue moving
                            yield return m_Current.PointTo(m_Current.GetData().NextPointer);
                        }

                    }
                }
            }
        }
    }
}
