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
        protected IEnumerator WaitForArray<T>(
            T prefab,
            int count,
            float spawnDelay,
            Vector3 spawnPos,
            Vector3 offset,
            System.Action <T, int> onSpawned,
            System.Action<T[]> result
        ) where T : MonoBehaviour
        {
            T[] instances = new T[count];

            for (int i = 0; i < count; i++)
            {
                // Instantiate using the prefab instance
                T instance = Instantiate(prefab);

                instance.transform.position = spawnPos + offset * i;

                instances[i] = instance;

                onSpawned?.Invoke(instance, i);

                // Wait before spawning next
                if (spawnDelay > 0f)
                    yield return new WaitForSeconds(spawnDelay);
            }


            // Return the result
            result?.Invoke(instances);
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

        protected IEnumerator GrowAndWait(GameObject go, float duration)
        {
            Vector3 startScale = go.transform.localScale;
            go.transform.localScale = Vector3.zero;
            go.LeanScale(startScale, duration);

            yield return new WaitForSeconds(duration);
        }
    }
}