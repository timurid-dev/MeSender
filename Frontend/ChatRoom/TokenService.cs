using Blazored.LocalStorage;

namespace ChatRoom;

internal sealed class TokenService(ILocalStorageService localStorage)
{
    private const string AccessTokenKey = "access_token";
    private const string RefreshTokenKey = "refresh_token";

    public async Task<string?> GetAccessTokenAsync() => await localStorage.GetItemAsync<string>(AccessTokenKey);

    public async Task<string?> GetRefreshTokenAsync() => await localStorage.GetItemAsync<string>(RefreshTokenKey);

    public async Task SetTokensAsync(string accessToken, string refreshToken)
    {
        await localStorage.SetItemAsync(AccessTokenKey, accessToken);
        await localStorage.SetItemAsync(RefreshTokenKey, refreshToken);
    }

    public async Task ClearTokensAsync()
    {
        await localStorage.RemoveItemAsync(AccessTokenKey);
        await localStorage.RemoveItemAsync(RefreshTokenKey);
    }
}
