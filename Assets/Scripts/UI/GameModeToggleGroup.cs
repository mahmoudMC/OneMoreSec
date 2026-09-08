using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameModeToggleGroup : MonoBehaviour
{
    [SerializeField] private Toggle ft1Toggle;
    [SerializeField] private Toggle ft3Toggle;
    [SerializeField] private Toggle ft5Toggle;
    private GameMode selectedGameMode;

    private void Awake() {
        ft1Toggle.onValueChanged.AddListener(isOn => {
            ft1Toggle.GetComponent<Image>().DOFade(isOn ? 1f : 0f, 0.2f);
            if (isOn) selectedGameMode = GameMode.FT1;
        });

        ft3Toggle.onValueChanged.AddListener(isOn => {
            ft3Toggle.GetComponent<Image>().DOFade(isOn ? 1f : 0f, 0.2f);
            if (isOn) selectedGameMode = GameMode.FT3;
        });

        ft5Toggle.onValueChanged.AddListener(isOn => {
            ft5Toggle.GetComponent<Image>().DOFade(isOn ? 1f : 0f, 0.2f);
            if (isOn) selectedGameMode = GameMode.FT5;
        });
    }

    public GameMode GetSelectedGameMode() {
        return selectedGameMode;
    }
}
