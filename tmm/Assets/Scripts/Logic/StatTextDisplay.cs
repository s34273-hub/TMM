using UnityEngine;
using UnityEngine.UI;
using TMPro;

[AddComponentMenu("Game/UI/Stat Text Display")]
public class StatTextDisplay : MonoBehaviour
{
    [Header("Source")]
    [Tooltip("The StatVariable to display.")]
    public StatVariable statVariable;

    [Header("Display Settings")]
    [Tooltip("Optional prefix (e.g., 'Coins: ').")]
    public string prefix = "";

    [Tooltip("Optional suffix (e.g., ' HP', ' pts').")]
    public string suffix = "";

    [Tooltip("Update text every frame instead of only when the variable changes.")]
    public bool updateContinuously = false;

    // Cached references to whichever text type this GameObject has
    private Text uiText;
    private TMP_Text tmpText;

    private void Awake()
    {
        uiText = GetComponent<Text>();
        tmpText = GetComponent<TMP_Text>();

        if (uiText == null && tmpText == null)
            Debug.LogWarning("[StatTextDisplay] No Text or TMP_Text component found on " + gameObject.name);
    }

    private void OnEnable()
    {
        if (statVariable != null)
        {
            statVariable.onValueChanged.AddListener(OnValueChanged);
            OnValueChanged(statVariable.GetValue()); // initialize immediately
        }
    }

    private void OnDisable()
    {
        if (statVariable != null)
            statVariable.onValueChanged.RemoveListener(OnValueChanged);
    }

    private void Update()
    {
        if (updateContinuously && statVariable != null)
            OnValueChanged(statVariable.GetValue());
    }

    private void OnValueChanged(int value)
    {
        string textValue = $"{prefix}{value}{suffix}";

        if (uiText != null)
            uiText.text = textValue;
        else if (tmpText != null)
            tmpText.text = textValue;
    }
}
