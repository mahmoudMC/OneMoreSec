using System;

public static class RechargeEvents
{
    public static event Action<IRechargeService> RechargeStarted;
    public static event Action<IRechargeService> RechargeCancelled;

    public static void RaiseRechargeStarted(IRechargeService source)
    {
        RechargeStarted?.Invoke(source);
    }

    public static void RaiseRechargeCancelled(IRechargeService source)
    {
        RechargeCancelled?.Invoke(source);
    }
}