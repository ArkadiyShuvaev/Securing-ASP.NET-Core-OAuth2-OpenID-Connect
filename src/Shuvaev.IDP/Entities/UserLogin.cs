using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shuvaev.IDP.Entities
{
	[Table("UserLogins")]
	public class UserLogin
    {
	    [Key]
	    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	    public int Id { get; set; }

	    //public int UserId { get; set; }

		[Required]
		[MaxLength(250)]
		public string LoginProvider { get; set; }

		[Required]
		[MaxLength(250)]
		public string ProviderKey { get; set; }

		//[ForeignKey(nameof (UserId))]
	 //   public virtual User User { get; set; }
    }
}
