using Microsoft.AspNetCore.Http;
using Prn232.Lab1.Repositories.Interfaces;
using Prn232.Lab1.Service.Interfaces;
using Prn232.Lab1.Service.Utils;
using System.Security.Claims;

namespace Prn232.Lab1.Service.Service;

public class ClaimsService : IClaimsService
{
    private readonly IUnitOfWork _unitOfWork;

    public ClaimsService(IHttpContextAccessor httpContextAccessor)
    {
        // Lấy ClaimsIdentity
        var identity = httpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity;

        var extractedId = AuthenTools.GetCurrentUserId(identity);
        if (Guid.TryParse(extractedId, out var parsedId))
            GetCurrentUserId = parsedId;
        else
            GetCurrentUserId = Guid.Empty;

        IpAddress = httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString();
    }

    public Guid GetCurrentUserId { get; }

    public string? IpAddress { get; }
}