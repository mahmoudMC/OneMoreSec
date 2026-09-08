using System;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginPage : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject registerPanel;
    [SerializeField] private GameObject guestPanel;

    [Header("Login Fields")]
    [SerializeField] private TMP_InputField loginUsernameInput;
    [SerializeField] private TMP_InputField loginPasswordInput;

    [Header("Register Fields")]
    [SerializeField] private TMP_InputField regUsernameInput;
    [SerializeField] private TMP_InputField regPlayerNameInput;
    [SerializeField] private TMP_InputField regPasswordInput;
    [SerializeField] private TMP_InputField regConfirmPasswordInput;

    [Header("Guest Fields")]
    [SerializeField] private TMP_InputField guestPlayerNameInput;

    [Header("Feedback UI")]
    [SerializeField] private TextMeshProUGUI statusText;

    private async void Start() {
        // Initialize Unity Gaming Services
        try {
            if (UnityServices.State == ServicesInitializationState.Uninitialized) {
                await UnityServices.InitializeAsync();
            }
            ShowLoginPanel();
        } catch (Exception e) {
            SetStatus($"Initialization Failed: {e.Message}");
        }
    }


    public async void Login() {
        string username = loginUsernameInput.text.Trim();
        string password = loginPasswordInput.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) {
            SetStatus("Please fill in all fields.");
            return;
        }

        SetStatus("Signing in...");

        try {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
            SetStatus($"Signed in as: {AuthenticationService.Instance.PlayerName ?? username}");
            OnAuthenticationSuccess();
        } catch (AuthenticationException ex) {
            SetStatus($"Sign in failed: {ex.Message}");
        } catch (RequestFailedException ex) {
            SetStatus($"Request failed: {ex.Message}");
        }
    }


    public async void SignUp() {
        string username = regUsernameInput.text.Trim();
        string playerName = regPlayerNameInput.text.Trim();
        string password = regPasswordInput.text;
        string confirmPassword = regConfirmPasswordInput.text;

        // Validations
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(playerName) ||
            string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword)) {
            SetStatus("All registration fields are required.");
            return;
        }

        if (password != confirmPassword) {
            SetStatus("Passwords do not match.");
            return;
        }

        if (password.Length < 8) {
            SetStatus("Password must be at least 8 characters long.");
            return;
        }

        SetStatus("Creating account...");

        try {
            // 1. Create username/password account
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);

            // 2. Set the player's in-game display name
            await AuthenticationService.Instance.UpdatePlayerNameAsync(playerName);

            SetStatus($"Account created successfully! Welcome {playerName}.");
            OnAuthenticationSuccess();
        } catch (AuthenticationException ex) {
            SetStatus($"Registration failed: {ex.Message}");
        } catch (RequestFailedException ex) {
            SetStatus($"Request failed: {ex.Message}");
        }
    }

    public async void LoginAsGuest() {
        string guestName = guestPlayerNameInput.text.Trim();

        if (string.IsNullOrEmpty(guestName)) {
            SetStatus("Please enter an in-game name.");
            return;
        }

        try {
            // 1. Authenticate anonymously
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            // 2. Assign the chosen in-game name
            await AuthenticationService.Instance.UpdatePlayerNameAsync(guestName);

            OnAuthenticationSuccess();
        } catch (AuthenticationException ex) {
            SetStatus($"Guest login failed: {ex.Message}");
        } catch (RequestFailedException ex) {
            SetStatus($"Request failed: {ex.Message}");
        }
    }

    private void TogglePasswordVisibility(TMP_InputField inputField) {
        if (inputField == null) return;

        if (inputField.contentType == TMP_InputField.ContentType.Password) {
            inputField.contentType = TMP_InputField.ContentType.Standard;
        } else {
            inputField.contentType = TMP_InputField.ContentType.Password;
        }

        inputField.ForceLabelUpdate();
    }

    public void ShowLoginPanel() {
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
        guestPanel.SetActive(false);
    }

    public void ShowRegisterPanel() {
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
        guestPanel.SetActive(false);
    }

    public void ShowGuestPanel() {
        loginPanel.SetActive(false);
        registerPanel.SetActive(false);
        guestPanel.SetActive(true);
    }

    private void OnAuthenticationSuccess() {
        // Proceed to Lobby scene or main menu UI
        Debug.Log($"Player Authenticated. ID: {AuthenticationService.Instance.PlayerId}");
        SceneManager.LoadScene(1);
    }

    private void SetStatus(string message) {
        if (statusText == null) return;
        statusText.SetText(message);
    }
}
