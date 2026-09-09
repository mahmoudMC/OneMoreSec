using UnityEngine;

public class PersistantDataManager : MonoBehaviour
{
    public static PersistantDataManager Instance {get; private set; }

    private string LobbyID;
    private byte LobbyPlayers = 0;
    private string RelayCode;
    private bool isHost = false;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
    public void SetHostData(string lobbyid, byte playersCount) {
        LobbyID = lobbyid;
        LobbyPlayers = playersCount;
        isHost = true;
    }
    public void SetClientData(string relayCode) {
        RelayCode = relayCode;
        isHost = false;
    }

    public bool IsHost() {
        return isHost;
    }
    public (string, byte) GetHostData() {
        return (LobbyID, LobbyPlayers);
    }
    public string GetClientData() {
        return RelayCode;
    }
}
