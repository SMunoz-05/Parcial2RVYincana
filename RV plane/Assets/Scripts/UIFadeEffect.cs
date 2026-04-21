using UnityEngine;
using System.Collections;

public class UIFadeEffect : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    public float duration = 1.5f; // Duración del efecto

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void OnEnable() // Se ejecuta cada vez que el panel se activa
    {
        StartCoroutine(DoFadeIn());
    }

    IEnumerator DoFadeIn()
    {
        float counter = 0f;
        canvasGroup.alpha = 0;

        // Efecto de escala inicial (opcional para el 5.0)
        transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);

        while (counter < duration)
        {
            counter += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, counter / duration);

            // Suavizamos la escala hacia el tamaño normal (1,1,1)
            transform.localScale = Vector3.Lerp(new Vector3(0.9f, 0.9f, 0.9f), Vector3.one, counter / duration);

            yield return null;
        }
    }
}