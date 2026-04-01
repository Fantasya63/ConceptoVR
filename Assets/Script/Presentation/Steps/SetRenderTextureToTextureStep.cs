using UnityEngine;

namespace Canvas
{
    public class SetRenderTextureToTextureStep : Step
    {
        [SerializeField]
        private RenderTexture outputRenderTexture;

        [SerializeField]
        private Texture2D inputTexture;

        public override void Activate()
        {
            if (outputRenderTexture == null || inputTexture == null)
            {
                Debug.LogWarning("[SetRenderTextureToTextureStep] Missing texture reference.");
                return;
            }

            Graphics.CopyTexture(inputTexture, outputRenderTexture);
            Complete();
        }


        public override void Deactivate()
        {
           
        }

        public override void OnSlideExit()
        {
           
        }
    }
}