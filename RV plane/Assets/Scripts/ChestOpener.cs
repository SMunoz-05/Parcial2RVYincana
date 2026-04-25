using UnityEngine;
using System.Collections;

public class ChestOpener : MonoBehaviour
{
    public Animator cofreAnimator;
    public GameObject tablaRanking;
    public float tiempoDeEspera = 2.0f; // Cuánto dura la animación del cofre

    void Start()
    {
        // Al empezar, la tabla está oculta
        tablaRanking.SetActive(false);
        StartCoroutine(SecuenciaApertura());
    }

    IEnumerator SecuenciaApertura()
    {
        // 1. Disparamos la animación del cofre
        cofreAnimator.SetTrigger("Abrir");

        // 2. Esperamos a que el cofre se abra visualmente
        yield return new WaitForSeconds(tiempoDeEspera);

        // 3. Activamos la tabla de puntajes con un efecto suave
        tablaRanking.SetActive(true);

        // 4. Desactivamos el cofre para que no estorbe
        gameObject.SetActive(false);
    }
}