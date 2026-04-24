using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class QuizManager : MonoBehaviour
{
    public static QuizManager instance;

    [Header("Base de Datos")]
    public QuizData[] bancoPreguntas;
    private List<QuizData> preguntasDeEstaTarjeta = new List<QuizData>();
    private QuizData preguntaActual;
    private int indicePreguntaLocal = 0;

    [Header("UI del Quiz")]
    public GameObject panelQuiz;
    public TextMeshProUGUI textoPregunta;
    public Button[] botonesOpciones;
    public TextMeshProUGUI textoPuntos;

    [Header("Progreso Global")]
    private int puntosTotales = 0;
    public int tarjetasCompletadas = 0;
    public int totalTarjetas = 6;
    public GameObject botonFinalizar;

    [Header("Referencias de Flujo")]
    public GameObject panelHUD_AR;
    public GameObject panelTutorial;

    void Awake()
    {
        instance = this;
        panelQuiz.SetActive(false);
    }

    void Start()
    {
        if (botonFinalizar != null) botonFinalizar.SetActive(false);
        ActualizarPuntosUI();
    }

    // Se activa desde el MultiImageTracker cuando detecta una imagen
    public void ActivarQuiz(string idTarjeta)
    {
        // 1. Limpiamos la lista local y buscamos todas las preguntas de esa tarjeta
        preguntasDeEstaTarjeta.Clear();
        indicePreguntaLocal = 0;

        foreach (QuizData q in bancoPreguntas)
        {
            if (q.idTarjeta == idTarjeta)
            {
                preguntasDeEstaTarjeta.Add(q);
            }
        }

        // 2. Si encontramos preguntas, lanzamos la primera
        if (preguntasDeEstaTarjeta.Count > 0)
        {
            MostrarSiguientePregunta();
        }
    }

    void MostrarSiguientePregunta()
    {
        preguntaActual = preguntasDeEstaTarjeta[indicePreguntaLocal];
        panelQuiz.SetActive(true);
        textoPregunta.text = preguntaActual.pregunta;

        for (int i = 0; i < botonesOpciones.Length; i++)
        {
            // Verificamos que la pregunta tenga suficientes opciones para los botones
            if (i < preguntaActual.opciones.Length)
            {
                botonesOpciones[i].gameObject.SetActive(true);
                botonesOpciones[i].GetComponentInChildren<TextMeshProUGUI>().text = preguntaActual.opciones[i];

                int index = i;
                botonesOpciones[i].onClick.RemoveAllListeners();
                botonesOpciones[i].onClick.AddListener(() => Responder(index));
            }
            else
            {
                botonesOpciones[i].gameObject.SetActive(false); // Escondemos botones si sobran
            }
        }
    }

    void Responder(int indiceSeleccionado)
    {
        // 1. Validar respuesta
        if (indiceSeleccionado == preguntaActual.respuestaCorrectaIndex)
        {
            puntosTotales += 100;
            ActualizarPuntosUI();
        }

        // 2. Pasar a la siguiente pregunta de la MISMA tarjeta
        indicePreguntaLocal++;

        if (indicePreguntaLocal < preguntasDeEstaTarjeta.Count)
        {
            MostrarSiguientePregunta();
        }
        else
        {
            // Ya no hay más preguntas para ESTA tarjeta
            panelQuiz.SetActive(false);
            tarjetasCompletadas++;

            if (tarjetasCompletadas >= totalTarjetas)
            {
                FinalizarJuego();
            }
        }
    }

    void FinalizarJuego()
    {
        if (panelTutorial != null) panelTutorial.SetActive(false);
        if (panelHUD_AR != null) panelHUD_AR.SetActive(true);
        if (botonFinalizar != null) botonFinalizar.SetActive(true);

        Debug.Log("¡Yincana Terminada! Todas las tarjetas completadas.");
    }

    void ActualizarPuntosUI()
    {
        if (textoPuntos != null) textoPuntos.text = "Puntos: " + puntosTotales;
        PlayerPrefs.SetInt("TotalPoints", puntosTotales);
    }

    public void IrAlRanking()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Escena_Ranking");
    }
}