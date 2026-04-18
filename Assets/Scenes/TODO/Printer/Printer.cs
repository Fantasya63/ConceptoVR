using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Printer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Paper paperPrefab;
    [SerializeField] private Camera renderCamera;
    [SerializeField] private Transform paperStartPos;
    [SerializeField] private Transform paperFinalPos;
    [SerializeField] private TMP_Text printerStamp;
    [SerializeField] private AudioSource printerAudio;

    [Header("Texture Settings")]
    [SerializeField] private int textureWidth = 256;
    [SerializeField] private int textureHeight = 192;

    [Header("Print Settings")]
    [SerializeField] private float printMoveDuration = 2f;

    [Header("Events")]
    public UnityEvent<Paper> OnPaperPrinted;


    private bool IsPrinting = false;
    private void Awake()
    {
        renderCamera.enabled = false;
    }

    public IEnumerator PrintNoAnimEnumarator(string text, System.Action<Paper> onFinished, Paper.PAPER_TYPE type)
    {
        yield return PrintRoutineNoAnim(text, type, onFinished);
    }

    public bool PrintNoAnim(string text, System.Action<Paper> onFinished, Paper.PAPER_TYPE type)
    {
        if (!IsPrinting)
        {
            StartCoroutine(PrintRoutineNoAnim(text, type, onFinished));
            IsPrinting = true;
            return true;
        }
        return false;
    }
    IEnumerator PrintRoutineNoAnim(string text, Paper.PAPER_TYPE type, System.Action<Paper> onFinished)
    {
        printerStamp.text = text;
        printerStamp.ForceMeshUpdate();

        yield return new WaitForEndOfFrame();
        renderCamera.Render();

        Texture2D snapshot = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
        RenderTexture.active = renderCamera.targetTexture;
        snapshot.ReadPixels(new Rect(0, 0, textureWidth, textureHeight), 0, 0);
        snapshot.Apply();
        RenderTexture.active = null;

        Paper paper = Instantiate(paperPrefab, paperStartPos.position, paperStartPos.rotation);
        paper.data = text;
        paper.PaperType = type;

        MeshRenderer renderer = paper.GetComponent<MeshRenderer>();
        Material newMat = new Material(renderer.material);
        renderer.material = newMat;
        renderer.material.SetTexture("_BaseMap", snapshot);

        IsPrinting = false;

        onFinished?.Invoke(paper);
    }


    public void Print(string text, bool paperHasInteractivity = true)
    {
        if (!IsPrinting)
        {
            StartCoroutine(PrintRoutine(text, Paper.PAPER_TYPE.Data, paperHasInteractivity));
            IsPrinting = true;
        }
    }

    public void PrintHashkey(string key, bool paperHasInteractivity = true)
    {
        if (!IsPrinting)
        {
            StartCoroutine(PrintRoutine(key, Paper.PAPER_TYPE.Hashkey, paperHasInteractivity));
            IsPrinting = true;
        }
    }

    public IEnumerator PrintHashkeyRoutine(string key, bool paperHasInteractivity = true)
    {
        yield return PrintRoutine(key, Paper.PAPER_TYPE.Hashkey, paperHasInteractivity);
    }


    private IEnumerator PrintRoutine(string text, Paper.PAPER_TYPE type, bool paperHasInteractivity)
    {
        // Set TMP text
        printerStamp.text = text;
        printerStamp.ForceMeshUpdate();

        // Render to texture
        // renderCamera.targetTexture = renderTexture;
        
        yield return new WaitForEndOfFrame();
        renderCamera.Render();
        
        // Copy to Texture2D
        Texture2D snapshot = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);

        RenderTexture.active = renderCamera.targetTexture;
        snapshot.ReadPixels(new Rect(0, 0, textureWidth, textureHeight), 0, 0);
        snapshot.Apply();
        RenderTexture.active = null;


        // Spawn paper at start
        Paper paper = Instantiate(paperPrefab, paperStartPos.position, paperStartPos.rotation);
        if (!paperHasInteractivity)
            paper.RemoveInteractivity();

        paper.data = text;
        paper.PaperType = type;

        MeshRenderer renderer = paper.GetComponent<MeshRenderer>();
        Material newMat = new Material(renderer.material);
        renderer.material = newMat;
        renderer.material.SetTexture("_BaseMap", snapshot);

        // Disable grabbing while printing
        XRGrabInteractable grab = paper.GetComponent<XRGrabInteractable>();
        Rigidbody rb = paper.GetComponent<Rigidbody>();

        // Disable grabbing while printing
        if (grab != null)
            grab.enabled = false;

        // Make physics safe during movement
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // Play audio
        if (printerAudio != null)
            printerAudio.Play();

        // Move paper
        float elapsed = 0f;

        Vector3 startPos = paperStartPos.position;
        Vector3 endPos = paperFinalPos.position;

        while (elapsed < printMoveDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / printMoveDuration;

            paper.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }
        paper.transform.position = endPos;

        // Stop audio
        if (printerAudio != null)
            printerAudio.Stop();

        // Restore physics
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        // Enable grabbing after printing
        if (grab != null)
            grab.enabled = true;

        Debug.Log("Printer: Paper Printed");
        OnPaperPrinted.Invoke(paper);
        IsPrinting = false;
    }
}
