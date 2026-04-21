using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    public static QuizManager instance;

    [Header("Base de Datos")]
    public QuizData[] bancoPreguntas;
    private QuizData preguntaActual;

    [Header("UI del Quiz")]
    public GameObject panelQuiz;
    public TextMeshProUGUI textoPregunta;
    public Button[] botonesOpciones;
    public TextMeshProUGUI textoPuntos;

    private int puntosTotales = 0;

    [Header("Progreso")]
    public int tarjetasCompletadas = 0;
    public int totalTarjetas = 6;
    public GameObject botonFinalizar; // El botón que llevará al Ranking

    [Header("Referencias de Flujo")]
    public GameObject panelHUD_AR; // Arrastra el HUD aquí
    public GameObject panelTutorial; // Arrastra el Panel de Pasos aquí

    void Awake()
    {
        instance = this;
        panelQuiz.SetActive(false);
    }

    void Start()
    {
        if (botonFinalizar != null) botonFinalizar.SetActive(false);
    }

    // Esta función la llamaremos desde el MultiImageTracker
    public void ActivarQuiz(string idTarjeta)
    {
        foreach (QuizData q in bancoPreguntas)
        {
            if (q.idTarjeta == idTarjeta)
            {
                preguntaActual = q;
                MostrarPregunta();
                break;
            }
        }
    }

    void MostrarPregunta()
    {
        panelQuiz.SetActive(true);
        textoPregunta.text = preguntaActual.pregunta;

        for (int i = 0; i < botonesOpciones.Length; i++)
        {
            botonesOpciones[i].GetComponentInChildren<TextMeshProUGUI>().text = preguntaActual.opciones[i];

            int index = i; // Necesario para el Listener
            botonesOpciones[i].onClick.RemoveAllListeners();
            botonesOpciones[i].onClick.AddListener(() => Responder(index));
        }
    }

    void Responder(int indiceSeleccionado)
    {
        if (indiceSeleccionado == preguntaActual.respuestaCorrectaIndex)
        {
            puntosTotales += 100;
            ActualizarPuntosUI();
        }

        panelQuiz.SetActive(false);
        tarjetasCompletadas++;

        // Verificar si ya terminó la Yincana
        if (tarjetasCompletadas >= totalTarjetas)
        {
            FinalizarJuego();
        }
    }
    void FinalizarJuego()
    {
        // 1. Apagamos el tutorial
        if (panelTutorial != null) panelTutorial.SetActive(false);

        // 2. Encendemos el HUD de Realidad Aumentada
        if (panelHUD_AR != null) panelHUD_AR.SetActive(true);

        // 3. Mostramos el botón de Ranking
        if (botonFinalizar != null) botonFinalizar.SetActive(true);

        Debug.Log("Yincana Finalizada. HUD activado.");
    }
    void ActualizarPuntosUI()
    {
        textoPuntos.text = "Puntos: " + puntosTotales;
        PlayerPrefs.SetInt("TotalPoints", puntosTotales);
    }
    public void IrAlRanking()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Escena_Ranking");
    }
}