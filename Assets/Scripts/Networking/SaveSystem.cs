using System;
using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance {  get; private set; }

    private string path;

    private PlayerData playerData;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            path = Path.Combine(Application.persistentDataPath, "playerData.json");
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        ReadData();
    }

    private void SaveData() {
        string json = JsonUtility.ToJson(playerData);
        File.WriteAllText(path, json);
    }
    private void ReadData() {
        if (File.Exists(path)) {
            playerData = JsonUtility.FromJson<PlayerData>(File.ReadAllText(path));
        } else {
            playerData = new PlayerData();
            SaveData();
        }
    }

    public void OnPlayerKilled() {
        playerData.kills++;
        SaveData();
    }
    public void OnPlayerDeath() {
        playerData.deaths++;
        SaveData();
    }
    public void OnPlayerJoinsGame() {
        playerData.gamesPlayed++;
        SaveData();
    }
    public void OnPlayerWin() {
        playerData.gameWon++;
        SaveData();
    }

    public PlayerData GetPlayerData() {
        return playerData;
    }
}

[Serializable]
public struct PlayerData {
    public int kills;
    public int deaths;
    public int gamesPlayed;
    public int gameWon;
}
