using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Shuvaev.IDP.Controllers.UserRegistration
{
	public class RegisterUserViewModel
	{
		[MaxLength(100)]
		public string UserName { get; set; }

		[MaxLength(100)]
		public string Password { get; set; }

		[Required]
		[MaxLength(100)]
		public string FirstName { get; set; }


		[Required]
		[MaxLength(100)]
		public string LastName { get; set; }

		[Required]
		[MaxLength(150)]
		public string Email { get; set; }

		[Required]
		[MaxLength(200)]
		public string Address { get; set; }

		[Required]
		[MaxLength(2)]
		public string Country { get; set; }

		public SelectList CountryCodes { get; set; } = 
			new SelectList(
				new []
				{
					new {Id = "BE", Value = "Belgium"},
					new {Id = "RU", Value = "Russia"},
					new {Id = "US", Value = "United States of America"},
					new {Id = "IN", Value = "India"}
				},
				"Id", "Value");


		public string ReturnUrl { get; set; }

		public string Provider { get; set; }

		public string ProviderUserId { get; set; }

		public bool IsProvisioningFromExternal => Provider != null && Provider.Any();
	}
}