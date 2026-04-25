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

    [Header("Sonidos")]
    public AudioSource audioSource;
    public AudioClip correctSound;
    public AudioClip wrongSound;
    public AudioClip clickSound;

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

    public void ActivarQuiz(string idTarjeta)
    {
        preguntasDeEstaTarjeta.Clear();
        indicePreguntaLocal = 0;
        foreach (QuizData q in bancoPreguntas)
        {
            if (q.idTarjeta == idTarjeta)
                preguntasDeEstaTarjeta.Add(q);
        }
        if (preguntasDeEstaTarjeta.Count > 0)
            MostrarSiguientePregunta();
    }

    void MostrarSiguientePregunta()
    {
        preguntaActual = preguntasDeEstaTarjeta[indicePreguntaLocal];
        panelQuiz.SetActive(true);
        textoPregunta.text = preguntaActual.pregunta;

        for (int i = 0; i < botonesOpciones.Length; i++)
        {
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
                botonesOpciones[i].gameObject.SetActive(false);
            }
        }
    }

    void Responder(int indiceSeleccionado)
    {
        // Sonido de click siempre
        if (audioSource != null && clickSound != null)
            audioSource.PlayOneShot(clickSound);

        if (indiceSeleccionado == preguntaActual.respuestaCorrectaIndex)
        {
            puntosTotales += 100;
            ActualizarPuntosUI();
            // Sonido correcto
            if (audioSource != null && correctSound != null)
                audioSource.PlayOneShot(correctSound);
        }
        else
        {
            // Sonido incorrecto
            if (audioSource != null && wrongSound != null)
                audioSource.PlayOneShot(wrongSound);
        }

        indicePreguntaLocal++;
        if (indicePreguntaLocal < preguntasDeEstaTarjeta.Count)
        {
            MostrarSiguientePregunta();
        }
        else
        {
            panelQuiz.SetActive(false);
            tarjetasCompletadas++;
            if (tarjetasCompletadas >= totalTarjetas)
                FinalizarJuego();
        }
    }

    void FinalizarJuego()
    {
        if (panelTutorial != null) panelTutorial.SetActive(false);
        if (panelHUD_AR != null) panelHUD_AR.SetActive(true);
        if (botonFinalizar != null) botonFinalizar.SetActive(true);
        Debug.Log("¡Yincana Terminada!");
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