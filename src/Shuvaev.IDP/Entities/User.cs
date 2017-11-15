using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shuvaev.IDP.Entities
{
	public class User
    {
		[Key]
		public int Id { get; set; }

		public Guid SubjectId { get; set; }

		[Required]
		[MaxLength(100)]
		public string Username { get; set; }

	    [Required]
		[MaxLength(100)]
		public string Password { get; set; }

	    public bool IsActive { get; set; }

	    public virtual ICollection<UserClaim> Claims { get; set; } = new List<UserClaim>();
	    public virtual ICollection<UserLogin> Logins { get; set; } = new List<UserLogin>();

	}
}
