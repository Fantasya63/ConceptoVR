using UnityEngine;

public class ExitGameHandler : MonoBehaviour
{
    public void QuitGame()
    {
        // Check if we are running in the Unity Editor
#if UNITY_EDITOR
        // This command exits "Play Mode" in the Editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // This command closes the application in a standalone build
            Application.Quit();
#endif
    }
}
