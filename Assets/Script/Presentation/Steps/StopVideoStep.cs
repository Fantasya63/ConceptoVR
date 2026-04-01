using UnityEngine;
using UnityEngine.Video;

namespace Canvas
{
    public class StopVideoStep : Step
    {
        [SerializeField]
        VideoPlayer player;

        public override void Activate()
        {
            Debug.Assert(player != null);
            player.Stop();
            Complete();
        }

        public override void Deactivate()
        {
            return;
        }

        public override void OnSlideExit()
        {
            
        }
    }
}