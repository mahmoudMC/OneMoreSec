using System;

public static class UpgradeEvents
{
    public static event Action<IUpgrade> UpgradeActivated;

    public static event Action<IUpgrade, UpgradeFailureReason>
        UpgradeActivationFailed;

    public static void RaiseUpgradeActivated(IUpgrade source)
    {
        UpgradeActivated?.Invoke(source);
    }

    public static void RaiseUpgradeActivationFailed(
        IUpgrade source,
        UpgradeFailureReason reason)
    {
        UpgradeActivationFailed?.Invoke(source, reason);
    }
}