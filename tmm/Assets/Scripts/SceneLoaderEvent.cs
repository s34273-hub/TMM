using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[AddComponentMenu("Utilities/Scene Loader Event")]
public class SceneLoaderEvent : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("The name of the scene to load (must be added in Build Settings).")]
    public string sceneName;

    [Tooltip("Optional: load scene additively instead of replacing the current one.")]
    public bool loadAdditively = false;

    [Tooltip("Optional delay before the scene loads (seconds).")]
    [Min(0f)] public float loadDelay = 0f;

    [Header("Events")]
    [Tooltip("Invoked immediately before the scene begins loading.")]
    public UnityEvent onBeforeLoad;

    [Tooltip("Invoked immediately after scene load call (not after async complete).")]
    public UnityEvent onAfterLoad;

    /// <summary>
    /// Call this from the Unity Event System to load the configured scene.
    /// </summary>
    public void LoadScene()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("[SceneLoaderEvent] Scene name not set.");
            return;
        }

        onBeforeLoad?.Invoke();

        if (loadDelay > 0f)
            Invoke(nameof(PerformLoad), loadDelay);
        else
            PerformLoad();
    }

    private void PerformLoad()
    {
        var mode = loadAdditively ? LoadSceneMode.Additive : LoadSceneMode.Single;
        SceneManager.LoadScene(sceneName, mode);

        onAfterLoad?.Invoke();
    }

#if UNITY_EDITOR
    [ContextMenu("Load Scene Now (Editor)")]
    private void ContextLoadNow() => LoadScene();
#endif
}
