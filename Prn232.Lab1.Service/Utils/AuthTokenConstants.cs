namespace Prn232.Lab1.Service.Utils;

public static class AuthTokenConstants
{
    public const int AccessTokenExpiresInSeconds = 3600;

    public static TimeSpan AccessTokenValidity =>
        TimeSpan.FromSeconds(AccessTokenExpiresInSeconds);
}
