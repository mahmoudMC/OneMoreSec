public interface IUpgrade
{
    string UpgradeName { get; }
    float Cost { get; }

    bool CanActivate();
    bool TryActivate();
}