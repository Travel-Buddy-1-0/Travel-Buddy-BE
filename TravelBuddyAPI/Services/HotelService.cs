using BusinessLogic.Exceptions;
using BusinessObject.Data;
using BusinessObject.DTOs;
using BusinessObject.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories;
using System.IO.Pipes;

namespace BusinessLogic.Services;

public class HotelService : IHotelService
{
    private readonly IHotelRepository _hotelRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPaymentHistoryRepository _paymentHistoryRepository;
    private readonly IVoucherRepository _voucherRepository;
    private readonly AppDbContext _dbContext;
    public HotelService(IHotelRepository hotelRepository, IUserRepository userRepository, IPaymentHistoryRepository paymentHistoryRepository, IVoucherRepository voucherRepository, AppDbContext dbContext)
    {
        _hotelRepository = hotelRepository;
        _userRepository = userRepository;
        _paymentHistoryRepository = paymentHistoryRepository;
        _voucherRepository = voucherRepository;
        _dbContext = dbContext;
    }

    public async Task<List<HotelSummaryDto>> GetSuggestionsAsync(int limit = 4)
    {
        var hotels = await _hotelRepository.GetSuggestionsAsync(limit);
        return await MapToSummaryAsync(hotels);
    }

    public async Task<List<HotelSummaryDto>> GetTopHotelsAsync(int limit)
    {
        var hotels = await _hotelRepository.GetTopHotelsAsync(25);
        var result = await MapToSummaryAsync(hotels);
        result = result.OrderByDescending(h => h.AverageRating)
        .ToList();
        return result.Take(limit).ToList();
        ;
    }
    public static string CleanProvinceName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return name;

        name = name.Trim().ToLower();

        if (name.StartsWith("tỉnh "))
            return name.Substring("tỉnh ".Length).Trim();

        if (name.StartsWith("thành phố "))
            return name.Substring("thành phố ".Length).Trim();

