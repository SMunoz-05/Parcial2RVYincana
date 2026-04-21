using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ImageTrackingController : MonoBehaviour
{
    private ARTrackedImageManager trackedImageManager;

    void Awake()
    {
        trackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    void OnEnable()
    {
        trackedImageManager.trackablesChanged.AddListener(OnChanged);
    }

    void OnDisable()
    {
        trackedImageManager.trackablesChanged.RemoveListener(OnChanged);
    }

    void OnChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        foreach (var newImage in eventArgs.added)
        {
            // 1. Identificar qué nave apareció
            Debug.Log("¡Nave detectada!: " + newImage.referenceImage.name);

            // 2. Feedback Sonoro (Buscamos el audio en el Prefab que acaba de aparecer)
            AudioSource naveAudio = newImage.GetComponentInChildren<AudioSource>();
            if (naveAudio != null)
            {
                naveAudio.Play();
            }

            // 3. Lógica de Puntos (Sumamos 100 puntos por encontrar la nave Alfa)
            int puntosActuales = PlayerPrefs.GetInt("TotalPoints", 0);
            PlayerPrefs.SetInt("TotalPoints", puntosActuales + 100);
            PlayerPrefs.Save();

            // Aquí podrías llamar a una función que actualice el texto en tu HUD
            Debug.Log("Puntos totales: " + PlayerPrefs.GetInt("TotalPoints"));
        }
    }
}