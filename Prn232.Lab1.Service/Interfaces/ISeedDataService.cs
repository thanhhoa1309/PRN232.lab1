namespace Prn232.Lab1.Service.Interfaces;

public interface ISeedDataService
{
    Task<object> SeedAsync();
    Task ClearAsync();
    Task EnsureSeedAsync();
}
