using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class UserManager : MonoBehaviour
{
    // Header UI References
    public TMP_InputField nameInput;
    public TMP_InputField emailInput;
    public Button startButton;
    public TextMeshProUGUI errorText;

    // Botones de facción
    public Button btnResistencia;
    public Button btnImperio;
    private string selectedFaction = ""; // guarda cuál eligió

    // Panels
    public GameObject panelLogin;
    public GameObject panelTutorial;

    void Start()
    {
        errorText.text = "";

        if (PlayerPrefs.HasKey("UserName"))
            nameInput.text = PlayerPrefs.GetString("UserName");

        // Asignar listeners a los botones de facción
        btnResistencia.onClick.AddListener(() => SelectFaction("Resistencia"));
        btnImperio.onClick.AddListener(() => SelectFaction("Imperio"));
    }

    public void SelectFaction(string faction)
    {
        selectedFaction = faction;
        errorText.text = ""; // limpiar error si había
                             // Resaltar botón seleccionado visualmente
        ColorBlock on = btnResistencia.colors;
        on.normalColor = new Color(1f, 0.9f, 0.2f, 1f); // dorado
        ColorBlock off = btnResistencia.colors;
        off.normalColor = Color.white;
        btnResistencia.colors = (faction == "Resistencia") ? on : off;
        btnImperio.colors = (faction == "Imperio") ? on : off;
    }

    public void OnClickStart()
    {
        string username = nameInput.text.Trim();
        string email = emailInput.text.Trim();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email))
        { ShowError("ERROR: Credenciales incompletas."); return; }

        if (!IsValidEmail(email))
        { ShowError("ERROR: Enlace de comunicacion no valido."); return; }

        if (string.IsNullOrEmpty(selectedFaction))
        { ShowError("ERROR: Debes elegir una faccion."); return; }

        PlayerPrefs.SetString("UserName", username);
        PlayerPrefs.SetString("UserEmail", email);
        PlayerPrefs.SetString("UserFaction", selectedFaction);
        PlayerPrefs.SetInt("CurrentMission", 1);
        PlayerPrefs.SetInt("TotalPoints", 0);
        PlayerPrefs.Save();

        panelLogin.SetActive(false);
        panelTutorial.SetActive(true);
    }

    private bool IsValidEmail(string email)
    { return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"); }

    private void ShowError(string msg)
    { errorText.text = msg; }

    System.Collections.IEnumerator TransitionSequence()
    {
        Debug.Log("Transición iniciada");
        yield return new WaitForSeconds(0.5f);
        Debug.Log("Desactivando login, activando tutorial");
        panelLogin.SetActive(false);
        panelTutorial.SetActive(true);
        Debug.Log("Listo");
    }

    public void TestBoton()
    {
        Debug.Log("TEST FUNCIONANDO");
    }
}