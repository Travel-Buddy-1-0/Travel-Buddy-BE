using System;
using System.Text.Json.Serialization;

namespace BusinessObject.DTOs
{
    // Lớp đại diện cho toàn bộ phần "data" bên trong payload
    public class WebhookDataDetail
    {
        // Sử dụng thuộc tính của JSON
        public long orderCode { get; set; }
        public int amount { get; set; }
        public string description { get; set; }
        public string accountNumber { get; set; }
        public string reference { get; set; }

        // Lưu ý: PayOS gửi transactionDateTime dưới dạng chuỗi "yyyy-MM-dd HH:mm:ss"
        public string transactionDateTime { get; set; } // Giữ là string để dễ deserialize

        public string currency { get; set; }
        public string paymentLinkId { get; set; }

        // Đây là code kết quả của giao dịch con (trong data)
        public string code { get; set; }
        public string desc { get; set; }

        // Thêm trường status nếu bạn cần. Dù không có trong JSON mẫu, 
        // nhiều hệ thống PayOS có thể gửi trường này. Nếu không, 
        // bạn phải tự ánh xạ dựa trên trường 'code' (ví dụ: code="00" là PAID).
        // public string status { get; set; } 
    }

    // Lớp đại diện cho toàn bộ Webhook Payload
    public class PayOsWebhookPayload
    {
        // Các trường ở cấp cao nhất
        public string code { get; set; }
        public string desc { get; set; }
        public bool success { get; set; }

        // Trường quan trọng nhất: Chứa các chi tiết giao dịch
        public WebhookDataDetail data { get; set; }

        // Chữ ký để xác thực nguồn gốc
        public string signature { get; set; }

        // ⚠️ Lưu ý: Trong code PaymentController của bạn, bạn đã dùng:
        // var orderCode = payload.orderCode;
        // Dòng này sẽ bị lỗi vì 'orderCode' nằm trong 'data'.
        // Sau khi sửa, bạn sẽ truy cập: var orderCode = payload.data.orderCode;
    }
}