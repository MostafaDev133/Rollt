using UnityEngine;

public class WaveExpander : MonoBehaviour
{
    [Header("Expansion Settings")]
    [SerializeField] private float maxRadius = 7.0f;
    [SerializeField] private float expansionDuration = 0.35f;

    private float timer = 0f;
    private Material mat;
    private Color initialColor;

    private void Start()
    {
        // Start tiny
        transform.localScale = Vector3.one * 0.1f;

        // Cache material for fading alpha if it has a color property
        var renderer = GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            mat = renderer.material;
            initialColor = mat.color;
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        float progress = Mathf.Clamp01(timer / expansionDuration);

        // Exponential ease-out curve so it starts fast and slows down gracefully
        float easedProgress = 1f - Mathf.Pow(1f - progress, 3f);

        // Scale up to match max radius (assuming base mesh is 1 unit wide, scale = radius * 2)
        float currentScale = Mathf.Lerp(0.1f, maxRadius * 2f, easedProgress);
        transform.localScale = new Vector3(currentScale, currentScale, currentScale);

        // Fade out alpha over time
        if (mat != null)
        {
            Color fadedColor = initialColor;
            fadedColor.a = Mathf.Lerp(initialColor.a, 0f, progress);
            mat.color = fadedColor;
        }

        // Destroy when finished
        if (timer >= expansionDuration)
        {
            Destroy(gameObject);
        }
    }
}