using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UpgradeHUD : MonoBehaviour
{
    [Header("Speed Boost")]
    [SerializeField] private SpeedBoost speedBoost;
    [SerializeField] private Image speedCooldownFill;
    [SerializeField] private TMP_Text speedTimeText;

    [Header("Damage Boost")]
    [SerializeField] private DamageBoost damageBoost;
    [SerializeField] private Image damageCooldownFill;
    [SerializeField] private TMP_Text damageTimeText;

    [Header("Player Scan")]
    [SerializeField] private PlayerScan playerScan;
    [SerializeField] private Image scanCooldownFill;
    [SerializeField] private TMP_Text scanTimeText;

    private Coroutine speedCoroutine;
    private Coroutine damageCoroutine;
    private Coroutine scanCoroutine;

    private void Awake()
    {
        ResetSlot(
            speedCooldownFill,
            speedTimeText,
            speedBoost != null ? speedBoost.Duration : 0f
        );

        ResetSlot(
            damageCooldownFill,
            damageTimeText,
            damageBoost != null ? damageBoost.Duration : 0f
        );

        ResetSlot(
            scanCooldownFill,
            scanTimeText,
            playerScan != null ? playerScan.Duration : 0f
        );
    }

    private void OnEnable()
    {
        UpgradeEvents.UpgradeActivated += OnUpgradeActivated;
    }

    private void OnDisable()
    {
        UpgradeEvents.UpgradeActivated -= OnUpgradeActivated;
    }

    private void OnUpgradeActivated(IUpgrade upgrade)
    {
        if (upgrade == speedBoost)
        {
            if (speedCoroutine != null)
                StopCoroutine(speedCoroutine);

            speedCoroutine = StartCoroutine(
                RunTimer(
                    speedBoost.Duration,
                    speedCooldownFill,
                    speedTimeText
                )
            );

            return;
        }

        if (upgrade == damageBoost)
        {
            if (damageCoroutine != null)
                StopCoroutine(damageCoroutine);

            damageCoroutine = StartCoroutine(
                RunTimer(
                    damageBoost.Duration,
                    damageCooldownFill,
                    damageTimeText
                )
            );

            return;
        }

        if (upgrade == playerScan)
        {
            if (scanCoroutine != null)
                StopCoroutine(scanCoroutine);

            scanCoroutine = StartCoroutine(
                RunTimer(
                    playerScan.Duration,
                    scanCooldownFill,
                    scanTimeText
                )
            );
        }
    }

    private IEnumerator RunTimer(
        float duration,
        Image cooldownFill,
        TMP_Text timeText)
    {
        float remainingTime = duration;

        while (remainingTime > 0f)
        {
            if (cooldownFill != null)
            {
                cooldownFill.fillAmount =
                    remainingTime / duration;
            }

            if (timeText != null)
            {
                timeText.text =
                    $"{Mathf.CeilToInt(remainingTime)}s";
            }

            remainingTime -= Time.deltaTime;

            yield return null;
        }

        if (cooldownFill != null)
            cooldownFill.fillAmount = 0f;

        if (timeText != null)
            timeText.text = $"{Mathf.CeilToInt(duration)}s";
    }

    private void ResetSlot(
        Image cooldownFill,
        TMP_Text timeText,
        float duration)
    {
        if (cooldownFill != null)
            cooldownFill.fillAmount = 0f;

        if (timeText != null)
            timeText.text = $"{Mathf.CeilToInt(duration)}s";
    }
}