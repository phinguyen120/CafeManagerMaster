using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace G11_Coffee.Models
{
    public class UserCafe
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int CafeId { get; set; }
        public Cafe Cafe { get; set; }
    }
}
