using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text.RegularExpressions; // Para validar el email

public class UserManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField nameInput;
    public TMP_InputField emailInput;
    public TMP_Dropdown factionDropdown;
    public Button startButton;
    public TextMeshProUGUI errorText; // Un texto pequeño en rojo para avisos

    [Header("Panels")]
    public GameObject panelLogin;
    public GameObject panelTutorial;

    void Start()
    {
        errorText.text = ""; // Limpiar errores al iniciar
        // Recuperar si ya existía un usuario (Opcional para agilidad)
        if (PlayerPrefs.HasKey("UserName"))
        {
            nameInput.text = PlayerPrefs.GetString("UserName");
        }
    }

    public void OnClickStart()
    {
        string username = nameInput.text.Trim();
        string email = emailInput.text.Trim();

        // 1. Validación de campos vacíos
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email))
        {
            ShowError("ERROR: Credenciales incompletas.");
            return;
        }

        // 2. Validación de Email (Ingeniería de Software básica)
        if (!IsValidEmail(email))
        {
            ShowError("ERROR: Enlace de comunicación no válido.");
            return;
        }

        // 3. Guardado Pro con PlayerPrefs
        PlayerPrefs.SetString("UserName", username);
        PlayerPrefs.SetString("UserEmail", email);
        PlayerPrefs.SetString("UserFaction", factionDropdown.options[factionDropdown.value].text);
        PlayerPrefs.SetInt("CurrentMission", 1); // Empezamos en la 1/6
        PlayerPrefs.SetInt("TotalPoints", 0);
        PlayerPrefs.Save();

        // 4. Transición Estética
        StartCoroutine(TransitionSequence());
    }

    private bool IsValidEmail(string email)
    {
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }

    private void ShowError(string msg)
    {
        errorText.text = msg;
        // Opcional: Podrías añadir un efecto de sonido de error aquí
    }

    System.Collections.IEnumerator TransitionSequence()
    {
        // Aquí podrías poner una animación de carga o un sonido de "Acceso Concedido"
        yield return new WaitForSeconds(0.5f);
        panelLogin.SetActive(false);
        panelTutorial.SetActive(true);
    }
}