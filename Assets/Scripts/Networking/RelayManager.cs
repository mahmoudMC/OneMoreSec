using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayManager : MonoBehaviour
{
    public static RelayManager Instance { get; private set; }

    private void Awake() {
        if (Instance == null) Instance = this;
        else {
            Destroy(gameObject);
            return;
        }
    }
    private void Start() {
        if (PersistantDataManager.Instance.IsHost()) {
            // create a relay
            CreateRelay();
        } else {
            // join a relay
            JoinRelay();
        }
    }

    private async void CreateRelay() {
        try {
            UnityTransport utp = NetworkManager.Singleton.GetComponent<UnityTransport>();
            byte playersCount;
            string lobbyID;
            (lobbyID, playersCount) = PersistantDataManager.Instance.GetHostData();

            // host doesn't count. So relay can have a maximum of n-1 players
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(playersCount - 1);

            string relayCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            await SetUpRelayCode(lobbyID, relayCode);

            RelayServerData relayServerData = AllocationUtils.ToRelayServerData(allocation, "dtls");

            utp.SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();

        } catch (Exception ex) {
            print($"Error creating a relay: {ex.Message}");
        }
        
    }
    private async void JoinRelay() {
        try {
            UnityTransport utp = NetworkManager.Singleton.GetComponent<UnityTransport>();
            string RelayCode = PersistantDataManager.Instance.GetClientData();

            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(RelayCode);

            RelayServerData relayServerData = AllocationUtils.ToRelayServerData(allocation, "dtls");

            utp.SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
        } catch (Exception ex) {
            print($"Error joining a relay: {ex.Message}");
        }
        
    }

    private async Task SetUpRelayCode(string lobbyId, string relayCode) {
        UpdateLobbyOptions options = new UpdateLobbyOptions() {
            IsPrivate = true,
            Data = new Dictionary<string, DataObject> {
                {ConstantKeys.RelayCode.ToString(), new DataObject(
                    DataObject.VisibilityOptions.Member,
                    relayCode)
                }
            }
        };

        await LobbyService.Instance.UpdateLobbyAsync(lobbyId, options);
    }
}
