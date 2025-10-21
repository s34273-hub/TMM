using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

[AddComponentMenu("Game/UI/Conditional Stat Text Display (Single or Dual)")]
public class ConditionalStatEvent : MonoBehaviour
{
    public enum Comparison { Greater, GreaterOrEqual, Equal, NotEqual, LessOrEqual, Less }
    public enum ShowMode { CanvasGroupAlpha, SetActive }
    public enum TargetMode { Single, Dual }

    [Header("Source")]
    public StatVariable statVariable;
    public int compareTo = 0;
    public Comparison comparison = Comparison.GreaterOrEqual;

    [Header("Target Mode")]
    public TargetMode targetMode = TargetMode.Single;

    [Header("Single Target")]
    [Tooltip("Used when Target Mode = Single")]
    public Graphic singleTextObject;
    [Tooltip("Shown when condition = TRUE (Single mode)")]
    public string singleTrueText = "";
    [Tooltip("Shown when condition = FALSE (Single mode)")]
    public string singleFalseText = "";

    [Header("Dual Targets")]
    [Tooltip("Shown when TRUE (Dual mode)")]
    public Graphic trueTextObject;
    [Tooltip("Shown when FALSE (Dual mode)")]
    public Graphic falseTextObject;

    [Header("Dual Mode Optional Overrides")]
    public string trueTextOverride = "";
    public string falseTextOverride = "";

    [Header("Behavior")]
    public bool autoCheckOnChange = true;
    public bool checkOnEnable = true;
    public bool invokeEventsOnAutoChecks = false;
    public bool onlyInvokeOnStateChange = true;
    public ShowMode showMode = ShowMode.CanvasGroupAlpha;

    [Header("Events")]
    public UnityEvent onConditionTrue;
    public UnityEvent onConditionFalse;

    [Header("Debug")]
    public bool debugLogs = false;

    private bool lastResult;
    private bool hasLastResult = false;

    private void OnValidate()
    {
        if (targetMode == TargetMode.Dual &&
            trueTextObject && falseTextObject &&
            ReferenceEquals(trueTextObject.gameObject, falseTextObject.gameObject))
        {
            Debug.LogWarning("[ConditionalStatTextDisplay] TRUE and FALSE targets reference the SAME object. " +
                             "Use Single mode, or assign different objects (or leave one empty).", this);
        }
    }

    private void OnEnable()
    {
        if (!ValidateSetup()) return;

        if (autoCheckOnChange)
            statVariable.onValueChanged.AddListener(OnStatChanged);

        if (showMode == ShowMode.CanvasGroupAlpha && targetMode == TargetMode.Dual)
        {
            EnsureCanvasGroup(trueTextObject);
            EnsureCanvasGroup(falseTextObject);
        }
        else if (targetMode == TargetMode.Single)
        {
            EnsureCanvasGroup(singleTextObject);
        }

        if (checkOnEnable) Check(invokeEventsOnAutoChecks);
    }

    private void OnDisable()
    {
        if (statVariable != null && autoCheckOnChange)
            statVariable.onValueChanged.RemoveListener(OnStatChanged);
    }

    private void OnStatChanged(int _) => Check(invokeEventsOnAutoChecks);

    public void CheckAndInvoke() => Check(true);
    public void CheckVisualOnly() => Check(false);

    private void Check(bool allowInvoke)
    {
        if (statVariable == null) return;

        bool result = Evaluate(statVariable.GetValue(), compareTo, comparison);
        ApplyResult(result, allowInvoke);
    }

    private void ApplyResult(bool result, bool allowInvoke)
    {
        // Update visuals first so they appear even if listeners disable other stuff
        UpdateVisuals(result);

        bool shouldInvoke = allowInvoke && (!onlyInvokeOnStateChange || !hasLastResult || result != lastResult);
        if (shouldInvoke)
        {
            if (result) onConditionTrue?.Invoke();
            else onConditionFalse?.Invoke();
        }

        if (debugLogs) Debug.Log($"[ConditionalStatTextDisplay] result={(result ? "TRUE" : "FALSE")}", this);
        lastResult = result;
        hasLastResult = true;
    }

