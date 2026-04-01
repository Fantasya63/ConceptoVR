using UnityEngine;

namespace Canvas
{
    public class WaitForInputStep : Step
    {
        // Call this function to trigger an input and move to the next step
        public void TriggerInput()
        {
            Complete();
        }

        public override void Activate()
        {
        }

        public override void Deactivate()
        {
        }

        public override void OnSlideExit()
        {
           
        }
    }

}