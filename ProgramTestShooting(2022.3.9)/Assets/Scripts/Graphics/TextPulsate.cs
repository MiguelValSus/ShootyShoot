using UnityEngine;
using UnityEngine.UI;

public class TextPulsate : MonoBehaviour
{
    public Text uiText;           
    public float pulseSpeed = 1f; 
    public float minAlpha = 0.2f; 
    public float maxAlpha = 1f;   

    private Color _originalColor;

    void Start()
    {
        _originalColor = uiText.color;
    }

    void Update()
    {
        if (uiText == null) return;
        PulsateTransparency();
    }

    void PulsateTransparency()
    {
        var textAlpha = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);
        uiText.color = new Color(_originalColor.r, _originalColor.g, _originalColor.b, textAlpha);
    }
}
