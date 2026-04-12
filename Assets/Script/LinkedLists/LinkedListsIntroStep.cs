using Canvas;
using Concepto;
using NUnit.Framework.Constraints;
using System.Collections;
using UnityEngine;

public class LinkedListsIntroStep : Step
{
    [Header("Anim References")]
    [SerializeField] Transform m_NodeDefStartTransform;
    [SerializeField] float m_NodeGrowDur = 1.0f;
    [SerializeField] Vector3 m_NodeOffset = Vector3.back;
    [SerializeField] int m_NumOfInstances = 3;
    [SerializeField] float m_NodeShiftDur = 1.0f;
    [SerializeField] float[] m_ShiftOffsets;
    [SerializeField] float m_ExamplePointerUpSpeed = 0.5f;
    [SerializeField] float m_PointerDisplayYOffset = 0.25f;
    

    [Header("References")]
    [SerializeField] AudioSource m_VoiceSource;

    [Header("Voice Clips")]
    [SerializeField] AudioClip m_PleaseHeadOver;
    [SerializeField] AudioClip m_LinkedListsIsA;
    [SerializeField] AudioClip m_AndEachNodePoints;
    [SerializeField] AudioClip m_UnlikeArrays;
    [SerializeField] AudioClip m_HoweverForTheSake;
    [SerializeField] AudioClip m_ToConnectTheseNodes;

    [Header("Prefabs")]
    [SerializeField] SpatialNode m_SpatialNodePrefab;

    GameObject m_TempObjectsHolder;

    SpatialNode[] m_TempNodes;
    SpatialPointer m_TempExamplePointer;
    Coroutine m_Coroutine;

    void DestroyNodes()
    {
        if (m_TempNodes != null)
        {
            for (int i = 0; i < m_TempNodes.Length; i++)
            {
                Destroy(m_TempNodes[i].gameObject);
            }
        }
        m_TempNodes = null;
    }

    private void Start()
    {
        Debug.Assert(m_VoiceSource != null);
        Debug.Assert(m_PleaseHeadOver != null);
        Debug.Assert(m_LinkedListsIsA != null);
        Debug.Assert(m_AndEachNodePoints != null);
        Debug.Assert(m_UnlikeArrays != null);
        Debug.Assert(m_HoweverForTheSake != null);
        Debug.Assert(m_ToConnectTheseNodes != null);

        Debug.Assert(m_SpatialNodePrefab != null);
        Debug.Assert(m_NodeDefStartTransform != null);
        Debug.Assert(m_NumOfInstances > 1);

        Debug.Assert(m_NumOfInstances == m_ShiftOffsets.Length);
    }


    public override void Activate()
    {
        CleanUP();

        m_TempObjectsHolder = new GameObject();
        m_Coroutine = StartCoroutine(OnRoutine());

    }


    private IEnumerator OnRoutine()
    {
        PlayVoiceNoWait(m_VoiceSource, m_LinkedListsIsA);

        SpatialNode m_NodeInstance = GameObject.Instantiate(m_SpatialNodePrefab, m_TempObjectsHolder.transform);
        m_NodeInstance.transform.position = m_NodeDefStartTransform.position;
        Vector3 startScale = m_NodeInstance.transform.localScale;
        m_NodeInstance.transform.localScale = Vector3.zero;
        m_NodeInstance.transform.LeanScale(startScale, m_NodeGrowDur);

        yield return new WaitForSeconds(m_NodeGrowDur);
        yield return new WaitUntil(() => !m_VoiceSource.isPlaying);

        PlayVoiceNoWait(m_VoiceSource, m_AndEachNodePoints);

        if (this.m_TempNodes != null && this.m_TempNodes.Length > 0)
        {
            DestroyNodes();
        }
        this.m_TempNodes = new SpatialNode[m_NumOfInstances];
        m_TempNodes[0] = m_NodeInstance;

        {
            Vector3 pos = m_NodeInstance.transform.position;
            for (int i = 1; i < m_NumOfInstances; ++i)
            {
                pos += m_NodeOffset;
                SpatialNode _node = GameObject.Instantiate(m_SpatialNodePrefab, m_TempObjectsHolder.transform);
                _node.transform.position = pos;
                _node.Data = i.ToString();
                
                m_TempNodes[i] = _node;
                m_TempNodes[i -  1].NextPointer.LookAtNoAnim(_node);


                yield return GrowAndWait(_node.gameObject, m_NodeGrowDur);
            }

            yield return new WaitUntil(() => !m_VoiceSource.isPlaying);
        }

        yield return new WaitForSeconds(3.0f);

        {
            PlayVoiceNoWait(m_VoiceSource, m_UnlikeArrays);
            Vector3[] initPositions = new Vector3[m_NumOfInstances];

            for (int i = 0; i < m_NumOfInstances; ++i)
            {
                // Save local ref for the tweening
                int index = i;
                float offset = m_ShiftOffsets[i];
                SpatialNode _node = m_TempNodes[i];
                initPositions[i] = _node.transform.position;
                Vector3 pos = _node.transform.position + Vector3.right * offset;
                _node.transform.LeanMove(pos, m_NodeShiftDur)
                    .setOnUpdate((float t) => {
                        int nextIndex = index + 1;
                        if (nextIndex< m_NumOfInstances)
                        {
                            SpatialNode nextNode = m_TempNodes[nextIndex];
                            _node.NextPointer.LookAtNoAnim(nextNode);
                        }
                    });

            }

            yield return new WaitForSeconds(m_NodeShiftDur);
            yield return new WaitUntil(() => !m_VoiceSource.isPlaying);

            PlayVoiceNoWait(m_VoiceSource, m_HoweverForTheSake);

            for (int i = 0; i < m_NumOfInstances; ++i) 
            {
                int index = i;
                SpatialNode _node = m_TempNodes[i];
                _node.transform.LeanMove(initPositions[i], m_NodeShiftDur)
                    .setOnUpdate((float t) => {
                        int nextIndex = index + 1;
                        if (nextIndex < m_NumOfInstances)
                        {
                            SpatialNode nextNode = m_TempNodes[nextIndex];
                            _node.NextPointer.LookAtNoAnim(nextNode);
                        }
                    });

            }

            yield return new WaitUntil(() => !m_VoiceSource.isPlaying);

            PlayVoiceNoWait(m_VoiceSource, m_ToConnectTheseNodes);

            {
                m_TempExamplePointer = Instantiate(m_TempNodes[0].NextPointer);
                string label = "Pointer";
                m_TempExamplePointer.SetLabel(label);

                Vector3 startPos = m_TempNodes[0].NextPointer.transform.position;
                Vector3 endPos = startPos + Vector3.up * m_PointerDisplayYOffset;

                m_TempExamplePointer.transform.position = startPos;
                m_TempExamplePointer.transform.LeanMove(endPos, m_ExamplePointerUpSpeed);
                //examplePointer.transform.LeanMove()

            }

            yield return new WaitUntil(() => !m_VoiceSource.isPlaying);

            Complete();
        }
    }

    void CleanUP()
    {
        if (m_Coroutine != null)
            StopCoroutine(m_Coroutine);

        DestroyNodes();

        if (m_TempExamplePointer != null)
        {
            Destroy(m_TempExamplePointer.gameObject);
            m_TempExamplePointer = null;
        }

        if (m_TempObjectsHolder)
        {
            Destroy(m_TempObjectsHolder.gameObject);
            m_TempObjectsHolder = null;
        }
    }

    public override void Deactivate()
    {
        CleanUP();
    }

    public override void OnSlideExit()
    {
       CleanUP();

    }
}
