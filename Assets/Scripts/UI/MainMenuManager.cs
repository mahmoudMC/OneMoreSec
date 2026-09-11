using DG.Tweening;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance { get; private set; }

    [Header("Important Components")]
    [SerializeField] private GameModeToggleGroup gameModeToggleGroup;
    [SerializeField] private CanvasGroup lobbyCanvas;
    [SerializeField] private CanvasGroup homePage;
    [SerializeField] private TextMeshProUGUI welcomeText;

    [Header("Menu Options")]
    [SerializeField] private float fadeDuration = 0.5f;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void Start() {
        string playerName = "Guest";
        if (AuthenticationService.Instance.IsSignedIn) {
            playerName = AuthenticationService.Instance.PlayerName.Split('#')[0];
        }
        welcomeText.SetText($"Welcome,\n{playerName}");
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
    public void ExitGame() {
        Application.Quit();
    }
    public GameMode getGameMode() {
        return gameModeToggleGroup.GetSelectedGameMode();
    }
    public async void SearchForGame() {
        LoadingManager.Instance.StartLoading();
        await LobbyManager.Instance.StartSearching(getGameMode());
        LoadingManager.Instance.FinishLoading();
        if (LobbyManager.Instance.GetLobby() != null) {
            ShowMenu(lobbyCanvas);
        } else {
            ShowMenu(homePage);
        }
    }
    public async void SetReady(bool isReady) {
        LoadingManager.Instance.StartLoading();
        await LobbyManager.Instance.UpdatePlayerReadyStatus(isReady);
        LoadingManager.Instance.FinishLoading();
    }
    public void LeaveLobby() {
        LobbyManager.Instance.LeaveLobby();
        HideMenu(lobbyCanvas);
        ShowMenu(homePage);
    }
}

public enum GameMode {
    FT1,
    FT3,
    FT5
}