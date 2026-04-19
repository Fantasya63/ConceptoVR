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
                lastNode.LeanMove(endPos, m_AnimDownwardDur);

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

        //public IEnumerator InsertAtPosNarrate(int value, AudioSource voiceSource, AudioClip finallyClip)
        //{
        //    SpatialNode newNode = Instantiate(m_SpatialNodePrefab, transform);
        //    newNode.gameObject.SetActive(false);
        //    newNode.Data = value.ToString();


        //    SpatialNode currentNode = m_Current.GetData();
        //    SpatialPointer next = currentNode.NextPointer;
        //    Debug.Log($"Next Pointer Val: {next.GetData()}");



        //    Vector3 spawnPos = next.GetPointedPosition();
        //    spawnPos += m_NewNodeSpawnOffset;

        //    // Prev Node
        //    SpatialNode lastNode = next.GetData();

        //    // Show New Node
        //    newNode.transform.position = spawnPos;
        //    newNode.gameObject.SetActive(true);

        //    if (lastNode != null)
        //    {
        //        yield return newNode.NextPointer.LookAt(lastNode);

        //        yield return new WaitUntil(() => !voiceSource.isPlaying);

        //        voiceSource.clip = finallyClip;
        //        voiceSource.Play();
        //        yield return new WaitUntil(() => !voiceSource.isPlaying);


        //        yield return next.LookAt(newNode);


        //        // Move old node
        //        Vector3 startPos = lastNode.transform.position;
        //        Vector3 endPos = lastNode.NextPointer.GetPointedPosition();

        //        //lastNode.transform
        //        lastNode.LeanMove(endPos, m_AnimDownwardDur);

        //        bool moved = false;
        //        LeanTween.move(newNode.gameObject, next.GetPointedPosition(), m_AnimDownwardDur)
        //            .setOnUpdate((float val) =>
        //            {
        //                Debug.Log("Point Update");
        //                next.LookAtNoAnim(newNode);
        //                newNode.NextPointer.LookAtNoAnim(lastNode);

        //            })
        //            .setOnComplete(() => {
        //                moved = true;
        //            }
        //        );
        //        yield return new WaitUntil(() => moved);
        //    }
        //    else
        //    {

        //        bool moved = false;
        //        LeanTween.move(newNode.gameObject, next.GetPointedPosition(), m_AnimDownwardDur)
        //            .setOnUpdate((float val) =>
        //            {
        //                Debug.Log("Point Update");
        //                next.LookAtNoAnim(newNode);
        //            })
        //            .setOnComplete(() => {
        //                moved = true;
        //            }
        //        );
        //        yield return new WaitUntil(() => moved);


        //        next.PointToNoAnim(newNode);
        //    }

        //    m_Size++;
        //    m_Current.PointToNoAnim(m_Head);

        //    Debug.Log(value + " is inserted.");
        ////}

        //public IEnumerator Delete(int pos)
        //{
        //    if (pos < 0)
        //        pos = m_Size - 1;

        //    m_TempPtr.gameObject.SetActive(false);

        //    if (m_Head.GetData() == null)
        //    {
        //        Debug.Log("List is empty.");
        //        yield break;
        //    }

        //    // Bounds Check
        //    if (pos >= m_Size)
        //    {
        //        Debug.Log($"Aborting Deletion at pos: {pos}. Pos it out of bounds. Size: {m_Size}");
        //        yield break;
        //    }

        //    SpatialPointer currPointer = null;
        //    SpatialNode nodeToDelete = null;
        //    // If deleting from the start
        //    if (pos == 0)
        //    {
        //        currPointer = m_Head;
        //        nodeToDelete = m_Head.GetData();
        //    }
        //    else
        //    {

        //        int currentPos = 0;
        //        m_Current.PointToNoAnim(m_Head);

        //        while (currentPos + 1 < pos)
        //        {
        //            yield return m_Current.PointTo(m_Current.GetData().NextPointer);
        //            currentPos++;
        //        }

        //        currPointer = m_Current.GetData().NextPointer;
        //        nodeToDelete = currPointer.GetData();
        //    }


        //    Debug.Assert(currPointer != null);
        //    Debug.Assert(nodeToDelete != null);

        //    SpatialNode nodeToRep = nodeToDelete.NextPointer.GetData(); // Can be null if deleting the last node in the list
        //    {
        //        m_TempPtr.gameObject.SetActive(true);
        //        m_TempPtr.PointToNoAnim(m_Current.GetData().NextPointer);

        //        // Animate node upwards
        //        bool moved = false;
        //        nodeToDelete.gameObject.LeanMove(nodeToDelete.transform.position + m_DelNodeMoveOffset, m_NodeMoveAnimDur)
        //            .setOnUpdate((float time) => {
        //                currPointer.LookAtNoAnim(nodeToDelete);
        //                m_TempPtr.PointToNoAnim(nodeToDelete);

        //                if (nodeToRep != null)
        //                    nodeToDelete.NextPointer.LookAtNoAnim(nodeToRep);
        //            })
        //            .setOnComplete(() => moved = true);

        //        yield return new WaitUntil(() => moved);
        //    }

        //    {
        //        // Set current node's next to the nodeToRep
        //        yield return currPointer.LookAt(nodeToRep);

        //        // Delete Node to delete
        //        Destroy(nodeToDelete.gameObject);
        //        m_TempPtr.gameObject.SetActive(false);
        //        m_Size--;


        //        if (nodeToRep != null)
        //        {
        //            Vector3 _pos = currPointer.GetPointedPosition();
        //            nodeToRep.LeanMove(_pos, m_NodeMoveAnimDur);
        //            yield return new WaitForSeconds(m_NodeMoveAnimDur);
        //        }

        //        yield return new WaitForSeconds(0.2f);
        //    }
        //}

        //public IEnumerator DeleteWithNarration(
        //    int pos, AudioSource voiceSource,
        //    AudioClip nowSuppose,
        //    AudioClip nextWeCreate,
        //    AudioClip afterThatWeSet,
        //    AudioClip thenWeCanSafely)
        //{
        //    m_TempPtr.gameObject.SetActive(false);

        //    if (m_Head.GetData() == null)
        //    {
        //        Debug.Log("List is empty.");
        //        yield break;
        //    }

        //    // Bounds Check
        //    if (pos < 0 || pos >= m_Size)
        //    {
        //        Debug.Log($"Aborting Deletion at pos: {pos}. Pos it out of bounds. Size: {m_Size}");
        //        yield break;
        //    }

        //    voiceSource.clip = nowSuppose;
        //    voiceSource.Play();


        //    int currentPos = 0;
        //    m_Current.PointToNoAnim(m_Head);

        //    while (currentPos + 1 < pos)
        //    {
        //        yield return m_Current.PointTo(m_Current.GetData().NextPointer);
        //        currentPos++;
        //    }

        //    yield return new WaitUntil(() => !voiceSource.isPlaying);

        //    SpatialNode currentNode = m_Current.GetData();
        //    SpatialNode nodeToDelete = currentNode.NextPointer.GetData();

        //    Debug.Assert(currentNode != null);
        //    Debug.Assert(nodeToDelete != null);

        //    SpatialNode nodeToRep = nodeToDelete.NextPointer.GetData(); // Can be null if deleting the last node in the list

        //    {
        //        voiceSource.clip = nextWeCreate;
        //        voiceSource.Play();

        //        m_TempPtr.gameObject.SetActive(true);
        //        m_TempPtr.PointToNoAnim(m_Current.GetData().NextPointer);



        //        // Animate node upwards
        //        bool moved = false;
        //        nodeToDelete.gameObject.LeanMove(nodeToDelete.transform.position + m_DelNodeMoveOffset, m_NodeMoveAnimDur)
        //            .setOnUpdate((float time) => {
        //                currentNode.NextPointer.LookAtNoAnim(nodeToDelete);
        //                m_TempPtr.PointToNoAnim(nodeToDelete);

        //                if (nodeToRep != null)
        //                    nodeToDelete.NextPointer.LookAtNoAnim(nodeToRep);
        //            })
        //            .setOnComplete(() => moved = true);

        //        yield return new WaitUntil(() => moved);
        //        yield return new WaitUntil(() => !voiceSource.isPlaying);

        //    }

        //    {
        //        voiceSource.clip = afterThatWeSet;
        //        voiceSource.Play();

        //        // Set current node's next to the nodeToRep
        //        yield return currentNode.NextPointer.LookAt(nodeToRep);

        //        yield return new WaitUntil(() => !voiceSource.isPlaying);

        //        voiceSource.clip = thenWeCanSafely;
        //        voiceSource.Play();

        //        // Delete Node to delete
        //        Destroy(nodeToDelete.gameObject);
        //        m_TempPtr.gameObject.SetActive(false);
        //        m_Size--;


        //        if (nodeToRep != null)
        //        {
        //            Vector3 _pos = currentNode.NextPointer.GetPointedPosition();
        //            nodeToRep.LeanMove(_pos, m_NodeMoveAnimDur);
        //            yield return new WaitForSeconds(m_NodeMoveAnimDur);
        //        }

        //        yield return new WaitUntil(() => !voiceSource.isPlaying);
        //    }
        //}

        //public IEnumerator Traverse(int pos = -1, bool resetCurrent = true)
        //{
        //    if (pos < 0 || pos > m_Size - 1)
        //    {
        //        pos = m_Size;
        //    }

        //    if (m_Head.GetData() == null)
        //    {
        //        Debug.Log("List is empty.");
        //        yield break;
        //    }

        //    m_Current.PointToNoAnim(m_Head);

        //    //Node temp = head;

        //    string output = "List: ";
        //    int currPos = -1;
        //    while (m_Current.GetData() != null && currPos + 1 < pos)
        //    {
        //        output += m_Current.GetData().Data + " -> ";
        //        //temp = temp.next;
        //        yield return m_Current.PointTo(m_Current.GetData().NextPointer);
        //        currPos++;
        //    }

        //    output += "NULL";

        //    if (resetCurrent)
        //    {
        //        m_Current.PointToNoAnim(m_Head);
        //    }
        //    Debug.Log(output);
        //}

        //private void OnDestroy()
        //{
        //    CleanupAllNodes();
        //}

        //void CleanupAllNodes()
        //{
        //    if (m_Head == null) return;

        //    SpatialNode current = m_Head.GetData();

        //    while (current != null)
        //    {
        //        SpatialNode next = current.NextPointer != null
        //            ? current.NextPointer.GetData()
        //            : null;

        //        Destroy(current.gameObject);
        //        current = next;
        //    }


        //    m_Size = 0;
        //}
    }


}
