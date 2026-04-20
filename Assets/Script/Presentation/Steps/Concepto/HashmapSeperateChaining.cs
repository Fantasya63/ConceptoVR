using Concepto;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Canvas {

    public class HashmapSeperateChaining : Step
    {
        
        [Header("References")]
        [SerializeField] AudioSource m_VoiceSource;
        [SerializeField] SpatialHashmap m_HashmapPrefab;
        [SerializeField] Printer m_ScriptedPrinter;
        [SerializeField] Transform m_KeyStartTransform;
        [SerializeField] Transform m_SpatialHashmapPosTransform;


        [Header("Voice Overs")]
        [SerializeField] AudioClip m_OneMethod;
        [SerializeField] AudioClip m_InsteadOfJust;
        [SerializeField] AudioClip m_FirstWeHashTheKeys;
        [SerializeField] AudioClip m_ThenWeTraverse;
        [SerializeField] AudioClip m_HoweverIfThereIsANode;
        [SerializeField] AudioClip m_ThisConcludesOurTopic;

        [Header("Example Values")]
        [SerializeField] String m_FirstKeyExample = "ABC";
        [SerializeField] String m_FirstValExample = "63";

        [SerializeField] String m_SecondKeyExample = "ABC";
        [SerializeField] String m_SecondValExample = "123";

        [SerializeField] String m_ThirdKeyExample = "CDE";
        [SerializeField] String m_ThirdValExample = "456";

        [SerializeField] String m_FourthKeyExample = "CDG";
        [SerializeField] String m_FourthValExample = "789";

        Coroutine m_Coroutine = null;
        SpatialHashmap m_HashmapInstance = null;

        public void Awake()
        {
            Debug.Assert(m_VoiceSource  != null);
            Debug.Assert(m_OneMethod != null);
            Debug.Assert(m_InsteadOfJust != null);
            Debug.Assert(m_FirstWeHashTheKeys != null);
            Debug.Assert(m_ThenWeTraverse != null);
            Debug.Assert(m_HoweverIfThereIsANode != null);
            Debug.Assert(m_ThisConcludesOurTopic != null);
            Debug.Assert(m_KeyStartTransform != null);

            Debug.Assert(m_HashmapPrefab != null);
        }

        List<GameObject> m_TempObjects = new List<GameObject>();


        public override void Activate()
        {
            if (m_Coroutine != null)
                StopCoroutine(m_Coroutine);

            if (m_HashmapInstance != null)
                Destroy(m_HashmapInstance.gameObject);

            m_HashmapInstance = Instantiate(m_HashmapPrefab, transform);
            m_HashmapInstance.m_ScriptedPrinter = m_ScriptedPrinter;
            m_HashmapInstance.transform.position = m_SpatialHashmapPosTransform.position;
            m_TempObjects.Add(m_HashmapInstance.gameObject);

            m_Coroutine = StartCoroutine(StepRoutine());
        }


        IEnumerator StepRoutine()
        {
            PlayVoiceNoWait(m_VoiceSource, m_OneMethod);

            //yield return m_HashmapInstance.Init();

            yield return WaitForAudioToFinish(m_VoiceSource);

            yield return new WaitForSeconds(0.5f);

            yield return PlayAndWaitVoice(m_VoiceSource, m_InsteadOfJust);

            Paper key = null;
            yield return m_ScriptedPrinter.PrintNoAnimEnumarator(m_FirstKeyExample, (Paper p) =>
            {
                key = p;
            }, Paper.PAPER_TYPE.Data);
            yield return new WaitUntil(() => key != null);
            key.RemoveInteractivity();

            Paper val = null;
            yield return m_ScriptedPrinter.PrintNoAnimEnumarator(m_FirstValExample, (Paper p) =>
            {
                val = p;
            }, Paper.PAPER_TYPE.Data);
            yield return new WaitUntil(() => val != null);
            val.RemoveInteractivity();


            Debug.Assert(val != null);
            Debug.Assert(key != null);

            m_TempObjects.Add(key.gameObject);
            m_TempObjects.Add(val.gameObject);


            PlayVoiceNoWait(m_VoiceSource, m_FirstWeHashTheKeys);

            Paper index = null;
            yield return m_HashmapInstance.HashWithNaration(key, (Paper i) =>
            {
                index = i;
            });
            index.RemoveInteractivity();

            m_TempObjects.Add(index.gameObject);

            yield return new WaitUntil(() => index != null);
            yield return m_HashmapInstance.PaperToIndexPos(index);

            yield return WaitForAudioToFinish(m_VoiceSource);

            PlayVoiceNoWait(m_VoiceSource, m_ThenWeTraverse);

            yield return m_HashmapInstance.TraverseAndReplaceOrCreateNaration(key, val, index);

            yield return WaitForAudioToFinish(m_VoiceSource);

            // OTher key value pair
            {
                Paper secondKey = null;
                yield return m_ScriptedPrinter.PrintNoAnimEnumarator(m_SecondKeyExample, (Paper p) =>
                {
                    secondKey = p;
                }, Paper.PAPER_TYPE.Data);
                yield return new WaitUntil(() => secondKey != null);


                Paper secondVal = null;
                yield return m_ScriptedPrinter.PrintNoAnimEnumarator(m_SecondValExample, (Paper p) =>
                {
                    secondVal = p;
                }, Paper.PAPER_TYPE.Data);
                yield return new WaitUntil(() => secondVal != null);


                m_TempObjects.Add(secondKey.gameObject);
                m_TempObjects.Add(secondVal.gameObject);


                yield return PlayAndWaitVoice(m_VoiceSource, m_HoweverIfThereIsANode);
                yield return m_HashmapInstance.Insert(secondKey, secondVal);

            }

            // Third key value pair
            {
                Paper _key = null;
                yield return m_ScriptedPrinter.PrintNoAnimEnumarator(m_ThirdKeyExample, (Paper p) =>
                {
                    _key = p;
                }, Paper.PAPER_TYPE.Data);
                yield return new WaitUntil(() => _key != null);


                Paper _val = null;
                yield return m_ScriptedPrinter.PrintNoAnimEnumarator(m_ThirdValExample, (Paper p) =>
                {
                    _val = p;
                }, Paper.PAPER_TYPE.Data);
                yield return new WaitUntil(() => _val != null);


                m_TempObjects.Add(_key.gameObject);
                m_TempObjects.Add(_val.gameObject);


                //yield return PlayAndWaitVoice(m_VoiceSource, m_HoweverIfThereIsANode);
                yield return m_HashmapInstance.Insert(_key, _val);

            }

            // Fourth key value pair
            {
                Paper _key = null;
                yield return m_ScriptedPrinter.PrintNoAnimEnumarator(m_FourthKeyExample, (Paper p) =>
                {
                    _key = p;
                }, Paper.PAPER_TYPE.Data);
                yield return new WaitUntil(() => _key != null);


                Paper _val = null;
                yield return m_ScriptedPrinter.PrintNoAnimEnumarator(m_FourthValExample, (Paper p) =>
                {
                    _val = p;
                }, Paper.PAPER_TYPE.Data);
                yield return new WaitUntil(() => _val != null);

                m_TempObjects.Add(_key.gameObject);
                m_TempObjects.Add(_val.gameObject);

                //yield return PlayAndWaitVoice(m_VoiceSource, m_HoweverIfThereIsANode);
                yield return m_HashmapInstance.Insert(_key, _val);

            }

            yield return PlayAndWaitVoice(m_VoiceSource, m_ThisConcludesOurTopic);

            Debug.Log("Seperate Chaining step finished");
            Complete();
            yield break;
        }


        public override void Deactivate()
        {
            if (m_TempObjects == null)
                return;

            for (int i = 0; i < m_TempObjects.Count; i++)
            {
                GameObject go = m_TempObjects[i];
                if (go != null)
                {
                    Destroy(go);
                }
            }

            m_TempObjects.Clear();
        }

        public override void OnSlideExit()
        {
            //throw new System.NotImplementedException();
        }
    }
}
