using UnityEngine;
using UnityEngine.Events;

[AddComponentMenu("Game/Variables/Stat Variable")]
public class StatVariable : MonoBehaviour
{
    [Header("Variable")]
    [Tooltip("Name for identification only (e.g., Health, Coins, Sanity).")]
    public string variableName = "Stat";

    [Tooltip("Current value of this stat.")]
    public int currentValue = 0;

    [Tooltip("Prevent this value from dropping below zero.")]
    public bool clampToZero = true;

    [System.Serializable] public class IntEvent : UnityEvent<int> { }

    [Header("Events")]
    [Tooltip("Invoked whenever the value changes. Passes the new value.")]
    public IntEvent onValueChanged;

    /// <summary>Adjust the stat by a delta (positive or negative).</summary>
    public void Add(int amount)
    {
        int newValue = currentValue + amount;
        if (clampToZero && newValue < 0)
            newValue = 0;

        if (newValue == currentValue)
            return;

        currentValue = newValue;
        onValueChanged?.Invoke(currentValue);
    }

    /// <summary>Decrease the stat by amount (uses Add internally).</summary>
    public void Subtract(int amount) => Add(-Mathf.Abs(amount));

    /// <summary>Set the stat directly.</summary>
    public void SetValue(int value)
    {
        int newValue = clampToZero && value < 0 ? 0 : value;
        if (newValue == currentValue)
            return;

        currentValue = newValue;
        onValueChanged?.Invoke(currentValue);
    }

    /// <summary>Return the current value (for UI binding, etc.).</summary>
    public int GetValue() => currentValue;
}
