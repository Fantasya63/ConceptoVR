using UnityEngine;

public class HashFuncVisualizer : MonoBehaviour
{
    [SerializeField] private GameObject visualizer;
    [SerializeField] private Transform printerPivot;

    [Header("Rotation Settings")]
    [SerializeField] private float printerCloseRot = 0f;
    [SerializeField] private float printerOpenRot = 90f;
    [SerializeField] private float rotationTime = 0.3f;

    [Header("Scale Tween Settings")]
    [SerializeField] private float growDuration = 0.4f;
    [SerializeField] private LeanTweenType easeType = LeanTweenType.easeOutBack;

    private bool open = false;

    private Quaternion baseLocalRotation;

    private void Awake()
    {
        if (printerPivot != null)
            baseLocalRotation = printerPivot.localRotation;
    }

    private void Start()
    {
        if (visualizer == null || printerPivot == null)
            return;

        visualizer.SetActive(true);

        printerPivot.localRotation =
            baseLocalRotation * Quaternion.AngleAxis(printerCloseRot, Vector3.right);

        visualizer.transform.localScale = Vector3.zero;
    }

    public void Toggle()
    {
        open = !open;

        if (open)
            Visualize();
        else
            HideVisualization();
    }

    private void Visualize()
    {
        if (visualizer == null || printerPivot == null)
            return;

        LeanTween.cancel(visualizer);
        visualizer.transform.localScale = Vector3.zero;

        LeanTween.scale(visualizer, Vector3.one, growDuration)
                 .setEase(easeType);

        RotateTo(printerOpenRot);
    }

    private void HideVisualization()
    {
        if (visualizer == null || printerPivot == null)
            return;

        LeanTween.cancel(visualizer);
        LeanTween.scale(visualizer, Vector3.zero, growDuration)
                 .setEase(easeType);

        RotateTo(printerCloseRot);
    }

    private void RotateTo(float targetAngle)
    {
        LeanTween.cancel(printerPivot.gameObject);

        Quaternion start = printerPivot.localRotation;
        Quaternion end =
            baseLocalRotation * Quaternion.AngleAxis(targetAngle, Vector3.right);

        LeanTween.value(printerPivot.gameObject, 0f, 1f, rotationTime)
            .setEase(easeType)
            .setOnUpdate((float t) =>
            {
                printerPivot.localRotation =
                    Quaternion.Slerp(start, end, t);
            });
    }
}