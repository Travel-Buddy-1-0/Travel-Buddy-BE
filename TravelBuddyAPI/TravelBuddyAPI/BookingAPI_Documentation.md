# Hotel Booking API Documentation

## Tổng quan
API Booking cho phép khách hàng đặt phòng khách sạn với đầy đủ thông tin chi tiết bao gồm thông tin khách hàng, yêu cầu đặc biệt, thời gian đến, và chi tiết đặt phòng.

## Base URL
```
https://localhost:7000/api/booking
```

## Endpoints

### 1. Tạo Booking Mới
**POST** `/hotel`

Tạo một booking mới cho khách sạn.

#### Request Body
```json
{
  "userId": 1,
  "hotelId": 1,
  "checkInDate": "2024-12-15",
  "checkOutDate": "2024-12-18",
  "numberOfNights": 3,
  "numberOfGuests": 2,
  "numberOfRooms": 1,
  "guestArrivalTime": "14:00",
  "specialRequirements": "Late check-in requested, need extra pillows",
  "guestInformation": {
    "fullName": "Nguyen Van A",
    "email": "nguyenvana@email.com",
    "phoneNumber": "+84901234567",
    "address": "123 Le Loi Street, District 1, Ho Chi Minh City",
    "nationality": "Vietnamese",
    "idNumber": "123456789"
  }
}
```

#### Response
```json
{
  "bookingId": 1,
  "userId": 1,
  "hotelId": 1,
  "hotelName": "Grand Hotel",
  "hotelAddress": "123 Main Street",
  "checkInDate": "2024-12-15",
  "checkOutDate": "2024-12-18",
  "numberOfNights": 3,
  "numberOfGuests": 2,
  "numberOfRooms": 1,
  "guestArrivalTime": "14:00",
  "specialRequirements": "Late check-in requested, need extra pillows",
  "guestInformation": {
    "fullName": "Nguyen Van A",
    "email": "nguyenvana@email.com",
    "phoneNumber": "+84901234567",
    "address": "123 Le Loi Street, District 1, Ho Chi Minh City",
    "nationality": "Vietnamese",
    "idNumber": "123456789"
  },
  "totalPrice": 300.00,
  "approved": false,
  "bookingDate": "2024-12-01T10:30:00Z",
  "bookingStatus": "Pending",
  "confirmationNumber": "TB202412011030001234"
}
```

### 2. Lấy Booking theo ID
**GET** `/{id}`

Lấy thông tin chi tiết của một booking.

#### Response
Trả về thông tin booking tương tự như endpoint tạo booking.

### 3. Lấy Booking theo Confirmation Number
**GET** `/confirmation/{confirmationNumber}`

Lấy thông tin booking bằng mã xác nhận.

### 4. Lấy Bookings của User
**GET** `/user/{userId}`

Lấy danh sách tất cả bookings của một user.

### 5. Lấy Bookings của Hotel
**GET** `/hotel/{hotelId}`

Lấy danh sách tất cả bookings của một khách sạn.

### 6. Cập nhật Booking
**PUT** `/{id}`

Cập nhật thông tin của một booking đã tồn tại.

#### Request Body
Tương tự như request body của endpoint tạo booking.

### 7. Xóa Booking
**DELETE** `/{id}`

Xóa một booking.

### 8. Duyệt Booking
**PATCH** `/{id}/approve`

Duyệt một booking (chuyển trạng thái thành "Confirmed").

### 9. Hủy Booking
**PATCH** `/{id}/cancel`

Hủy một booking (chuyển trạng thái thành "Cancelled").

### 10. Lấy Bookings theo Khoảng Thời gian
**GET** `/date-range?startDate={startDate}&endDate={endDate}`

Lấy danh sách bookings trong một khoảng thời gian.

#### Query Parameters
- `startDate`: Ngày bắt đầu (format: yyyy-MM-dd)
- `endDate`: Ngày kết thúc (format: yyyy-MM-dd)

### 11. Lấy Thông tin Xác nhận Booking
**GET** `/{id}/confirmation`

Lấy thông tin xác nhận booking (dạng tóm tắt).

#### Response
```json
{
  "bookingId": 1,
  "confirmationNumber": "TB202412011030001234",
  "hotelName": "Grand Hotel",
  "checkInDate": "2024-12-15",
  "checkOutDate": "2024-12-18",
  "numberOfNights": 3,
  "numberOfGuests": 2,
  "numberOfRooms": 1,
  "totalPrice": 300.00,
  "guestName": "Nguyen Van A",
  "guestEmail": "nguyenvana@email.com",
  "guestPhone": "+84901234567",
  "bookingDate": "2024-12-01T10:30:00Z",
  "bookingStatus": "Pending"
}
```

