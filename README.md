# RENTORA API - Property Management System

## Overview
RENTORA is a comprehensive Property Management System API built with ASP.NET Core 8.0, MongoDB, and JWT authentication.

## Features Implemented

### Authentication System
- ✅ User Registration with validation
- ✅ User Login with JWT token generation
- ✅ OTP generation and verification
- ✅ Password hashing using HMACSHA512
- ✅ Multi-role support (SuperAdmin, Admin, Landlords, Tenants, Agents)

## Project Structure

```
RENTORA.API/
├── Controllers/
│   ├── AuthController.cs          # Authentication endpoints
│   └── WeatherForecastController.cs
├── Models/
│   ├── BaseEntity.cs              # Base entity with common properties
│   ├── Registration.cs            # User registration entity
│   ├── Login.cs                   # Login response models
│   ├── BsonCollectionAttribute.cs # MongoDB collection attribute
│   ├── DTOs/
│   │   ├── LoginDTO.cs           # Login request DTO
│   │   ├── RegistrationDTO.cs    # Registration request DTO
│   │   ├── OtpData.cs            # OTP data model
│   │   └── VerifyOtpRequest.cs   # OTP verification DTO
│   ├── Enums/
│   │   └── enum.cs               # Role enumerations
│   └── MongoDB/
│       ├── MongoDbSettings.cs    # MongoDB configuration
│       └── JwtSettings.cs        # JWT configuration
├── Repository/
│   ├── IRepository/
│   │   └── IUserRepository.cs    # User repository interface
│   └── UserRepository.cs         # User repository implementation
├── Services/
│   ├── IAuthService.cs           # Authentication service interface
│   └── AuthService.cs            # Authentication service implementation
├── Program.cs                     # Application configuration
└── appsettings.json              # Configuration settings
```

## API Endpoints

### Authentication Endpoints

#### 1. Register User
```http
POST /api/auth/register
Content-Type: application/json

{
  "fullName": "John Doe",
  "email": "john.doe@example.com",
  "mobile": "+1234567890",
  "password": "SecurePassword123",
  "confirmPassword": "SecurePassword123",
  "gender": "Male",
  "dateOfBirth": "1990-01-01",
  "role": "tenant"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Registration successful",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": "507f1f77bcf86cd799439011",
    "fullName": "John Doe",
    "email": "john.doe@example.com",
    "mobile": "+1234567890",
    "role": "tenant",
    "isEmailVerified": false,
    "isMobileVerified": false
  }
}
```

#### 2. Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "emailOrMobile": "john.doe@example.com",
  "password": "SecurePassword123"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Login successful",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": "507f1f77bcf86cd799439011",
    "fullName": "John Doe",
    "email": "john.doe@example.com",
    "mobile": "+1234567890",
    "role": "tenant",
    "isEmailVerified": false,
    "isMobileVerified": false
  }
}
```

#### 3. Send OTP
```http
POST /api/auth/send-otp
Content-Type: application/json

"john.doe@example.com"
```

**Response:**
```json
{
  "success": true,
  "message": "OTP sent successfully"
}
```

#### 4. Verify OTP
```http
POST /api/auth/verify-otp
Content-Type: application/json

{
  "emailOrMobile": "john.doe@example.com",
  "otpCode": "123456"
}
```

**Response:**
```json
{
  "success": true,
  "message": "OTP verified successfully"
}
```

## Configuration

### appsettings.json
```json
{
  "MongoDb": {
    "ConnectionString": "mongodb+srv://username:password@cluster.mongodb.net/",
    "Database": "RENTORA"
  },
  "Jwt": {
    "SecretKey": "your-secret-key-here",
    "Issuer": "RailwayTicketBooking",
    "Audience": "RailwayTicketBookingUsers",
    "ExpirationMinutes": "60"
  }
}
```

## Database Schema

### Users Collection
```json
{
  "_id": "ObjectId",
  "fullName": "string",
  "gender": "string",
  "dateOfBirth": "DateTime",
  "email": "string",
  "isEmailVerified": "boolean",
  "mobile": "string",
  "isMobileVerified": "boolean",
  "passwordHash": "string",
  "passwordSalt": "string",
  "lastOtpCode": "string",
  "lastOtpGeneratedAt": "DateTime",
  "isOtpVerified": "boolean",
  "profileImageUrl": "string",
  "address": {
    "addressLine1": "string",
    "addressLine2": "string",
    "city": "string",
    "state": "string",
    "country": "string",
    "zipCode": "string"
  },
  "role": "string",
  "tenantId": "string",
  "ownerId": "string",
  "createdBy": "string",
  "isActive": "boolean",
  "isDeleted": "boolean",
  "createdAt": "DateTime",
  "updatedAt": "DateTime"
}
```

## User Roles
- **SuperAdmin**: Full system access
- **Admin**: Administrative access
- **Landlords**: Property owner access
- **Tenants**: Tenant access
- **Agents**: Agent access

## Security Features
- ✅ Password hashing with HMACSHA512
- ✅ JWT token-based authentication
- ✅ Token expiration (configurable)
- ✅ Role-based authorization ready
- ✅ CORS enabled
- ✅ Input validation with data annotations

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- MongoDB Atlas account or local MongoDB instance

### Installation

1. **Restore NuGet packages:**
```bash
dotnet restore
```

2. **Update appsettings.json** with your MongoDB connection string

3. **Run the application:**
```bash
dotnet run
```

4. **Access Swagger UI:**
```
https://localhost:5001/swagger
```

## Testing with Swagger

1. Navigate to the Swagger UI
2. Use the `/api/auth/register` endpoint to create a new user
3. Copy the JWT token from the response
4. Click the "Authorize" button in Swagger
5. Enter: `Bearer {your-token-here}`
6. Now you can test protected endpoints

## Next Steps

### Recommended Enhancements
1. **Email Service Integration**: Implement actual email sending for OTP
2. **SMS Service Integration**: Implement SMS sending for mobile OTP
3. **Password Reset**: Add forgot password functionality
4. **Refresh Tokens**: Implement refresh token mechanism
5. **Rate Limiting**: Add rate limiting for authentication endpoints
6. **Logging**: Implement structured logging with Serilog
7. **Unit Tests**: Add comprehensive unit tests
8. **API Versioning**: Implement API versioning
9. **Health Checks**: Add health check endpoints

## NuGet Packages Used
- Microsoft.AspNetCore.Authentication.JwtBearer (8.0.0)
- MongoDB.Bson (3.5.2)
- MongoDB.Driver (3.5.2)
- Swashbuckle.AspNetCore (6.6.2)
- System.IdentityModel.Tokens.Jwt (8.0.0)

## License
This project is part of the RENTORA Property Management System.

## Support
For issues and questions, please contact the development team.
