using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ImageGallery.Client.ViewModels;
using Newtonsoft.Json;
using ImageGallery.Model;
using System.Net.Http;
using System.IO;
using IdentityModel.Client;
using ImageGallery.Client.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace ImageGallery.Client.Controllers
{
	[Authorize]
	public class GalleryController : Controller
    {
        private readonly IImageGalleryHttpClient _imageGalleryHttpClient;

        public GalleryController(IImageGalleryHttpClient imageGalleryHttpClient)
        {
            _imageGalleryHttpClient = imageGalleryHttpClient;
        }

        public async Task<IActionResult> Index()
        {

	        await WriteOutIdentityInformation();

			// call the API
			var httpClient = await _imageGalleryHttpClient.GetClient(); 

            var response = await httpClient.GetAsync("api/images").ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var imagesAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                var galleryIndexViewModel = new GalleryIndexViewModel(
                    JsonConvert.DeserializeObject<IList<Image>>(imagesAsString).ToList());

                return View(galleryIndexViewModel);
            }

			if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
	            response.StatusCode == System.Net.HttpStatusCode.Forbidden)
	        {
		        return RedirectToAction("AccessDenied", "Authorization");
	        }


	        throw new Exception($"A problem happened while calling the API: {response.ReasonPhrase}");
        }

        public async Task<IActionResult> EditImage(Guid id)
        {
            // call the API
            var httpClient = await _imageGalleryHttpClient.GetClient();

            var response = await httpClient.GetAsync($"api/images/{id}").ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var imageAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var deserializedImage = JsonConvert.DeserializeObject<Image>(imageAsString);

                var editImageViewModel = new EditImageViewModel()
                {
                    Id = deserializedImage.Id,
                    Title = deserializedImage.Title
                };
                
                return View(editImageViewModel);
            }

	        if (response.StatusCode == System.Net.HttpStatusCode.Forbidden ||
	            response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
	        {
		        return RedirectToAction("AccessDenied", "Authorization");
	        }
           
            throw new Exception($"A problem happened while calling the API: {response.ReasonPhrase}");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditImage(EditImageViewModel editImageViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // create an ImageForUpdate instance
            var imageForUpdate = new ImageForUpdate()
                { Title = editImageViewModel.Title };

            // serialize it
            var serializedImageForUpdate = JsonConvert.SerializeObject(imageForUpdate);

            // call the API
            var httpClient = await _imageGalleryHttpClient.GetClient();

            var response = await httpClient.PutAsync(
                $"api/images/{editImageViewModel.Id}",
                new StringContent(serializedImageForUpdate, System.Text.Encoding.Unicode, "application/json"))
                .ConfigureAwait(false);                        

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

	        if (response.StatusCode == System.Net.HttpStatusCode.Forbidden ||
	            response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
	        {
		        return RedirectToAction("AccessDenied", "Authorization");
	        }

			throw new Exception($"A problem happened while calling the API: {response.ReasonPhrase}");
        }

        public async Task<IActionResult> DeleteImage(Guid id)
        {
            // call the API
            var httpClient = await _imageGalleryHttpClient.GetClient();

            var response = await httpClient.DeleteAsync($"api/images/{id}").ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

	        if (response.StatusCode == System.Net.HttpStatusCode.Forbidden ||
	            response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
	        {
		        return RedirectToAction("AccessDenied", "Authorization");
	        }

			throw new Exception($"A problem happened while calling the API: {response.ReasonPhrase}");
        }

	    [Authorize(Roles = "PayingUser")]
		public IActionResult AddImage()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(Roles = "PayingUser")]
        public async Task<IActionResult> AddImage(AddImageViewModel addImageViewModel)
        {   
            if (!ModelState.IsValid)
            {
                return View();
            }

            // create an ImageForCreation instance
            var imageForCreation = new ImageForCreation()
                { Title = addImageViewModel.Title };

            // take the first (only) file in the Files list
            var imageFile = addImageViewModel.Files.First();

            if (imageFile.Length > 0)
            {
                using (var fileStream = imageFile.OpenReadStream())
                using (var ms = new MemoryStream())
                {
                    fileStream.CopyTo(ms);
                    imageForCreation.Bytes = ms.ToArray();                     
                }
            }
            
            // serialize it
            var serializedImageForCreation = JsonConvert.SerializeObject(imageForCreation);

            // call the API
            var httpClient = await _imageGalleryHttpClient.GetClient();

            var response = await httpClient.PostAsync(
                $"api/images",
                new StringContent(serializedImageForCreation, System.Text.Encoding.Unicode, "application/json"))
                .ConfigureAwait(false);


	        if (response.StatusCode == System.Net.HttpStatusCode.Forbidden ||
	            response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
	        {
		        return RedirectToAction("AccessDenied", "Authorization");
	        }


			if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }



            throw new Exception($"A problem happened while calling the API: {response.ReasonPhrase}");
        }

	    [Authorize(Roles = "PayingUser")]
	    public async Task<IActionResult> OrderFrame()
	    {
		    var discoveryClient = new DiscoveryClient("https://localhost:44393/");
		    var metaDataResponse = await discoveryClient.GetAsync();
			var userInfoClient = new UserInfoClient(metaDataResponse.UserInfoEndpoint);
		    await WriteOutAccessTokenInfo();
			var accessToken =
			    await HttpContext.Authentication.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
			var userInfoResponse = await userInfoClient.GetAsync(accessToken);

		    if (userInfoResponse.IsError)
		    {
			    throw new Exception(
					"Problem accessing the UserInfo endpoint.", userInfoResponse.Exception);
		    }

		    var address = userInfoResponse.Claims.FirstOrDefault(c => c.Type == "address")?.Value;
			//Convert.ToString(accessToken.Principal.Claims.FirstOrDefault(c => c.Type == "address"))

			return View(new OrderFrameViewModel(address));
	    }

	    private async Task WriteOutIdentityInformation()
	    {
		    var identityToken = await HttpContext.Authentication.GetTokenAsync(OpenIdConnectParameterNames.IdToken);

			Debug.WriteLine($"Identity token: {identityToken}");

		    foreach (var userClaim in User.Claims)
		    {
			    Debug.WriteLine($"Claim type: {userClaim.Type} - Claim value: {userClaim.Value}");
		    }
		}

	    private async Task WriteOutAccessTokenInfo()
	    {
		    var accessToken = await HttpContext.Authentication.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

		    Debug.WriteLine($"Access token: {accessToken}");
	    }

		public async Task Logout()
	    {
		    await HttpContext.Authentication.SignOutAsync("Cookies");
		    await HttpContext.Authentication.SignOutAsync("oidc");
		}
    }
}
