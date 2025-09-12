namespace CleaningSuppliesSystem.WebUI.Services.TokenServices
{
    public interface ITokenService
    {
        string GetUserToken { get; }
        int GetUserId { get; }
        string GetUserRole { get; }
        string GetUserNameSurname { get; }
        string GetUserTheme { get; }
        void UpdateToken(string newToken);
        void ClearToken();
    }
}