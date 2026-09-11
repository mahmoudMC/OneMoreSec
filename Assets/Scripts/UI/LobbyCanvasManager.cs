using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyCanvasManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gameModeText;
    [SerializeField] private Transform playerTemplateParent;
    [SerializeField] private PlayerTemplate playerTemplatePrefab;
    private List<PlayerTemplate> playerTemplates = new List<PlayerTemplate>();

    private void Awake() {
        playerTemplatePrefab.gameObject.SetActive(false);
        Application.runInBackground = true;
    }
    private void OnEnable() {
        LobbyManager.Instance.OnLobbyPulled += Instance_OnLobbyPulled;
        // set up UI for the current game mode
        gameModeText.SetText(MainMenuManager.Instance.getGameMode().ToString());

        // setting up the UI for player names
        List<Player> players = LobbyManager.Instance.GetPlayersInLobby();
        bool firstPlayer = LobbyManager.Instance.isHost();
        foreach (Player player in players) {
            PlayerTemplate template = Instantiate(playerTemplatePrefab, playerTemplateParent);
            // check for safety
            string playerName = "Player";
            if (player.Data != null && player.Data.TryGetValue(ConstantKeys.PlayerName.ToString(), out PlayerDataObject nameData)) {
                playerName = nameData.Value;
            }
            bool isReady = false;
            if (player.Data != null && player.Data.TryGetValue(ConstantKeys.isReady.ToString(), out PlayerDataObject readyData)) {
                bool.TryParse(readyData.Value, out isReady);
            }
            template.InitializeTemplate(
                playerName,
                LobbyManager.Instance.isHost() ^ firstPlayer);
            template.UpdateReadyStatus(isReady);

            playerTemplates.Add(template);
            firstPlayer = false;
        }
    }

    private void Instance_OnLobbyPulled(Lobby lobby) {
        if (lobby == null) return;

        // Clear existing UI templates
        ClearTemplate();

        // Populate new templates for every player in the lobby
        bool firstPlayer = LobbyManager.Instance.isHost();
        foreach (Player player in lobby.Players) {
            PlayerTemplate template = Instantiate(playerTemplatePrefab, playerTemplateParent);

            // Extract Player Name safely
            string playerName = "Player";
            if (player.Data != null && player.Data.TryGetValue(ConstantKeys.PlayerName.ToString(), out PlayerDataObject nameData)) {
                playerName = nameData.Value;
            }

            // Extract Ready Status safely
            bool isReady = false;
            if (player.Data != null && player.Data.TryGetValue(ConstantKeys.isReady.ToString(), out PlayerDataObject readyData)) {
                bool.TryParse(readyData.Value, out isReady);
            }

            // Initialize template with player attributes
            template.InitializeTemplate(playerName, LobbyManager.Instance.isHost() ^ firstPlayer);
            template.UpdateReadyStatus(isReady);
            template.AddListenerForKick(() => {
                LobbyManager.Instance.KickPlayer(player.Id);
            });
            firstPlayer = false;
            playerTemplates.Add(template);
        }
    }

    private void OnDisable() {
        LobbyManager.Instance.OnLobbyPulled -= Instance_OnLobbyPulled;
        ClearTemplate();
    }

    private void ClearTemplate() {
        foreach (PlayerTemplate template in playerTemplates) {
            if (template != null) Destroy(template.gameObject);
        }
        playerTemplates.Clear();
    }
}
