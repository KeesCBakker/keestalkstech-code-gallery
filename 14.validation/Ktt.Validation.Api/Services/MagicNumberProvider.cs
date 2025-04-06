namespace Ktt.Validation.Api.Services;

public class MagicNumberProvider : IMagicNumberProvider
{
    public Task<int> GetMagicNumber()
    {
        return Task.FromResult(42);
    }
}
