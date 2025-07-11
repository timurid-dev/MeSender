using System.Net.Http.Headers;
using System.Net.Http.Json;
using ChatRoom.Models;
using Microsoft.AspNetCore.Components;

namespace ChatRoom;

internal sealed class AuthHandler(TokenService tokenService, IHttpClientFactory httpClientFactory)
    : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var accessToken = await tokenService.GetAccessTokenAsync();
        if (!string.IsNullOrEmpty(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        var response = await base.SendAsync(request, cancellationToken);
        if (response.StatusCode != System.Net.HttpStatusCode.Unauthorized)
        {
            return response;
        }

        var refreshToken = await tokenService.GetRefreshTokenAsync();
        if (string.IsNullOrEmpty(refreshToken))
        {
            return await base.SendAsync(request, cancellationToken);
        }

        var refreshClient = httpClientFactory.CreateClient("IdentityApi");
        var refreshResponse = await refreshClient.PostAsJsonAsync(
            "api/refresh",
            new
            {
                RefreshToken = refreshToken,
            },
            cancellationToken);

        if (!refreshResponse.IsSuccessStatusCode)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        var tokens = await refreshResponse.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken: cancellationToken);
        if (tokens is null)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        await tokenService.SetTokensAsync(tokens.AccessToken, tokens.RefreshToken);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);
        return await base.SendAsync(request, cancellationToken);
    }
}
