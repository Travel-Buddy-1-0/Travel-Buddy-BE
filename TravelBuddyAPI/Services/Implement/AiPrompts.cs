namespace Services.Implement
{
    public static class AiPrompts
    {
        // 1. Prompt lấy Text thô (Input cho bước phân tích)
        //        public const string CvParserPrompt = @"
        //Bạn là công cụ OCR/Parser. 
        //Nhiệm vụ: Trích xuất *nguyên văn* nội dung text từ file PDF hoặc DOCX đính kèm.
        //Yêu cầu:
        //- Giữ nguyên thứ tự dòng nếu có thể.
        //- Không tóm tắt, không suy diễn, không dịch, không thêm nội dung.
        //- Không phân loại, không phân tích.
        //- Không xuất JSON.
        //- Không dùng markdown (không dùng ```).
        //- Chỉ trả về TEXT THUẦN.

        //Nếu có phần không đọc được, hãy bỏ qua.
        //Bắt đầu xuất text ngay lập tức:";

        // 2. Prompt Phân tích & Tối ưu (Output ra JSON chuẩn chỉ chứa data_json)
        public const string CvOptimizerPrompt = @"
Bạn là một chuyên gia nhân sự (HR Specialist) và Chuyên gia tối ưu hóa CV (Resume Optimizer).

NHIỆM VỤ:
1. Phân tích dữ liệu 'RAW CV DATA' (dữ liệu ứng viên) và 'JOB DESCRIPTION' (yêu cầu công việc).
2. Trích xuất thông tin từ CV và điền vào trường 'data_json'.
3. TỐI ƯU HÓA NỘI DUNG (Optimize Content):
   - Viết lại phần 'summary' để làm nổi bật sự phù hợp với JD.
   - Viết lại 'experience.desc' và 'projects.desc' để nhấn mạnh các kỹ năng/từ khóa có trong JD.
   - Sửa lỗi chính tả và ngữ pháp.
   - Giữ nguyên sự thật (tên công ty, thời gian, bằng cấp).

OUTPUT FORMAT:
Chỉ trả về chuỗi JSON hợp lệ. Không kèm markdown block (```json).

CẤU TRÚC JSON MỤC TIÊU:
{
  ""data_json"": {
    ""profile"": {
      ""avatar"": ""string (URL ảnh nếu tìm thấy trong text, nếu không hãy để string rỗng)"",
      ""name"": ""string (Họ tên đầy đủ)"",
      ""email"": ""string"",
      ""phone"": ""string""
    },
    ""summary"": ""string (Đoạn tóm tắt tối ưu)"",
    ""experience"": [
      {
        ""position"": ""string"",
        ""company"": ""string"",
        ""time"": ""string (VD: 2020 - Present)"",
        ""desc"": ""string (Mô tả chi tiết công việc)""
      }
    ],
    ""projects"": [
      {
        ""name"": ""string"",
        ""tech"": [""string (Công nghệ sử dụng)""],
        ""desc"": ""string""
      }
    ],
    ""education"": [
      {
        ""school"": ""string"",
        ""year"": ""string"",
        ""degree"": ""string""
      }
    ],
    ""skills"": [""string""],
    ""languages"": [""string""],
    ""interests"": [""string""]
  }
}";
    }
}