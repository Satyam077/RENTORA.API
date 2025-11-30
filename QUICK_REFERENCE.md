# RENTORA API - Quick Reference Guide

## 🚀 Quick Start

### 1. Run the Application
```bash
cd "d:\CHANDAN\CHANDAN 2025\Uniquex PMS\RENTORA\RENTORA.API"
dotnet run
```

### 2. Access Swagger UI
```
https://localhost:5001/swagger
```

## 📋 API Endpoints

### Base URL
```
https://localhost:5001
```

### Authentication Endpoints

#### 1️⃣ Register User
```http
POST /api/auth/register
Content-Type: application/json

{
  "fullName": "John Doe",
  "email": "john@example.com",
  "mobile": "+1234567890",
  "password": "Password123",
  "confirmPassword": "Password123",
  "gender": "Male",
  "dateOfBirth": "1990-01-01",
  "role": "tenant"
}
```

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Registration successful",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": "507f1f77bcf86cd799439011",
    "fullName": "John Doe",
    "email": "john@example.com",
    "mobile": "+1234567890",
    "role": "tenant",
    "isEmailVerified": false,
    "isMobileVerified": false
  }
}
```

#### 2️⃣ Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "emailOrMobile": "john@example.com",
  "password": "Password123"
}
```

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Login successful",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": "507f1f77bcf86cd799439011",
    "fullName": "John Doe",
    "email": "john@example.com",
    "mobile": "+1234567890",
    "role": "tenant",
    "isEmailVerified": false,
    "isMobileVerified": false
  }
}
```

#### 3️⃣ Send OTP
```http
POST /api/auth/send-otp
Content-Type: application/json

"john@example.com"
```

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "OTP sent successfully"
}
```

#### 4️⃣ Verify OTP
```http
POST /api/auth/verify-otp
Content-Type: application/json

{
  "emailOrMobile": "john@example.com",
  "otpCode": "123456"
}
```

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "OTP verified successfully"
}
```

## 🔐 Using JWT Token

### In Swagger UI
1. Click the **"Authorize"** button (🔒 icon)
2. Enter: `Bearer {your-token-here}`
3. Click **"Authorize"**
4. Click **"Close"**

### In Postman
1. Go to **Authorization** tab
2. Select **Type**: Bearer Token
3. Paste your token in the **Token** field

### In HTTP Headers
```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## 👥 User Roles

| Role | Value | Description |
|------|-------|-------------|
| Super Admin | `superadmin` | Full system access |
| Admin | `admin` | Administrative access |
| Landlord | `landlord` | Property owner |
| Tenant | `tenant` | Property renter |
| Agent | `agent` | Property agent |

## ✅ Validation Rules

### Registration
- **Full Name**: Required, 2-100 characters
- **Email**: Valid email format (optional if mobile provided)
- **Mobile**: Valid phone format (optional if email provided)
- **Password**: Required, minimum 6 characters
- **Confirm Password**: Must match password
- **Gender**: Optional
- **Date of Birth**: Optional
- **Role**: Optional (defaults to "tenant")

### Login
- **Email or Mobile**: Required
- **Password**: Required

### OTP Verification
- **Email or Mobile**: Required
- **OTP Code**: Required, exactly 6 digits

## 🔒 Security Features

### Password Security
- ✅ HMACSHA512 hashing
- ✅ Unique salt per user
- ✅ Never stored in plain text

### JWT Token
- ✅ Expires in 60 minutes (configurable)
- ✅ Contains user claims (ID, Name, Email, Mobile, Role)
- ✅ Signed with secret key
- ✅ Validated on each request

### Input Validation
- ✅ Data annotations on DTOs
- ✅ Model state validation
- ✅ Business logic validation

## 📊 HTTP Status Codes

| Code | Meaning | When Used |
|------|---------|-----------|
| 200 | OK | Successful request |
| 400 | Bad Request | Invalid input, validation failed |
| 401 | Unauthorized | Invalid credentials, expired token |
| 404 | Not Found | Resource not found |
| 500 | Internal Server Error | Server error |

## 🐛 Common Errors

### Error Response Format
```json
{
  "success": false,
  "message": "Error description here"
}
```

### Common Error Messages

#### Registration Errors
- "Email or Mobile is required"
- "Passwords do not match"
- "User with this email or mobile already exists"
- "Full name is required"
- "Password must be at least 6 characters"

#### Login Errors
- "Invalid credentials"
- "Account is inactive. Please contact support."
- "Email or mobile is required"
- "Password is required"

