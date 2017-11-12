using Microsoft.AspNetCore.Http;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace ImageGallery.Client.Services
{
    public class ImageGalleryHttpClient : IImageGalleryHttpClient
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient = new HttpClient();
	    private const string IdentityPointUri = "https://localhost:44393/";
	    private const string ImageApiPointUri = "https://localhost:44380/";

		public ImageGalleryHttpClient(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        
        public async Task<HttpClient> GetClient()
        {
	        var token = string.Empty;
	        var httpContext = _httpContextAccessor.HttpContext;
			//token = await httpContext.Authentication.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

	  //      var authentificateInfo = await currentContext.Authentication.GetAuthenticateInfoAsync(Consts.AuthenticationScheme);
			//authentificateInfo.Properties.UpdateTokenValue("expires_at", DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture));

			//var identificationToken = await currentContext.Authentication.GetAuthenticateInfoAsync(Consts.AuthenticationScheme);
			var expiresAtAsString = await httpContext.Authentication.GetTokenAsync("expires_at");

	        if (string.IsNullOrWhiteSpace(expiresAtAsString) || DateTime.Parse(expiresAtAsString).ToUniversalTime() < DateTime.UtcNow)
	        {
		        Debug.WriteLine($"Retriving new access token. Current 'expires_at' property: {expiresAtAsString}");
				token = await RenewTokens();
	        }
			else
	        {
		        token = await httpContext.Authentication.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
	        }

	        _httpClient.SetBearerToken(token);
	        Debug.WriteLine($"Access token: {token}");

			_httpClient.BaseAddress = new Uri(ImageApiPointUri);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
	        
            return _httpClient;
        }

	    private async Task<string> RenewTokens()
	    {
		    var currentContext = _httpContextAccessor.HttpContext;

			var discoveryClient = new DiscoveryClient(IdentityPointUri);
		    var metaDataResponse = await discoveryClient.GetAsync();

		    var tokenClient = new TokenClient(metaDataResponse.TokenEndpoint, "imagegalleryclient", "secret");

		    var currentRefreshToken = await currentContext.Authentication.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

		    var tokenResult = await tokenClient.RequestRefreshTokenAsync(currentRefreshToken);

		    if (tokenResult.IsError)
		    {
			    throw new Exception("Problem encountered while refreshing tokens.", tokenResult.Exception);
		    }


		    var authentificateInfo = await currentContext.Authentication.GetAuthenticateInfoAsync(Consts.AuthenticationScheme);

		    var expiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(tokenResult.ExpiresIn);
			authentificateInfo.Properties.UpdateTokenValue("expires_at", expiresAt.ToString("o", CultureInfo.InvariantCulture));

			authentificateInfo.Properties.UpdateTokenValue(OpenIdConnectParameterNames.RefreshToken,
			    tokenResult.RefreshToken);
		    authentificateInfo.Properties.UpdateTokenValue(OpenIdConnectParameterNames.AccessToken,
			    tokenResult.AccessToken);

		    await currentContext.Authentication.SignInAsync(Consts.AuthenticationScheme, authentificateInfo.Principal,
			    authentificateInfo.Properties);

		    return tokenResult.AccessToken;
	    }
    }
}

