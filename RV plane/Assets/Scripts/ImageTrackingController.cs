using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ImageTrackingController : MonoBehaviour
{
    private ARTrackedImageManager _trackedImageManager;

    void Awake()
    {
        _trackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    void OnEnable()
    {
        if (_trackedImageManager != null)
        {
            _trackedImageManager.trackablesChanged.AddListener(OnChanged);
        }
    }

    void OnDisable()
    {
        if (_trackedImageManager != null)
        {
            _trackedImageManager.trackablesChanged.RemoveListener(OnChanged);
        }
    }

    void OnChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        foreach (var newImage in eventArgs.added)
        {
            Debug.Log("Nave detectada: " + newImage.referenceImage.name);

            AudioSource naveAudio = newImage.GetComponentInChildren<AudioSource>();
            if (naveAudio != null)
            {
                naveAudio.Play();
            }

            int puntosActuales = PlayerPrefs.GetInt("TotalPoints", 0);
            PlayerPrefs.SetInt("TotalPoints", puntosActuales + 100);
            PlayerPrefs.Save();

            Debug.Log("Puntos totales: " + PlayerPrefs.GetInt("TotalPoints"));
        }
    }
}