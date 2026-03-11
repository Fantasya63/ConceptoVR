using UnityEngine;
using UnityEngine.Video;

namespace Canvas
{
    public class PlayVideoStep : Step
    {
        [SerializeField]
        VideoPlayer player;
        

        public override void Activate()
        {
            Debug.Log($"Playing video player: {player.name}");
            player.Play();
            Complete();
        }

        public override void Deactivate()
        {
            return;
        }

        public override void OnSlideExit()
        {
            if (player != null)
            {
                Debug.Log($"Stopping video player: {player.name}");
                player.Stop();
            }
        }
    }

}