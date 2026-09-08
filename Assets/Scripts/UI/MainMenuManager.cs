using DG.Tweening;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance { get; private set; }

    [Header("Important Components")]
    [SerializeField] private GameModeToggleGroup gameModeToggleGroup;
    [SerializeField] private CanvasGroup lobbyCanvas;
    [SerializeField] private CanvasGroup homePage;

    [Header("Menu Options")]
    [SerializeField] private float fadeDuration = 0.5f;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
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
        await LobbyManager.Instance.StartSearching(getGameMode());
        if (LobbyManager.Instance.GetLobby() != null) {
            ShowMenu(lobbyCanvas);
        } else {
            ShowMenu(homePage);
        }
    }
    public void SetReady(bool isReady) {
        LobbyManager.Instance.UpdatePlayerReadyStatus(isReady);
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