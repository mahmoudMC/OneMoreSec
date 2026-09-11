using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTemplate : MonoBehaviour
{
    [SerializeField] private Button kickbutton;
    [SerializeField] private GameObject readyIcon;
    [SerializeField] private TextMeshProUGUI playerNameText;

    public void InitializeTemplate(string playerName, bool isHost = false) {
        playerNameText.SetText(playerName);
        kickbutton.gameObject.SetActive(isHost);
        readyIcon.SetActive(false);
        gameObject.SetActive(true);
    }
    public void UpdateReadyStatus(bool isReady) {
        readyIcon.SetActive(isReady);
    }
    public void AddListenerForKick(UnityEngine.Events.UnityAction action) {
        kickbutton.onClick.AddListener(action);
    }
}