        return name;
    }

    public async Task<List<HotelSummaryDto>> SearchAsync(HotelSearchRequestDto request, int limit = 20, int offset = 0)
    {
        var hotels = await _hotelRepository.SearchHotelsAsync(CleanProvinceName(request.Location), request.Guests, null, null, request?.Type, request?.Stars, request?.Amenities, limit, offset);
        return await MapToSummaryAsync(hotels);
    }

    public async Task<HotelDetailDto> GetDetailAsync(int hotelId)
    {
        var hotel = await _hotelRepository.GetByIdAsync(hotelId) ?? throw new NotFoundException($"Hotel {hotelId} not found");
        var rooms = await _hotelRepository.GetRoomsByHotelAsync(hotelId);
        var rating = await _hotelRepository.GetAverageRatingAsync(hotelId);
        return new HotelDetailDto
        {
            HotelId = hotel.HotelId,
            Name = hotel.Name,
            Description = hotel.Description,
            Address = hotel.Address,
            Image = hotel.Image?.ToString(),
            Style = hotel.Style?.ToString(),
            AverageRating = rating,
            Rooms = rooms.Select(r => new HotelRoomDto
            {
                RoomId = r.RoomId,
                RoomNumber = r.RoomNumber,
                RoomType = r.RoomType,
                PricePerNight = r.PricePerNight,
                Capacity = r.Capacity,
                IsAvailable = r.IsAvailable,
                Image = r.Image?.ToString(),
                LstCheckoutDate = r.Bookingdetails.OrderByDescending(x => x.CheckOutDate).FirstOrDefault()?.CheckOutDate
            }).ToList()
        };
    }

    public async Task<int> BookAsync(HotelBookingRequestDto request, int userId)
    {
        var hotel = await _hotelRepository.GetByIdAsync(request.HotelId) ?? throw new NotFoundException($"Hotel {request.HotelId} not found");
        decimal originalPrice = (decimal)request.TotalPrice; // Giá gốc
        decimal finalPrice = originalPrice;
        decimal discountAmount = 0;
        Voucher? appliedVoucher = null;
        if (!string.IsNullOrEmpty(request.VoucherCode))
        {
            appliedVoucher = await _voucherRepository.GetByCodeAsync(request.VoucherCode);

            // Validation...
            if (appliedVoucher == null) throw new Exception("Voucher không tồn tại.");
            if (!appliedVoucher.IsActive) throw new Exception("Voucher đã bị vô hiệu hóa."); // Dùng C# PascalCase
            if (DateTime.UtcNow > appliedVoucher.EndDate) throw new Exception("Voucher đã hết hạn.");
            if (appliedVoucher.CurrentUsageCount >= appliedVoucher.MaxUsageCount) throw new Exception("Voucher đã hết lượt sử dụng.");
            if (originalPrice < appliedVoucher.MinBookingAmount)
                throw new Exception($"Đơn hàng phải từ {appliedVoucher.MinBookingAmount:N0} VNĐ.");

            // Tính toán
            if (appliedVoucher.DiscountType == DiscountType.Percentage)
            {
                discountAmount = originalPrice * (appliedVoucher.DiscountValue / 100);
            }
            else
            {
                discountAmount = appliedVoucher.DiscountValue;
            }
            finalPrice = originalPrice - discountAmount;
            if (finalPrice < 0) finalPrice = 0;
        }
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            // Trừ tiền ví (dùng finalPrice)
            if (request.TypePayment == 2)
            {
                var user = await _userRepository.GetUserByIdAsync(userId);
                if (user == null) throw new NotFoundException($"User {userId} not found");
                if (finalPrice > user.WalletBalance) // Dùng finalPrice
                {
                    throw new Exception($"Your wallet balance is insufficient!");
                }
                user.WalletBalance = user.WalletBalance - finalPrice; // Dùng finalPrice
                await _userRepository.UpdateUserAsync(user);
            }

            // Tạo Bookingdetail (với các trường giá mới)
            var detail = new Bookingdetail
            {
                UserId = userId,
                HotelId = request.HotelId,
                CheckInDate = request.CheckIn,
                CheckOutDate = request.CheckOut,
                Status = 1,
                RoomId = request.RoomId,
                RestaurantId = request.RestaurantId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Phone = request.Phone,
                Note = request.Note,
                Country = request.Country,
                OriginalPrice = originalPrice,
                DiscountAmount = discountAmount,
                VoucherCode = request.VoucherCode,
                TotalPrice = finalPrice
            };
            if (request.TypePayment == 0)
            {
                detail.Status = 0;
            }
            else if (request.TypePayment == 1 || request.TypePayment == 2)
            {
                detail.Status = 1;
            }
            var created = await _hotelRepository.CreateBookingAsync(detail);

            // Cập nhật số lượt dùng voucher (nếu có)
            if (appliedVoucher != null)
            {
                appliedVoucher.CurrentUsageCount++; // Dùng C# PascalCase
                await _voucherRepository.UpdateAsync(appliedVoucher);
            }

            PaymentHistory paymentHistory = new PaymentHistory();
            paymentHistory.UserId = userId;
            paymentHistory.Amount = (decimal)finalPrice;
            paymentHistory.PaymentMethod = "Wallet";
            paymentHistory.Status = "Done";
            paymentHistory.Description = "Thanh toán đơn hàng " +created.BookingId +" bằng ví thành công";
            paymentHistory.CreatedAt = DateTime.Now;
            var random = new Random();
            paymentHistory.TransactionCode = ((long)random.Next() << 32) | (long)random.Next();
            _paymentHistoryRepository.AddAsync(paymentHistory);

            await transaction.CommitAsync(); // Lưu tất cả thay đổi
            return created.BookingId;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(); // Hoàn tác nếu có lỗi
            throw;
        }
    }

    public async Task<List<BookingHistoryDto>> GetBookingHistoryAsync(int userId, DateOnly? bookingDate)
    {
        var bookings = await _hotelRepository.GetBookingHistoryAsync(userId, bookingDate);
        return bookings.Select(b => new BookingHistoryDto
        {
            BookingId = b.BookingId,
            UserId = b.UserId ?? 0,
            HotelId = b.HotelId,
            BookingDate = b.BookingDate ?? DateTime.UtcNow,
            CheckInDate = b.CheckInDate,
            CheckOutDate = b.CheckOutDate,
            TotalPrice = b.TotalPrice,
            Approved = b.Approved ?? false,
            FirstName = b.FirstName,
            LastName = b.LastName,
            Email = b.Email,
            Phone = b.Phone,
            Note = b.Note,
            Country = b.Country,
            Status = b.Status
        }).ToList();
    }

    public async Task<List<ReviewDto>> GetReviewsAsync(int hotelId, int? rating, int limit = 20, int offset = 0)
    {
        var reviews = await _hotelRepository.GetReviewsByHotelAsync(hotelId, rating, limit, offset);
        return reviews.Select(r => new ReviewDto
        {
            ReviewId = r.ReviewId,
            UserId = r.UserId,
            TourId = r.TourId,
            HotelId = r.HotelId,
            RestaurantId = r.RestaurantId,
            Rating = r.Rating,
            Comment = r.Comment,
            Image = r.Image,
            ReviewDate = r.ReviewDate,
            UserName = r.User?.FullName ?? r.User?.Username,
            HotelName = r.Hotel?.Name,
            RestaurantName = r.Restaurant?.Name,
            TourName = r.Tour?.Title
        }).ToList();
    }

    private async Task<List<HotelSummaryDto>> MapToSummaryAsync(List<Hotel> hotels)
    {
        var result = new List<HotelSummaryDto>(hotels.Count);

        foreach (var h in hotels)
        {
            var avg = await _hotelRepository.GetAverageRatingAsync(h.HotelId);
            var rooms = await _hotelRepository.GetRoomsByHotelAsync(h.HotelId);

            result.Add(new HotelSummaryDto
            {
                HotelId = h.HotelId,
                Name = h.Name,
                Address = h.Address,
                Image = h.Image?.ToString(),
                AverageRating = avg,
                Style = h.Style,
                Description = h.Description,
                Rooms = rooms.Select(r => new HotelRoomDto
                {
                    RoomId = r.RoomId,

                    PricePerNight = r.PricePerNight,


                }).ToList()
            });
        }

        return result;
    }

    public Task<int> ChangeStatusBookingAsync(int bookingId, int status)
    {
        return _hotelRepository.ChangeStatusBookingAsync(bookingId, status);
    }

    public async Task<List<VoucherDto>> GetActiveVouchersAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("--- 1. Đang vào HotelService.GetActiveVouchersAsync ---");

            // 1. KIỂM TRA LỖI DI (Dependency Injection)
            if (_voucherRepository == null)
            {
                System.Diagnostics.Debug.WriteLine("--- LỖI NGHIÊM TRỌNG: _voucherRepository BỊ NULL! ---");
                System.Diagnostics.Debug.WriteLine("--- KIỂM TRA DI TRONG Program.cs. Bạn đã thêm builder.Services.AddScoped<IVoucherRepository, VoucherRepository>(); CHƯA? ---");
                throw new Exception("_voucherRepository was null. Check DI in Program.cs");
            }

            System.Diagnostics.Debug.WriteLine("--- 2. _voucherRepository không null. Đang gọi GetActiveVouchersAsync... ---");
            var vouchers = await _voucherRepository.GetActiveVouchersAsync();
            System.Diagnostics.Debug.WriteLine("--- 3. Đã gọi xong GetActiveVouchersAsync ---");

            // 2. KIỂM TRA KẾT QUẢ TRẢ VỀ TỪ REPOSITORY
            if (vouchers == null)
            {
                System.Diagnostics.Debug.WriteLine("--- LỖI: _voucherRepository.GetActiveVouchersAsync() đã trả về NULL! ---");
                System.Diagnostics.Debug.WriteLine("--- Lỗi này nằm trong VoucherRepository.cs. ToListAsync() không thể trả về null trừ khi _context hoặc _context.Vouchers bị null. ---");
                throw new Exception("Voucher list returned from repository was null.");
            }

            System.Diagnostics.Debug.WriteLine($"--- 4. Lấy được {vouchers.Count} vouchers. Đang map sang DTO... ---");

            return vouchers.Select(v => new VoucherDto
            {
                Code = v.Code,
                Description = v.Description,
                DiscountType = v.DiscountType,
                DiscountValue = v.DiscountValue,
                MinBookingAmount = v.MinBookingAmount,
                StartDate = v.StartDate,
                EndDate = v.EndDate,
                MaxUsageCount = v.MaxUsageCount,
                CurrentUsageCount = v.CurrentUsageCount,
                IsActive = v.IsActive
            }).ToList();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("--- !!! LỖI TRONG HotelService.GetActiveVouchersAsync !!! ---");
            // Ghi log đầy đủ stack trace
            System.Diagnostics.Debug.WriteLine(ex.ToString());
            throw; // Ném lỗi lại để API báo lỗi 500
        }
    }
    public async Task<VoucherDto?> GetVoucherByCodeAsync(string code)
    {
        // 1. Gọi repository để tìm voucher
        var voucher = await _voucherRepository.GetByCodeAsync(code);

        // 2. Nếu không tìm thấy, trả về null
        if (voucher == null)
        {
            return null;
        }

        // 3. Nếu tìm thấy, map sang DTO để trả về
        return new VoucherDto
        {
            Code = voucher.Code,
            Description = voucher.Description,
            DiscountType = voucher.DiscountType,
            DiscountValue = voucher.DiscountValue,
            MinBookingAmount = voucher.MinBookingAmount,
            StartDate = voucher.StartDate,
            EndDate = voucher.EndDate,
            MaxUsageCount = voucher.MaxUsageCount,
            CurrentUsageCount = voucher.CurrentUsageCount,
            IsActive = voucher.IsActive
        };
    }
}


