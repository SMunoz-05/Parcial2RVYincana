using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;

public class MultiImageTracker : MonoBehaviour
{
    [Header("Ajustes de Equipo")]
    public GameObject[] navesPrefabs; // Arrastra aquí tus 6 prefabs de naves
    private Dictionary<string, GameObject> navesDiccionario = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> navesInstanciadas = new Dictionary<string, GameObject>();
    private ARTrackedImageManager trackedImageManager;

    void Awake()
    {
        trackedImageManager = GetComponent<ARTrackedImageManager>();

        // Llenamos el diccionario con el nombre de la imagen y su nave
        // Asegúrate que el nombre del Prefab sea igual al de la imagen en la Library
        foreach (GameObject nave in navesPrefabs)
        {
            navesDiccionario.Add(nave.name, nave);
        }
    }

    void OnEnable() => trackedImageManager.trackablesChanged.AddListener(OnChanged);
    void OnDisable() => trackedImageManager.trackablesChanged.RemoveListener(OnChanged);

    void OnChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        foreach (var newImage in eventArgs.added)
        {
            string imageName = newImage.referenceImage.name;

            if (navesDiccionario.ContainsKey(imageName))
            {
                // Instanciamos la nave específica para esta tarjeta
                GameObject naveInstanciada = Instantiate(navesDiccionario[imageName], newImage.transform);
                navesInstanciadas.Add(imageName, naveInstanciada);


                // === AQUI SE CONECTA EL QUIZ ===
                if (QuizManager.instance != null)
                {
                    QuizManager.instance.ActivarQuiz(imageName);
                }
            }

        }

        // Si la imagen se oculta, podemos ocultar la nave (opcional)
        foreach (var updatedImage in eventArgs.updated)
        {
            navesInstanciadas[updatedImage.referenceImage.name].SetActive(
                updatedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking
            );
        }
    }
}