### 12. Kiểm tra Tính khả dụng của Khách sạn
**GET** `/availability?hotelId={hotelId}&checkInDate={checkInDate}&checkOutDate={checkOutDate}&numberOfRooms={numberOfRooms}`

Kiểm tra xem khách sạn có còn phòng trống trong khoảng thời gian yêu cầu không.

#### Query Parameters
- `hotelId`: ID của khách sạn
- `checkInDate`: Ngày check-in (format: yyyy-MM-dd)
- `checkOutDate`: Ngày check-out (format: yyyy-MM-dd)
- `numberOfRooms`: Số lượng phòng cần đặt

#### Response
```json
{
  "hotelId": 1,
  "checkInDate": "2024-12-20",
  "checkOutDate": "2024-12-23",
  "numberOfRooms": 2,
  "isAvailable": true
}
```

## Validation Rules

### HotelBookingRequestDto
- `userId`: Bắt buộc, phải là số nguyên dương
- `hotelId`: Bắt buộc, phải là số nguyên dương
- `checkInDate`: Bắt buộc, phải là ngày hợp lệ và không được trong quá khứ
- `checkOutDate`: Bắt buộc, phải là ngày hợp lệ và sau ngày check-in
- `numberOfNights`: Bắt buộc, phải >= 1
- `numberOfGuests`: Bắt buộc, phải >= 1
- `numberOfRooms`: Bắt buộc, phải >= 1
- `guestArrivalTime`: Bắt buộc, phải là thời gian hợp lệ
- `specialRequirements`: Tùy chọn, tối đa 500 ký tự
- `guestInformation`: Bắt buộc, phải chứa đầy đủ thông tin

### GuestInformationDto
- `fullName`: Bắt buộc, tối đa 100 ký tự
- `email`: Bắt buộc, phải là email hợp lệ, tối đa 100 ký tự
- `phoneNumber`: Bắt buộc, phải là số điện thoại hợp lệ, tối đa 20 ký tự
- `address`: Tùy chọn, tối đa 200 ký tự
- `nationality`: Tùy chọn, tối đa 50 ký tự
- `idNumber`: Tùy chọn, tối đa 20 ký tự

## Error Responses

### 400 Bad Request
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "userId": ["User ID is required"],
    "checkInDate": ["Check-in date must be before check-out date"]
  }
}
```

### 404 Not Found
```json
{
  "message": "Hotel with ID 999 not found."
}
```

### 409 Conflict
```json
{
  "message": "Hotel is not available for the selected dates and number of rooms."
}
```

### 500 Internal Server Error
```json
{
  "message": "An error occurred while creating the booking."
}
```

## Business Logic

1. **Tính giá**: Giá được tính dựa trên số đêm × số phòng × giá mỗi đêm (hiện tại cố định 100$/đêm/phòng)

2. **Mã xác nhận**: Tự động tạo theo format `TB{yyyyMMddHHmmss}{random4digits}`

3. **Trạng thái booking**: 
   - `Pending`: Chờ duyệt
   - `Confirmed`: Đã duyệt
   - `Cancelled`: Đã hủy

4. **Kiểm tra tính khả dụng**: Kiểm tra xem khách sạn có đủ phòng trống trong khoảng thời gian yêu cầu không

5. **Validation ngày**: 
   - Ngày check-in không được trong quá khứ
   - Ngày check-out phải sau ngày check-in
   - Số đêm được tính tự động từ ngày check-in và check-out

## Database Schema

### BookingDetail Table
- `booking_id`: Primary key
- `user_id`: Foreign key to users table
- `hotel_id`: Foreign key to hotel table
- `check_in_date`: Ngày nhận phòng
- `check_out_date`: Ngày trả phòng
- `number_of_nights`: Số đêm
- `number_of_guests`: Số lượng khách
- `number_of_rooms`: Số lượng phòng
- `guest_arrival_time`: Thời gian đến của khách
- `special_requirements`: Yêu cầu đặc biệt
- `guest_information`: Thông tin khách (JSON)
- `total_price`: Tổng giá
- `approved`: Đã duyệt hay chưa
- `confirmation_number`: Mã xác nhận
- `booking_status`: Trạng thái booking
- `booking_date`: Ngày tạo booking
