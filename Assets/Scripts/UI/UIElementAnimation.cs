using DG.Tweening;
using UnityEngine;

public class UIElementAnimation : MonoBehaviour
{
    private enum AnimationType {
        None,
        SlidingIn
    }
    [SerializeField] private AnimationType animationType;
    [SerializeField] private float animationDuration = 1f;
    [SerializeField] private float animationDelay = 0f;
    [SerializeField] private Ease animationEase = Ease.Linear;
    [Header("Sliding In Animation Settings")]
    [SerializeField] private Vector2 startPositionOffset;

    private void OnEnable() {
        switch (animationType) {
            case AnimationType.SlidingIn: SlidingIn(); break;
            default: break;
        }
    }

    private void SlidingIn() {
        RectTransform rect = GetComponent<RectTransform>();
        rect.DOAnchorPos(rect.anchoredPosition, animationDuration)
            .From(rect.anchoredPosition + startPositionOffset)
            .SetEase(animationEase)
            .SetDelay(animationDelay);
    }
}
