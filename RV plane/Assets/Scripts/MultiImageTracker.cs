using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections;
using System.Collections.Generic;

public class MultiImageTracker : MonoBehaviour
{
    [System.Serializable]
    public class ImagePrefabBinding
    {
        public string imageName;
        public GameObject navePrefab;
    }

    [Header("Ajustes de Equipo")]
    public GameObject[] navesPrefabs;
    [Tooltip("Mapeo explicito: nombre de imagen en library -> prefab a instanciar")]
    public ImagePrefabBinding[] imageBindings;

    [Header("Secuencia al detectar tarjeta")]
    public GameObject detectionAnimationPrefab;
    public float animationDuration = 2f;
    public Vector3 animationLocalOffset = new Vector3(0f, 0.1f, 0f);

    private Dictionary<string, GameObject> navesDiccionario = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> navesInstanciadas = new Dictionary<string, GameObject>();
    private HashSet<string> quizDisparadoPorTarjeta = new HashSet<string>();
    private HashSet<string> missingBindingWarningShown = new HashSet<string>();

    private ARTrackedImageManager trackedImageManager;

    void Awake()
    {
        trackedImageManager = GetComponent<ARTrackedImageManager>();

        if (trackedImageManager == null)
        {
            Debug.LogError("[MultiImageTracker] No ARTrackedImageManager found on this GameObject.");
            enabled = false;
            return;
        }

        // Prioridad 1: mapeo explicito de imagen -> prefab.
        foreach (ImagePrefabBinding binding in imageBindings)
        {
            if (binding == null || string.IsNullOrWhiteSpace(binding.imageName) || binding.navePrefab == null)
                continue;

            navesDiccionario[binding.imageName] = binding.navePrefab;
        }

        // Fallback: por nombre de prefab, por compatibilidad.
        foreach (GameObject nave in navesPrefabs)
        {
            if (nave == null)
                continue;

            if (!navesDiccionario.ContainsKey(nave.name))
                navesDiccionario.Add(nave.name, nave);
        }
    }

    void OnEnable()
    {
        if (trackedImageManager != null)
            trackedImageManager.trackablesChanged.AddListener(OnChanged);
    }

    void OnDisable()
    {
        if (trackedImageManager != null)
            trackedImageManager.trackablesChanged.RemoveListener(OnChanged);
    }

    void OnChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        foreach (var newImage in eventArgs.added)
        {
            HandleTrackedImage(newImage);
        }

        foreach (var updatedImage in eventArgs.updated)
        {
            HandleTrackedImage(updatedImage);

            string imageName = updatedImage.referenceImage.name;
            if (navesInstanciadas.TryGetValue(imageName, out GameObject naveInstanciada))
            {
                naveInstanciada.SetActive(updatedImage.trackingState == TrackingState.Tracking);
            }
        }
    }

    private void HandleTrackedImage(ARTrackedImage trackedImage)
    {
        if (trackedImage.trackingState != TrackingState.Tracking)
            return;

        string imageName = trackedImage.referenceImage.name;

        if (!navesInstanciadas.ContainsKey(imageName))
        {
            if (navesDiccionario.TryGetValue(imageName, out GameObject navePrefab))
            {
                GameObject naveInstanciada = Instantiate(navePrefab, trackedImage.transform);
                navesInstanciadas[imageName] = naveInstanciada;
            }
            else if (!missingBindingWarningShown.Contains(imageName))
            {
                missingBindingWarningShown.Add(imageName);
                Debug.LogWarning($"[MultiImageTracker] No prefab mapped for image '{imageName}'.");
            }
        }

        if (!quizDisparadoPorTarjeta.Contains(imageName))
        {
            StartCoroutine(PlayAnimationThenShowQuiz(imageName, trackedImage.transform));
        }
    }

    private IEnumerator PlayAnimationThenShowQuiz(string imageName, Transform imageTransform)
    {
        quizDisparadoPorTarjeta.Add(imageName);

        GameObject fxInstance = null;
        if (detectionAnimationPrefab != null && imageTransform != null)
        {
            fxInstance = Instantiate(detectionAnimationPrefab, imageTransform);
            fxInstance.transform.localPosition = animationLocalOffset;
            fxInstance.transform.localRotation = Quaternion.identity;
        }

        yield return new WaitForSeconds(animationDuration);

        if (fxInstance != null)
            Destroy(fxInstance);

        if (QuizManager.instance != null)
        {
            QuizManager.instance.ActivarQuiz(imageName);
        }
        else
        {
            Debug.LogWarning("[MultiImageTracker] QuizManager.instance is null, cannot show quiz UI.");
        }
    }
}