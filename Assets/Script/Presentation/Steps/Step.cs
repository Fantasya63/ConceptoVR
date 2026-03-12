using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Canvas
{
    public abstract class Step : MonoBehaviour
    {
        // Event fired when a Step is completed
        public UnityEvent OnCompleted = new UnityEvent();

        [HideInInspector]
        public Slides slide;

        public abstract void Activate();

        public abstract void Deactivate();

        public abstract void OnSlideExit();


        // public abstract void Replay();
       
        /// <summary>
        /// Call this when your Step finishes what it needs to do.
        /// </summary>
        protected void Complete()
        {
            OnCompleted?.Invoke();
        }

        private void Start()
        {
            Deactivate();
        }

        protected IEnumerator WaitForAnimator(Animator anim)
        {
            yield return null;

            yield return new WaitUntil(() =>
            {
                AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
                return state.normalizedTime >= 1f && !anim.IsInTransition(0);
            });
        }
        protected void PlayVoiceNoWait(AudioSource source, AudioClip clip)
        {
            source.clip = clip;
            source.Play();
        }

        protected IEnumerator PlayAndWaitVoice(AudioSource source, AudioClip clip)
        {
            PlayVoiceNoWait(source, clip);
            
            yield return new WaitWhile(() => source.isPlaying);
        }
    }
}