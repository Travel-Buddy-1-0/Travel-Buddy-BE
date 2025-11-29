using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Entities
{
    public class Favorite
    {
        public int FavoriteId { get; set; }
        public int UserId { get; set; }

        public string TargetType { get; set; } = string.Empty;//"HOTEL" "RESTAURANT" "POST"

        public string TargetId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual User User { get; set; }
    }
}
