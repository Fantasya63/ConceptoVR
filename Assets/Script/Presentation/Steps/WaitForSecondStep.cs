using System.Collections;
using UnityEngine;

namespace Canvas
{
    public class WaitForSecondStep : Step
    {
        [SerializeField]
        private float seconds = 1.0f;

        private Coroutine waitRoutine;
        public override void Activate()
        {
            Debug.Log($"Start to wait for {seconds} seconds");
            Debug.Log($"Timescale is: {Time.timeScale}");
            waitRoutine = StartCoroutine(WaitRoutine());
        }
        public override void Deactivate()
        {
            if (waitRoutine != null)
            {
                StopCoroutine(waitRoutine);
                waitRoutine = null;
            }
        }

        public override void OnSlideExit()
        {
            
        }

        private IEnumerator WaitRoutine()
        {
            yield return new WaitForSeconds(seconds);
            Debug.Log($"Waited for {seconds} seconds!");
            Complete();
        }
    }
}