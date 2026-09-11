using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager Instance {  get; private set; }

    [SerializeField] private Image background;
    [SerializeField] private Transform loadingIcon;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            gameObject.SetActive(false);
        } else {
            Destroy(gameObject);
        }
    }

    private void OnEnable() {
        loadingIcon.DORotate(new Vector3(0, 0, -360), 1.5f, RotateMode.FastBeyond360)
            .From(Vector3.zero)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
    }
    private void OnDisable() {
        loadingIcon.DOKill();
    }

    public void StartLoading() {
        gameObject.SetActive(true);
        background.DOFade(0.4f, 0.1f);
    }
    public void FinishLoading() {
        gameObject.SetActive(false);
    }
    public async void LoadScene(int index) {
        ShowLoadingIconForScene();
        await SceneManager.LoadSceneAsync(index);
        HideLoadingIconForScene();
    }
    public async void LoadScene(string name) {
        ShowLoadingIconForScene();
        await SceneManager.LoadSceneAsync(name);
        HideLoadingIconForScene();
    }
    private void ShowLoadingIconForScene() {
        gameObject.SetActive(true);
        background.DOFade(1, 0.2f);
    }
    private void HideLoadingIconForScene() {
        background.DOFade(0, 0.5f).OnComplete(() => {
            gameObject.SetActive(false);
        });
    }
}
