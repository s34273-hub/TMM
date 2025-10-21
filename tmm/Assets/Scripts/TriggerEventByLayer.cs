using UnityEngine;
using UnityEngine.Events;

[AddComponentMenu("Utilities/Trigger Event By Layer")]
[RequireComponent(typeof(Collider))]
public class TriggerEventByLayer : MonoBehaviour
{
    [Header("Trigger Settings")]
    [Tooltip("Only objects on this layer will trigger the event.")]
    public LayerMask triggerLayer;

    [Tooltip("Only trigger once (disable after firing).")]
    public bool triggerOnce = false;

    [Tooltip("Optional delay before event fires (seconds).")]
    [Min(0f)] public float delay = 0f;

    [Header("Events")]
    [Tooltip("Invoked when a valid object enters the trigger.")]
    public UnityEvent onTriggerEnterEvent;

    [Tooltip("Invoked when a valid object exits the trigger.")]
    public UnityEvent onTriggerExitEvent;

    private bool _hasTriggered = false;

    private void Reset()
    {
        // Ensure collider is set as trigger
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_hasTriggered && triggerOnce)
            return;

        if (IsInLayerMask(other.gameObject.layer))
        {
            if (delay > 0f)
                Invoke(nameof(InvokeEnterEvent), delay);
            else
                InvokeEnterEvent();

            if (triggerOnce)
                _hasTriggered = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsInLayerMask(other.gameObject.layer))
        {
            onTriggerExitEvent?.Invoke();
        }
    }

    private void InvokeEnterEvent()
    {
        onTriggerEnterEvent?.Invoke();
    }

    private bool IsInLayerMask(int layer)
    {
        return (triggerLayer.value & (1 << layer)) != 0;
    }
}
