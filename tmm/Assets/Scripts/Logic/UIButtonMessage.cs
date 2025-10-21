using UnityEngine;

public class UIButtonMessage : MonoBehaviour
{
    [TextArea]
    [SerializeField] private string message;

    // This method can be hooked up to a Button OnClick event in the Inspector
    public void PrintMessage()
    {
        Debug.Log($"[UIButtonMessage] {message}");
    }

    // Optional: expose a version that accepts a parameter if needed
    public void PrintCustom(string customMessage)
    {
        Debug.Log($"[UIButtonMessage] {customMessage}");
    }
}
