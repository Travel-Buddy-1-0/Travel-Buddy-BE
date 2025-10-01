# API Get Room Detail by Room ID

## Tổng quan
API này được xây dựng theo kiến trúc 3-layer để lấy thông tin chi tiết của phòng theo ID, bao gồm thời gian checkout gần nhất.

## Kiến trúc 3-Layer

### 1. Presentation Layer (Controller)
- **File**: `TravelBuddyAPI/Controllers/RoomController.cs`
- **Chức năng**: Xử lý HTTP requests và responses
- **Endpoint**: `GET /api/room/{id}`

### 2. Business Logic Layer (Service)
- **Interface**: `TravelBuddyAPI/Services/IRoomService.cs`
- **Implementation**: `TravelBuddyAPI/Services/RoomService.cs`
- **Chức năng**: Xử lý business logic, mapping DTO

### 3. Data Access Layer (Repository)
- **Interface**: `TravelBuddyAPI/Repositories/IRoomRepository.cs`
- **Implementation**: `TravelBuddyAPI/Repositories/RoomRepository.cs`
- **Chức năng**: Truy cập database, thao tác với Entity Framework

## DTO (Data Transfer Object)
- **File**: `TravelBuddyAPI/BusinessObject/DTOs/RoomDetailDto.cs`
- **Chức năng**: Định nghĩa cấu trúc dữ liệu trả về cho client

## Luồng xử lý

```
Client Request → RoomController → RoomService → RoomRepository → Database
                ↓
Client Response ← RoomController ← RoomService ← RoomRepository ← Database
```

## Cách sử dụng

### Request
```
GET /api/room/{id}
```

### Response thành công (200 OK)
```json
{
  "roomId": 1,
  "hotelId": 1,
  "roomNumber": "101",
  "roomType": "Deluxe",
  "pricePerNight": 150.00,
  "capacity": 2,
  "isAvailable": true,
  "image": {...},
  "lastCheckoutDate": "2024-01-15T12:00:00Z",
  "hotel": {
    "hotelId": 1,
    "name": "Hotel Name",
    "address": "Hotel Address",
    "description": "Hotel Description",
    "image": {...}
  }
}
```

### Response lỗi (404 Not Found)
```json
{
  "message": "Room with ID {id} not found"
}
```

### Response lỗi server (500 Internal Server Error)
```json
{
  "message": "An error occurred while retrieving room details",
  "error": "Error details"
}
```

## Cài đặt Dependency Injection
Các dependencies đã được đăng ký trong `Program.cs`:
- `IRoomRepository` → `RoomRepository`
- `IRoomService` → `RoomService`

## Testing
Sử dụng file `room-test.http` để test API với các trường hợp:
- Room tồn tại
- Room không tồn tại
