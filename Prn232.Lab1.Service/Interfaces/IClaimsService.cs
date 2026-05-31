namespace Prn232.Lab1.Service.Interfaces;

public interface IClaimsService
{
    public Guid GetCurrentUserId { get; }

    public string? IpAddress { get; }
}

