using Microsoft.JSInterop;

namespace ChatRoom;

internal sealed class TokenService(IJSRuntime js)
{
    private const string AccessTokenKey = "access_token";
    private const string RefreshTokenKey = "refresh_token";

    public async Task<string?> GetAccessTokenAsync() => await js.InvokeAsync<string>("localStorage.getItem", AccessTokenKey);

    public async Task<string?> GetRefreshTokenAsync() => await js.InvokeAsync<string>("localStorage.getItem", RefreshTokenKey);

    public async Task SetTokensAsync(string accessToken, string refreshToken)
    {
        await js.InvokeVoidAsync("localStorage.setItem", AccessTokenKey, accessToken);
        await js.InvokeVoidAsync("localStorage.setItem", RefreshTokenKey, refreshToken);
    }

    public async Task ClearTokensAsync()
    {
        await js.InvokeVoidAsync("localStorage.removeItem", AccessTokenKey);
        await js.InvokeVoidAsync("localStorage.removeItem", RefreshTokenKey);
    }
}
