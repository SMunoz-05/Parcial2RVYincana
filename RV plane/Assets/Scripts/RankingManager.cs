using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class RankingManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI listaNombresText;
    public TextMeshProUGUI listaPuntosText;

    void Start()
    {
        GuardarPuntajeActual();
        MostrarRanking();
    }

    void GuardarPuntajeActual()
    {
        string nombre = PlayerPrefs.GetString("UserName", "Invitado");
        int puntos = PlayerPrefs.GetInt("TotalPoints", 0);

        // Creamos una clave única para este intento o usamos un sistema de lista simple
        // Para este parcial, lo guardaremos en una cadena formateada para simular una base de datos local
        string historial = PlayerPrefs.GetString("RankingData", "");
        historial += nombre + ":" + puntos + "|";
        PlayerPrefs.SetString("RankingData", historial);
        PlayerPrefs.Save();
    }

    void MostrarRanking()
    {
        string data = PlayerPrefs.GetString("RankingData", "");
        string[] entradas = data.Split('|');

        List<KeyValuePair<string, int>> listaPuntajes = new List<KeyValuePair<string, int>>();

        foreach (string entrada in entradas)
        {
            if (string.IsNullOrEmpty(entrada)) continue;
            string[] datos = entrada.Split(':');
            listaPuntajes.Add(new KeyValuePair<string, int>(datos[0], int.Parse(datos[1])));
        }

        // Ordenar de mayor a menor
        listaPuntajes.Sort((x, y) => y.Value.CompareTo(x.Value));

        listaNombresText.text = "";
        listaPuntosText.text = "";

        // Mostrar solo los top 5
        int limite = Mathf.Min(listaPuntajes.Count, 5);
        for (int i = 0; i < limite; i++)
        {
            listaNombresText.text += (i + 1) + ". " + listaPuntajes[i].Key + "\n";
            listaPuntosText.text += listaPuntajes[i].Value + " pts\n";
        }
    }
    public void BorrarTodoElHistorial()
    {
        PlayerPrefs.DeleteKey("RankingData");
        // También podrías usar PlayerPrefs.DeleteAll(); si quieres borrar nombres y correos
        Debug.Log("Ranking reseteado");

        // Recargamos la escena para que la tabla se vea vacía de inmediato
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
    public void VolverAlInicio()
    {
        // Limpiamos los datos del jugador actual para el siguiente, 
        // pero NO el RankingData
        PlayerPrefs.DeleteKey("UserName");
        PlayerPrefs.DeleteKey("UserEmail");
        PlayerPrefs.DeleteKey("TotalPoints");

        SceneManager.LoadScene(0); // Carga la escena del Login (asegúrate que sea la 0)
    }
}