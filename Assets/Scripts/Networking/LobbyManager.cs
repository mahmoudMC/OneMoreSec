using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyManager : MonoBehaviour {

    public static LobbyManager Instance { get; private set; }

    public event Action<Lobby> OnLobbyPulled;
    
    private const byte MAX_Players = 6;

    private string playerName;

    private Lobby currentLobby;
    private ILobbyEvents lobbyEvents;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        playerName = "Player_" + UnityEngine.Random.Range(1000, 9999);
    }

    public async Task StartSearching(GameMode gameMode) {
        try {
            // 1. Try Quick Joining using indexed S1 string filter
            QuickJoinLobbyOptions quickJoinOptions = new QuickJoinLobbyOptions {
                Player = GetPlayer(),
                Filter = new List<QueryFilter> {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT),
                    new QueryFilter(QueryFilter.FieldOptions.S1, gameMode.ToString(), QueryFilter.OpOptions.EQ)
                }
            };
            // quick join the lobby
            currentLobby = await LobbyService.Instance.QuickJoinLobbyAsync(quickJoinOptions);

            // if we joined. subscribe to lobby events
            var callBacks = new LobbyEventCallbacks();
            callBacks.KickedFromLobby += CallBacks_KickedFromLobby;
            callBacks.LobbyDeleted += CallBacks_LobbyDeleted;
            lobbyEvents = await LobbyService.Instance.SubscribeToLobbyEventsAsync(currentLobby.Id, callBacks);

            PullLobby(currentLobby.Id);
            print($"Successfully joined Lobby: {currentLobby.Name} [{currentLobby.Id}]");

        } catch (LobbyServiceException e) when (e.Reason == LobbyExceptionReason.NoOpenLobbies) {
            // 2. If no available lobby exists, create one
            print("No lobbies found, creating one");
            await CreateLobby(gameMode);
        } catch (Exception e) {
            print($"Failed to join or create lobby: {e.Message}");
        }
    }

    private void CallBacks_LobbyDeleted() {
        currentLobby = null;
        MainMenuManager.Instance.LeaveLobby();
        UnsubscribeFromEvents();
    }

    private void CallBacks_KickedFromLobby() {
        currentLobby = null;
        MainMenuManager.Instance.LeaveLobby();
        UnsubscribeFromEvents();
    }

    private void UnsubscribeFromEvents() {
        if (lobbyEvents != null) {
            lobbyEvents.UnsubscribeAsync();
            lobbyEvents = null;
        }
    }

    private async Task CreateLobby(GameMode gameMode) {
        try {
            // set up a random lobby name
            string lobbyName = "Lobby_" + UnityEngine.Random.Range(1000, 10000);
            CreateLobbyOptions options = new CreateLobbyOptions {
                IsPrivate = false,
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject> {
                    {ConstantKeys.GameMode.ToString(), new DataObject(
                        DataObject.VisibilityOptions.Public,
                        gameMode.ToString(),
                        DataObject.IndexOptions.S1)
                    },
                    {ConstantKeys.RelayCode.ToString(), new DataObject(
                        DataObject.VisibilityOptions.Member,
                        "0")
                    }
                }
            };
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, MAX_Players, options);
            print($"Created Lobby: {lobby.Name} with ID: {lobby.Id}");
            currentLobby = lobby;
            StartCoroutine(HeartbeatLobbyCoroutine(lobby.Id, 15f));
            PullLobby(currentLobby.Id);
        } catch (Exception e) {
            print($"Failed to create lobby: {e.Message}");
        }
    }

    private Player GetPlayer() {
        return new Player {
            Data = new Dictionary<string, PlayerDataObject>
            {
                {ConstantKeys.PlayerName.ToString(), new PlayerDataObject(
                    PlayerDataObject.VisibilityOptions.Member,
                    playerName)
                },
                {ConstantKeys.isReady.ToString(), new PlayerDataObject(
                    PlayerDataObject.VisibilityOptions.Member,
                    "false")
                }
            }
        };
    }
    public async void UpdatePlayerReadyStatus(bool isReady) {
        if (currentLobby == null) return;
        try {
            await LobbyService.Instance.UpdatePlayerAsync(currentLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions {
                Data = new Dictionary<string, PlayerDataObject> {
                    {ConstantKeys.isReady.ToString(), new PlayerDataObject(
                        PlayerDataObject.VisibilityOptions.Member,
                        isReady.ToString())
                    }
                }
            });
            print($"Updated player ready status to: {isReady}");
        } catch (Exception e) {
            print($"Failed to update player ready status: {e.Message}");
        }
    }

    private IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds) {
        var delay = new WaitForSecondsRealtime(waitTimeSeconds);
        while (currentLobby != null && lobbyId == currentLobby.Id) {
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }
    private async void PullLobby(string lobbyId) {
        while (currentLobby != null && lobbyId == currentLobby.Id) {
            try {
                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(lobbyId);
                currentLobby = lobby;
                OnLobbyPulled?.Invoke(lobby);
            } catch (LobbyServiceException e) {
                // If the lobby was deleted by host or player was kicked
                if (e.Reason == LobbyExceptionReason.LobbyNotFound) {
                    print("Lobby no longer exists.");
                    currentLobby = null;
                    break;
                }
            } catch (Exception e) {
                print($"Polling error: {e.Message}");
            }
            await Task.Delay(1500);
        }
    }

    public List<Player> GetPlayersInLobby() {
        if (currentLobby == null) return new List<Player>();
        return currentLobby.Players;
    }
    public bool isHost() {
        if (currentLobby == null) return false;
        return currentLobby.HostId == AuthenticationService.Instance.PlayerId;
    }
    public Lobby GetLobby() {
        return currentLobby;
    }
    public async void LeaveLobby() {
        if (currentLobby == null) return;
        string lobbyId = currentLobby.Id;
        currentLobby = null;
        StopAllCoroutines();
        try {
            await LobbyService.Instance.RemovePlayerAsync(lobbyId, AuthenticationService.Instance.PlayerId);
            print($"Left Lobby: {lobbyId}");
        } catch (Exception e) {
            print($"Failed to leave lobby: {e.Message}");
        }
    }
    public async void KickPlayer(string playerId) {
        if (currentLobby == null) return;
        try {
            await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, playerId);
            print($"Kicked player with ID: {playerId}");
        } catch (Exception e) {
            print($"Failed to kick player: {e.Message}");
        }
    }
    public async void destroyLobby() {
        if (currentLobby == null) return;
        StopAllCoroutines();
        string lobbyId = currentLobby.Id;
        currentLobby = null;
        await LobbyService.Instance.DeleteLobbyAsync(lobbyId);
    }

}

public enum ConstantKeys {
    PlayerName,
    GameMode,
    RelayCode,
    isReady,
    Volume
}