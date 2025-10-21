using UnityEngine;
using UnityEngine.Events;

[AddComponentMenu("Utilities/Delayed Unity Event")]
public class DelayedUnityEvent : MonoBehaviour
{
    [Header("Timing")]
    [Tooltip("Delay (in seconds) before invoking the event.")]
    [Min(0f)] public float delay = 0.5f;

    [Tooltip("Automatically trigger when this component is enabled.")]
    public bool invokeOnEnable = false;

    [Tooltip("Use unscaled time (ignores Time.timeScale).")]
    public bool useUnscaledTime = false;

    [Header("Event")]
    [Tooltip("Invoked after the delay elapses.")]
    public UnityEvent onDelayElapsed;

    private Coroutine _running;

    private void OnEnable()
    {
        if (invokeOnEnable)
            Trigger();
    }

    /// <summary>Start (or restart) the delayed event.</summary>
    public void Trigger()
    {
        if (_running != null)
            StopCoroutine(_running);
        _running = StartCoroutine(InvokeRoutine());
    }

    /// <summary>Cancel a pending delayed event (if any).</summary>
    public void Cancel()
    {
        if (_running != null)
        {
            StopCoroutine(_running);
            _running = null;
        }
    }

    private System.Collections.IEnumerator InvokeRoutine()
    {
        if (useUnscaledTime)
        {
            float end = Time.unscaledTime + delay;
            while (Time.unscaledTime < end)
                yield return null;
        }
        else
        {
            yield return new WaitForSeconds(delay);
        }

        _running = null;
        onDelayElapsed?.Invoke();
    }

#if UNITY_EDITOR
    [ContextMenu("Trigger (Editor)")]
    private void ContextTrigger() => Trigger();
#endif
}
