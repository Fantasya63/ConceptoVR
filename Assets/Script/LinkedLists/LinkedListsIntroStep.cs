using Canvas;
using Concepto;
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
    [SerializeField] Transform m_PointerShowPos;

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

    Coroutine m_Coroutine;
    [SerializeField] GameObject m_TempObjectsHolder = null;

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
        Debug.Assert(m_PointerShowPos != null);
    }


    public override void Activate()
    {
        if (m_Coroutine != null)
            StopCoroutine(m_Coroutine);

        m_Coroutine = StartCoroutine(OnRoutine());
    }

    private IEnumerator OnRoutine()
    {
        PlayVoiceNoWait(m_VoiceSource, m_LinkedListsIsA);

        SpatialNode node = GameObject.Instantiate(m_SpatialNodePrefab);
        node.transform.position = m_NodeDefStartTransform.position;
        Vector3 startScale = node.transform.localScale;
        node.transform.localScale = Vector3.zero;
        node.transform.LeanScale(startScale, m_NodeGrowDur);

        yield return new WaitForSeconds(m_NodeGrowDur);
        yield return new WaitUntil(() => !m_VoiceSource.isPlaying);

        PlayVoiceNoWait(m_VoiceSource, m_AndEachNodePoints);


        SpatialNode[] nodes = new SpatialNode[m_NumOfInstances];
        nodes[0] = node;

        {
            Vector3 pos = node.transform.position;
            for (int i = 1; i < m_NumOfInstances; ++i)
            {
                pos += m_NodeOffset;
                SpatialNode _node = GameObject.Instantiate(m_SpatialNodePrefab);
                _node.transform.position = pos;
                _node.Data = i.ToString();
                
                nodes[i] = _node;
                nodes[i -  1].NextPointer.LookAtNoAnim(_node);


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
                float offset = Random.value;
                SpatialNode _node = nodes[i];
                initPositions[i] = _node.transform.position;
                Vector3 pos = _node.transform.position + Vector3.right * offset;
                _node.transform.LeanMove(pos, m_NodeShiftDur)
                    .setOnUpdate((float t) => {
                        int nextIndex = index + 1;
                        if (nextIndex< m_NumOfInstances)
                        {
                            SpatialNode nextNode = nodes[nextIndex];
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
                SpatialNode _node = nodes[i];
                _node.transform.LeanMove(initPositions[i], m_NodeShiftDur)
                    .setOnUpdate((float t) => {
                        int nextIndex = index + 1;
                        if (nextIndex < m_NumOfInstances)
                        {
                            SpatialNode nextNode = nodes[nextIndex];
                            _node.NextPointer.LookAtNoAnim(nextNode);
                        }
                    });

            }

            yield return new WaitUntil(() => !m_VoiceSource.isPlaying);

            PlayVoiceNoWait(m_VoiceSource, m_ToConnectTheseNodes);

            {
                SpatialPointer examplePointer = Instantiate(nodes[0].NextPointer);
                
                
                Vector3 startPos = examplePointer.transform.position;
                //examplePointer.transform.LeanMove()

            }
        }
    }

    public override void Deactivate()
    {
    }

    public override void OnSlideExit()
    {
    }
}
