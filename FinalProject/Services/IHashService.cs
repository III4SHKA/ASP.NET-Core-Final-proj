namespace FinalProject.Services;

public interface IHashService
{
    string GenerateSalt();
    string BuildHash(string password, string salt);
    bool Verify(string password, string salt, string savedHash);
}
