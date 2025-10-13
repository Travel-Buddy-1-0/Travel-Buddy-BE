using System;

namespace BusinessObject.Entities
{
    public partial class FeedbackHotel
    {
        public int FeedbackId { get; set; }
        public int UserId { get; set; }
        public int HotelId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual User? User { get; set; }
        public virtual Hotel? Hotel { get; set; }
    }
}
