using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    [Header("Panels de Pasos")]
    public CanvasGroup[] steps; // Cambiamos GameObject por CanvasGroup
    private int currentStep = 0;

    [Header("UI Elements")]
    public Button nextButton;
    public TextMeshProUGUI buttonText;

    [Header("Configuración")]
    public float fadeDuration = 0.5f;
    private AudioSource audioSource;

    [Header("Finalización")]
    public GameObject panelTutorial;
    public GameObject panelHUD_AR;
    public GameObject arSessionOrigin;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        // Apagamos todo y prendemos solo el primero
        foreach (var step in steps)
        {
            step.alpha = 0;
            step.gameObject.SetActive(false);
        }

        steps[0].gameObject.SetActive(true);
        StartCoroutine(FadeIn(steps[0]));
        arSessionOrigin.SetActive(false);
    }

    public void OnClickNext()
    {
        if (audioSource != null) audioSource.Play();
        StartCoroutine(SwitchStep());
    }

    IEnumerator SwitchStep()
    {
        nextButton.interactable = false;

        // 1. Desvanecer el paso actual
        yield return StartCoroutine(FadeOut(steps[currentStep]));
        steps[currentStep].gameObject.SetActive(false);

        currentStep++;

        if (currentStep < steps.Length)
        {
            // 2. Aparecer el siguiente paso
            steps[currentStep].gameObject.SetActive(true);
            yield return StartCoroutine(FadeIn(steps[currentStep]));

            UpdateUI();
            nextButton.interactable = true;
        }
        else
        {
            FinishTutorial();
        }
    }

    // Lógica pura de Fade (Igual que en el Login)
    IEnumerator FadeIn(CanvasGroup cg)
    {
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            cg.alpha = t / fadeDuration;
            yield return null;
        }
        cg.alpha = 1;
    }

    IEnumerator FadeOut(CanvasGroup cg)
    {
        float t = fadeDuration;
        while (t > 0)
        {
            t -= Time.deltaTime;
            cg.alpha = t / fadeDuration;
            yield return null;
        }
        cg.alpha = 0;
    }

    void UpdateUI()
    {
        buttonText.text = (currentStep == steps.Length - 1) ? "¡comenzar!" : "siguiente";
    }

    void FinishTutorial()
    {
        panelTutorial.SetActive(false);
        panelHUD_AR.SetActive(true);
        arSessionOrigin.SetActive(true);
    }
}