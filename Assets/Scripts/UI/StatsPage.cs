using TMPro;
using UnityEngine;

public class StatsPage : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI killsText;
    [SerializeField] private TextMeshProUGUI deathsText;
    [SerializeField] private TextMeshProUGUI kdText;
    [SerializeField] private TextMeshProUGUI gamesText;
    [SerializeField] private TextMeshProUGUI winsText;
    [SerializeField] private TextMeshProUGUI winRateText;

    private void OnEnable() {
        if (killsText == null || deathsText == null || kdText == null ||
            gamesText == null || winsText == null || winRateText == null ||
            SaveSystem.Instance == null) return;

        PlayerData data = SaveSystem.Instance.GetPlayerData();
        killsText.SetText($"Kills: {data.kills}");
        deathsText.SetText($"Deaths: {data.deaths}");
        float kd = data.deaths == 0 ? data.kills : (float)data.kills / data.deaths;
        kdText.SetText($"K/D: {kd:F2}");

        gamesText.SetText($"Games Played: {data.gamesPlayed}");
        winsText.SetText($"Wins: {data.gameWon}");
        float winRate = data.gamesPlayed == 0 ? 0 : (float)data.gameWon / data.gamesPlayed;
        winRateText.SetText($"Win Rate: {winRate:P0}");
    }
}
