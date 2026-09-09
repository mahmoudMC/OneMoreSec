using DG.Tweening;
using System;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginPage : MonoBehaviour
{

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
    [SerializeField] private CanvasGroup statusCanvas;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private float fadeDuration = 0.5f;

    private async void Start() {
        // Initialize Unity Gaming Services
        try {
            if (UnityServices.State == ServicesInitializationState.Uninitialized) {
                await UnityServices.InitializeAsync();
            }
            // Check if user has a valid cached session token
            if (AuthenticationService.Instance.SessionTokenExists) {

                // Re-authenticates using the existing session token
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                OnAuthenticationSuccess();
                return;
            }
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

        try {
            // 1. Create username/password account
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);

            // 2. Set the player's in-game display name
            await AuthenticationService.Instance.UpdatePlayerNameAsync(playerName);

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

    public void TogglePasswordVisibility(TMP_InputField inputField) {
        if (inputField == null) return;

        if (inputField.contentType == TMP_InputField.ContentType.Password) {
            inputField.contentType = TMP_InputField.ContentType.Standard;
        } else {
            inputField.contentType = TMP_InputField.ContentType.Password;
        }

        inputField.ForceLabelUpdate();
    }

    public void ShowMenu(CanvasGroup menu) {
        menu.gameObject.SetActive(true);
        menu.DOFade(1, fadeDuration).From(0);
    }
    public void HideMenu(CanvasGroup menu) {
        menu.DOFade(0, fadeDuration).OnComplete(() => {
            menu.gameObject.SetActive(false);
        });
    }

    private void OnAuthenticationSuccess() {
        // Proceed to Lobby scene or main menu UI
        print($"Player Authenticated. ID: {AuthenticationService.Instance.PlayerId}");
        SceneManager.LoadScene(1);
    }

    private void SetStatus(string message) {
        if (statusText == null || statusCanvas == null) return;
        ShowMenu(statusCanvas);
        statusText.SetText(message);
    }
}
