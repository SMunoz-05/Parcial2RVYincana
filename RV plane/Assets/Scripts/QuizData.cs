using UnityEngine;

[System.Serializable]
public class QuizData
{
    public string idTarjeta; // Debe coincidir con el nombre en la Library (ej: "Tarjeta_Alfa")
    [TextArea(3, 5)]
    public string pregunta;
    public string[] opciones; // Un array de 3 o 4 opciones
    public int respuestaCorrectaIndex; // 0, 1, 2... según cuál sea la correcta
}