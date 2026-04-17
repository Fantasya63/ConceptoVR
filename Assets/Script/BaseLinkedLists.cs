
using System.Collections;
using UnityEngine;

namespace Concepto
{
    public abstract class BaseLinkedLists<TData, TPointer, TNode> : MonoBehaviour
        where TPointer : BasePointer<TNode>
        where TNode : BaseNode<TData, TPointer, TNode>
    {
        [Header("References")]
        [SerializeField] protected TPointer m_Head;
        [SerializeField] protected TPointer m_Current;
        [SerializeField] protected TPointer m_TempPtr;
        [SerializeField] protected AudioSource m_AudioSource;
        [SerializeField] protected AudioClip m_ErrorClip;

        [Header("Options")]
        [SerializeField] protected Vector3 m_NewNodeSpawnOffset = Vector3.up * 0.25f;
        [SerializeField] protected Vector3 m_DelNodeMoveOffset = Vector3.up * 0.25f;
        [SerializeField] protected float m_AnimDownwardDur = 1.0f;
        [SerializeField] protected float m_PointerLookLerpDur = 0.5f;
        [SerializeField] protected float m_NodeMoveAnimDur = 0.5f;

        [Header("Prefabs")]
        [SerializeField] protected TNode m_HashmapNodePrefab;

        protected int m_Size = 0;


        public int Size
        {
            get
            {
                return m_Size;
            }
        }

        public TPointer CurrentPointer
        {
            get
            {
                return m_Current;
            }
        }


        Coroutine m_CommandCoroutine;


        void Start()
        {
            m_Size = 0;
            m_TempPtr.gameObject.SetActive(false);
            Debug.Assert(m_Head != null);

            if (m_CommandCoroutine != null)
                StopCoroutine(m_CommandCoroutine);

        }




        public bool CanInsert(string value, int pos)
        {
            if (pos < 0 || pos > m_Size || m_CommandCoroutine != null)
                return false;

            return true;
        }

        public bool CanDelete(int pos)
        {
            if (pos < 0 || pos >= m_Size || m_CommandCoroutine != null)
            {
                return false;
            }
            return true;
        }

        public bool CanTraverse()
        {
            return m_CommandCoroutine == null;
        }


        private void OnDestroy()
        {
            CleanupAllNodes();
        }

        void CleanupAllNodes()
        {
            if (m_Head == null) return;

            TNode current = m_Head.GetData();

            while (current != null)
            {
                TNode next = current.NextPointer != null
                    ? current.NextPointer.GetData()
                    : null;

                Destroy(current.gameObject);
                current = next;
            }


            m_Size = 0;
        }
    }


}
