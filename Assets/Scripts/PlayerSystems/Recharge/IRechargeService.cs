public interface IRechargeService
{
    bool IsRecharging { get; }

    void StartRecharge();
    void CancelRecharge();
}