    private void UpdateVisuals(bool result)
    {
        if (targetMode == TargetMode.Single)
        {
            if (!singleTextObject) return;
            // Just swap text; never disable this object
            if (singleTextObject is Text t) t.text = result ? singleTrueText : singleFalseText;
            else if (singleTextObject is TMP_Text tm) tm.text = result ? singleTrueText : singleFalseText;

            if (showMode == ShowMode.CanvasGroupAlpha)
                SetCanvasGroupVisible(singleTextObject, true); // keep visible
            else
                singleTextObject.gameObject.SetActive(true);    // ensure on
            return;
        }

        // Dual mode
        // Guard against same-object assignment
        if (trueTextObject && falseTextObject &&
            ReferenceEquals(trueTextObject.gameObject, falseTextObject.gameObject))
        {
            // Fall back to text-only update to avoid self-disabling
            if (result && !string.IsNullOrEmpty(trueTextOverride)) SetText(trueTextObject, trueTextOverride);
            if (!result && !string.IsNullOrEmpty(falseTextOverride)) SetText(falseTextObject, falseTextOverride);
            return;
        }

        // Apply optional overrides
        if (result && trueTextObject && !string.IsNullOrEmpty(trueTextOverride))
            SetText(trueTextObject, trueTextOverride);
        if (!result && falseTextObject && !string.IsNullOrEmpty(falseTextOverride))
            SetText(falseTextObject, falseTextOverride);

        if (showMode == ShowMode.CanvasGroupAlpha)
        {
            SetCanvasGroupVisible(trueTextObject, result);
            SetCanvasGroupVisible(falseTextObject, !result);
        }
        else // SetActive
        {
            if (trueTextObject) trueTextObject.gameObject.SetActive(result);
            if (falseTextObject) falseTextObject.gameObject.SetActive(!result);
        }
    }

    // --- helpers ---
    private static bool Evaluate(int a, int b, Comparison op)
    {
        switch (op)
        {
            case Comparison.Greater: return a > b;
            case Comparison.GreaterOrEqual: return a >= b;
            case Comparison.Equal: return a == b;
            case Comparison.NotEqual: return a != b;
            case Comparison.LessOrEqual: return a <= b;
            case Comparison.Less: return a < b;
            default: return false;
        }
    }

    private void SetText(Graphic target, string text)
    {
        if (!target) return;
        if (target is Text t) t.text = text;
        else if (target is TMP_Text tm) tm.text = text;
    }

    private void EnsureCanvasGroup(Graphic g)
    {
        if (showMode != ShowMode.CanvasGroupAlpha || !g) return;
        if (!g.GetComponent<CanvasGroup>()) g.gameObject.AddComponent<CanvasGroup>();
    }

    private void SetCanvasGroupVisible(Graphic g, bool visible)
    {
        if (!g) return;
        var cg = g.GetComponent<CanvasGroup>();
        if (!cg) cg = g.gameObject.AddComponent<CanvasGroup>();
        cg.alpha = visible ? 1f : 0f;
        cg.interactable = visible;
        cg.blocksRaycasts = visible;
    }

    private bool ValidateSetup()
    {
        if (statVariable == null)
        {
            Debug.LogWarning("[ConditionalStatTextDisplay] No StatVariable assigned.", this);
            return false;
        }
        if (targetMode == TargetMode.Single && !singleTextObject)
        {
            Debug.LogWarning("[ConditionalStatTextDisplay] Single mode selected but no Single Text Object assigned.", this);
            return false;
        }
        if (targetMode == TargetMode.Dual && !trueTextObject && !falseTextObject)
        {
            Debug.LogWarning("[ConditionalStatTextDisplay] Dual mode: assign at least one of TRUE/FALSE targets.", this);
            return false;
        }
        return true;
    }
}
