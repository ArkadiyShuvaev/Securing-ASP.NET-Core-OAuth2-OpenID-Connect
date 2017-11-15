using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shuvaev.IDP.Entities
{
	[Table("UserClaims")]
	public class UserClaim
    {
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		//public int UserId { get; set; }

		[Required]
		[MaxLength(250)]
		public string ClaimType { get; set; }
	    
	    [Required]
	    [MaxLength(250)]
		public string ClaimValue { get; set; }

		//[ForeignKey(nameof(UserId))]
		//public virtual User User { get; set; }
    }
}
