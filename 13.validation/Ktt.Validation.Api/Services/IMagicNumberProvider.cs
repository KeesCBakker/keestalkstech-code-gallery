namespace Ktt.Validation.Api.Services;

public interface IMagicNumberProvider
{
    Task<int> GetMagicNumber();
}