#### OTP Errors
- "Email or mobile is required"
- "Failed to send OTP"
- "Invalid or expired OTP"
- "OTP must be 6 digits"

## 🧪 Testing Scenarios

### Scenario 1: New User Registration
```bash
1. POST /api/auth/register (with valid data)
2. Copy the token from response
3. Use token for authenticated requests
```

### Scenario 2: Existing User Login
```bash
1. POST /api/auth/login (with email/mobile and password)
2. Copy the token from response
3. Use token for authenticated requests
```

### Scenario 3: OTP Verification
```bash
1. POST /api/auth/send-otp (with email or mobile)
2. Check console for OTP code (in development)
3. POST /api/auth/verify-otp (with email/mobile and OTP)
4. User's email/mobile is now verified
```

## 📝 Sample Test Data

### Test User 1 - Tenant
```json
{
  "fullName": "Alice Johnson",
  "email": "alice@example.com",
  "mobile": "+1234567890",
  "password": "Alice@123",
  "confirmPassword": "Alice@123",
  "gender": "Female",
  "dateOfBirth": "1995-05-15",
  "role": "tenant"
}
```

### Test User 2 - Landlord
```json
{
  "fullName": "Bob Smith",
  "email": "bob@example.com",
  "mobile": "+9876543210",
  "password": "Bob@123",
  "confirmPassword": "Bob@123",
  "gender": "Male",
  "dateOfBirth": "1980-03-20",
  "role": "landlord"
}
```

### Test User 3 - Agent
```json
{
  "fullName": "Charlie Brown",
  "email": "charlie@example.com",
  "mobile": "+5555555555",
  "password": "Charlie@123",
  "confirmPassword": "Charlie@123",
  "gender": "Male",
  "dateOfBirth": "1988-11-10",
  "role": "agent"
}
```

## 🔧 Configuration

### MongoDB Connection
Located in `appsettings.json`:
```json
{
  "MongoDb": {
    "ConnectionString": "mongodb+srv://username:password@cluster.mongodb.net/",
    "Database": "RENTORA"
  }
}
```

### JWT Settings
Located in `appsettings.json`:
```json
{
  "Jwt": {
    "SecretKey": "your-secret-key-here",
    "Issuer": "RailwayTicketBooking",
    "Audience": "RailwayTicketBookingUsers",
    "ExpirationMinutes": "60"
  }
}
```

## 📦 NuGet Packages

```xml
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.0" />
<PackageReference Include="MongoDB.Bson" Version="3.5.2" />
<PackageReference Include="MongoDB.Driver" Version="3.5.2" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.6.2" />
<PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="8.0.0" />
```

## 🛠️ Useful Commands

### Build Project
```bash
dotnet build
```

### Run Project
```bash
dotnet run
```

### Restore Packages
```bash
dotnet restore
```

### Clean Build
```bash
dotnet clean
```

### Watch Mode (Auto-reload)
```bash
dotnet watch run
```

## 📚 Additional Resources

- **API Documentation**: `README.md`
- **Implementation Details**: `IMPLEMENTATION_SUMMARY.md`
- **Architecture Diagrams**: `ARCHITECTURE.md`
- **Postman Collection**: `RENTORA_API.postman_collection.json`

## 💡 Tips & Best Practices

1. **Always use HTTPS** in production
2. **Store JWT tokens securely** (HttpOnly cookies or secure storage)
3. **Never commit secrets** to version control
4. **Use environment variables** for sensitive configuration
5. **Implement rate limiting** for authentication endpoints
6. **Add logging** for debugging and monitoring
7. **Write unit tests** for critical functionality
8. **Keep packages updated** for security patches

## 🆘 Troubleshooting

### Issue: Build Fails
```bash
# Solution: Restore packages
dotnet restore
dotnet clean
dotnet build
```

### Issue: MongoDB Connection Failed
```bash
# Solution: Check connection string in appsettings.json
# Ensure MongoDB is running and accessible
# Check network connectivity
```

### Issue: JWT Token Invalid
```bash
# Solution: Check token expiration
# Verify secret key matches in configuration
# Ensure token is properly formatted: "Bearer {token}"
```

### Issue: OTP Not Received
```bash
# Solution: Check console output (development mode)
# In production, implement actual email/SMS service
```

## 📞 Support

For issues or questions:
1. Check the documentation files
2. Review error messages in Swagger UI
3. Check application logs
4. Contact the development team

---
**Happy Coding! 🚀**
