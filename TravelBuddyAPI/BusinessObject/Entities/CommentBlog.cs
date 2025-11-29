using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Entities
{
    public partial class CommentBlog
    {
        public int CommentId { get; set; }
        public string BlogOnlineId { get; set; }

        public int? UserId { get; set; }  

        public string Content { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public int? ParentCommentId { get; set; } 


        public virtual User? User { get; set; }

        public virtual CommentBlog? ParentComment { get; set; } 

        public virtual ICollection<CommentBlog> Replies { get; set; } = new List<CommentBlog>(); 
    }
}
