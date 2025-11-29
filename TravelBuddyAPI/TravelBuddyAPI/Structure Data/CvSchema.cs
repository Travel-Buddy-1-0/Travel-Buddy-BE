//using Mscc.GenerativeAI;

//namespace TravelBuddyAPI.Models
//{
//    public Schema CreateCvSchema()
//    {
//        return new Schema
//        {
//            Type = ParameterType.Object,
//            // Key cha ngoài cùng là "data_json"
//            Properties = new Dictionary<string, Schema>
//        {
//            { "data_json", new Schema
//                {
//                    Type = ParameterType.Object,
//                    Properties = new Dictionary<string, Schema>
//                    {
//                        // 1. PROFILE
//                        { "profile", new Schema
//                            {
//                                Type = ParameterType.Object,
//                                Properties = new Dictionary<string, Schema>
//                                {
//                                    { "avatar", new Schema { Type = ParameterType.String, Description = "URL ảnh nếu tìm thấy trong text, nếu không hãy để string rỗng" } },
//                                    { "name", new Schema { Type = ParameterType.String, Description = "Họ tên đầy đủ" } },
//                                    { "email", new Schema { Type = ParameterType.String } },
//                                    { "phone", new Schema { Type = ParameterType.String } }
//                                },
//                                Required = new List<string> { "name", "email" }
//                            }
//                        },

//                        // 2. SUMMARY
//                        { "summary", new Schema
//                            {
//                                Type = ParameterType.String,
//                                Description = "Đoạn tóm tắt tối ưu về ứng viên"
//                            }
//                        },

//                        // 3. EXPERIENCE
//                        { "experience", new Schema
//                            {
//                                Type = ParameterType.Array,
//                                Items = new Schema
//                                {
//                                    Type = ParameterType.Object,
//                                    Properties = new Dictionary<string, Schema>
//                                    {
//                                        { "position", new Schema { Type = ParameterType.String } },
//                                        { "company", new Schema { Type = ParameterType.String } },
//                                        { "time", new Schema { Type = ParameterType.String, Description = "VD: 2020 - Present" } },
//                                        { "desc", new Schema { Type = ParameterType.String, Description = "Mô tả chi tiết công việc" } }
//                                    }
//                                }
//                            }
//                        },

//                        // 4. PROJECTS
//                        { "projects", new Schema
//                            {
//                                Type = ParameterType.Array,
//                                Items = new Schema
//                                {
//                                    Type = ParameterType.Object,
//                                    Properties = new Dictionary<string, Schema>
//                                    {
//                                        { "name", new Schema { Type = ParameterType.String } },
//                                        { "tech", new Schema
//                                            {
//                                                Type = ParameterType.Array,
//                                                Items = new Schema { Type = ParameterType.String, Description = "Công nghệ sử dụng" }
//                                            }
//                                        },
//                                        { "desc", new Schema { Type = ParameterType.String } }
//                                    }
//                                }
//                            }
//                        },

//                        // 5. EDUCATION
//                        { "education", new Schema
//                            {
//                                Type = ParameterType.Array,
//                                Items = new Schema
//                                {
//                                    Type = ParameterType.Object,
//                                    Properties = new Dictionary<string, Schema>
//                                    {
//                                        { "school", new Schema { Type = ParameterType.String } },
//                                        { "year", new Schema { Type = ParameterType.String } },
//                                        { "degree", new Schema { Type = ParameterType.String } }
//                                    }
//                                }
//                            }
//                        },

//                        // 6. SKILLS, LANGUAGES, INTERESTS
//                        { "skills", new Schema
//                            {
//                                Type = ParameterType.Array,
//                                Items = new Schema { Type = ParameterType.String }
//                            }
//                        },
//                        { "languages", new Schema
//                            {
//                                Type = ParameterType.Array,
//                                Items = new Schema { Type = ParameterType.String }
//                            }
//                        },
//                        { "interests", new Schema
//                            {
//                                Type = ParameterType.Array,
//                                Items = new Schema { Type = ParameterType.String }
//                            }
//                        }
//                    },
//                    // Các trường bắt buộc phải có bên trong data_json
//                    Required = new List<string> { "profile", "experience", "education", "skills" }
//                }
//            }
//        },
//            Required = new List<string> { "data_json" }
//        };
//    }
//}
