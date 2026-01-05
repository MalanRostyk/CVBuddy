using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CVBuddy.Models
{
    public class ProjectUser
    {
        [Key]
        public int PUId { get; set; }

        public int ProjId { get; set; }
        [ForeignKey(nameof(ProjId))]
        public Project Project { get; set; }


        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        public bool IsOwner { get; set; } = false;
    }
}
