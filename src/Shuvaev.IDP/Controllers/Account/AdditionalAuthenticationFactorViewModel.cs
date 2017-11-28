using System.ComponentModel.DataAnnotations;

namespace Shuvaev.IDP.Controllers.Account
{
	public class AdditionalAuthenticationFactorViewModel
	{
		[Required]
		public string Code { get; set; }
		public string ReturnUrl { get; set; }
		public bool RememberLogin { get; set; }
	}
}