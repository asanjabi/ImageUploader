using System;
using System.Threading;
using System.Threading.Tasks;

using Azure.Core;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

using AccessToken = Azure.Core.AccessToken;

namespace ImageUploader
{
    public class AccessTokenProviderTokenCredential : TokenCredential
    {
        private readonly IAccessTokenProvider _accessTokenProvider;
        private readonly NavigationManager _navigationManager;

        public AccessTokenProviderTokenCredential(IAccessTokenProvider accessTokenProvider, NavigationManager navigationManager)
        {
            _accessTokenProvider = accessTokenProvider;
            _navigationManager = navigationManager;
        }

        public override async ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            var result = await _accessTokenProvider.RequestAccessToken(new AccessTokenRequestOptions
            {
                Scopes = requestContext.Scopes
            });

            if (result.Status == AccessTokenResultStatus.RequiresRedirect)
            {
                _navigationManager.NavigateTo(result.InteractiveRequestUrl);
            }

            if (result.TryGetToken(out var accessToken))
            {
                return new AccessToken(accessToken.Value, accessToken.Expires);
            }
            throw new Exception("Couldn't get the access token");
        }

        public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}