# RENTORA API - Architecture Diagram

## System Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                         CLIENT LAYER                             │
│  (Web Browser, Mobile App, Postman, Swagger UI)                 │
└────────────────────────┬────────────────────────────────────────┘
                         │
                         │ HTTPS/JSON
                         │
┌────────────────────────▼────────────────────────────────────────┐
│                    CONTROLLER LAYER                              │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │  AuthController                                          │   │
│  │  - POST /api/auth/register                              │   │
│  │  - POST /api/auth/login                                 │   │
│  │  - POST /api/auth/send-otp                              │   │
│  │  - POST /api/auth/verify-otp                            │   │
│  └────────────────────┬─────────────────────────────────────┘   │
└─────────────────────────┼───────────────────────────────────────┘
                         │
                         │ DTOs (Data Transfer Objects)
                         │
┌────────────────────────▼────────────────────────────────────────┐
│                     SERVICE LAYER                                │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │  IAuthService / AuthService                             │   │
│  │  - RegisterAsync()                                      │   │
│  │  - LoginAsync()                                         │   │
│  │  - SendOtpAsync()                                       │   │
│  │  - VerifyOtpAsync()                                     │   │
│  │  - GenerateJwtToken()                                   │   │
│  │  - CreatePasswordHash()                                 │   │
│  │  - VerifyPasswordHash()                                 │   │
│  └────────────────────┬─────────────────────────────────────┘   │
└─────────────────────────┼───────────────────────────────────────┘
                         │
                         │ Entity Models
                         │
┌────────────────────────▼────────────────────────────────────────┐
│                   REPOSITORY LAYER                               │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │  IUserRepository / UserRepository                       │   │
│  │  - GetUserByEmailAsync()                                │   │
│  │  - GetUserByMobileAsync()                               │   │
│  │  - GetUserByEmailOrMobileAsync()                        │   │
│  │  - GetUserByIdAsync()                                   │   │
│  │  - CreateUserAsync()                                    │   │
│  │  - UpdateUserAsync()                                    │   │
│  │  - DeleteUserAsync()                                    │   │
│  │  - GetAllUsersAsync()                                   │   │
│  └────────────────────┬─────────────────────────────────────┘   │
└─────────────────────────┼───────────────────────────────────────┘
                         │
                         │ MongoDB Driver
                         │
┌────────────────────────▼────────────────────────────────────────┐
│                     DATABASE LAYER                               │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │  MongoDB Atlas / Local MongoDB                          │   │
│  │  ┌────────────────────────────────────────────────┐     │   │
│  │  │  RENTORA Database                              │     │   │
│  │  │  └─ Users Collection                           │     │   │
│  │  │     - _id (ObjectId)                           │     │   │
│  │  │     - fullName, email, mobile                  │     │   │
│  │  │     - passwordHash, passwordSalt               │     │   │
│  │  │     - role, isActive, isDeleted                │     │   │
│  │  │     - createdAt, updatedAt                     │     │   │
│  │  └────────────────────────────────────────────────┘     │   │
│  └──────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
```

## Data Flow - User Registration

```
┌─────────┐     ┌────────────┐     ┌──────────┐     ┌────────────┐     ┌──────────┐
│ Client  │────▶│ Controller │────▶│ Service  │────▶│ Repository │────▶│ MongoDB  │
└─────────┘     └────────────┘     └──────────┘     └────────────┘     └──────────┘
    │                 │                  │                  │                 │
    │ POST /register  │                  │                  │                 │
    │ RegistrationDTO │                  │                  │                 │
    ├────────────────▶│                  │                  │                 │
    │                 │ Validate Model   │                  │                 │
    │                 ├─────────────────▶│                  │                 │
    │                 │                  │ Hash Password    │                 │
    │                 │                  │ Check Duplicate  │                 │
    │                 │                  ├─────────────────▶│                 │
    │                 │                  │                  │ Query Users     │
    │                 │                  │                  ├────────────────▶│
    │                 │                  │                  │◀────────────────┤
    │                 │                  │◀─────────────────┤                 │
    │                 │                  │ Create User      │                 │
    │                 │                  ├─────────────────▶│                 │
    │                 │                  │                  │ Insert Document │
    │                 │                  │                  ├────────────────▶│
    │                 │                  │                  │◀────────────────┤
    │                 │                  │◀─────────────────┤                 │
    │                 │                  │ Generate JWT     │                 │
    │                 │◀─────────────────┤                  │                 │
    │◀────────────────┤                  │                  │                 │
    │ LoginResponse   │                  │                  │                 │
    │ (with JWT)      │                  │                  │                 │
```

## Data Flow - User Login

```
┌─────────┐     ┌────────────┐     ┌──────────┐     ┌────────────┐     ┌──────────┐
│ Client  │────▶│ Controller │────▶│ Service  │────▶│ Repository │────▶│ MongoDB  │
└─────────┘     └────────────┘     └──────────┘     └────────────┘     └──────────┘
    │                 │                  │                  │                 │
    │ POST /login     │                  │                  │                 │
    │ LoginDTO        │                  │                  │                 │
    ├────────────────▶│                  │                  │                 │
    │                 │ Validate Model   │                  │                 │
    │                 ├─────────────────▶│                  │                 │
    │                 │                  │ Find User        │                 │
    │                 │                  ├─────────────────▶│                 │
    │                 │                  │                  │ Query by Email  │
    │                 │                  │                  │ or Mobile       │
    │                 │                  │                  ├────────────────▶│
    │                 │                  │                  │◀────────────────┤
    │                 │                  │◀─────────────────┤                 │
    │                 │                  │ Verify Password  │                 │
    │                 │                  │ Check Active     │                 │
    │                 │                  │ Generate JWT     │                 │
    │                 │◀─────────────────┤                  │                 │
    │◀────────────────┤                  │                  │                 │
    │ LoginResponse   │                  │                  │                 │
    │ (with JWT)      │                  │                  │                 │
```

## Security Flow - JWT Authentication

```
┌─────────────────────────────────────────────────────────────────┐
│                    JWT Token Generation                          │
└─────────────────────────────────────────────────────────────────┘
                         │
                         ▼
        ┌────────────────────────────────┐
        │  User Credentials Validated    │
        └────────────┬───────────────────┘
                     │
                     ▼
        ┌────────────────────────────────┐
        │  Create Claims:                │
        │  - NameIdentifier (User ID)    │
        │  - Name (Full Name)            │
        │  - Email                       │
        │  - MobilePhone                 │
        │  - Role                        │
        └────────────┬───────────────────┘
                     │
                     ▼
        ┌────────────────────────────────┐
        │  Sign with HMACSHA256          │
        │  - Secret Key from config      │
        │  - Issuer validation           │
        │  - Audience validation         │
        │  - Expiration time (60 min)    │
        └────────────┬───────────────────┘
                     │
                     ▼
        ┌────────────────────────────────┐
        │  Return JWT Token              │
        │  eyJhbGciOiJIUzI1NiIsInR5...  │
        └────────────────────────────────┘
```

## Password Security Flow

```
┌─────────────────────────────────────────────────────────────────┐
│                    Password Hashing (Registration)               │
└─────────────────────────────────────────────────────────────────┘
                         │
                         ▼
        ┌────────────────────────────────┐
        │  Plain Text Password           │
        │  "SecurePassword123"           │
        └────────────┬───────────────────┘
                     │
                     ▼
        ┌────────────────────────────────┐
        │  Generate Random Salt          │
        │  HMACSHA512.Key                │
        └────────────┬───────────────────┘
                     │
                     ▼
        ┌────────────────────────────────┐
        │  Compute Hash                  │
        │  HMACSHA512(password + salt)   │
        └────────────┬───────────────────┘
                     │
                     ▼
        ┌────────────────────────────────┐
        │  Store in Database:            │
        │  - PasswordHash (Base64)       │
        │  - PasswordSalt (Base64)       │
        └────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────┐
│                    Password Verification (Login)                 │
└─────────────────────────────────────────────────────────────────┘
                         │
                         ▼
        ┌────────────────────────────────┐
        │  Input Password + Stored Salt  │
        └────────────┬───────────────────┘
                     │
                     ▼
        ┌────────────────────────────────┐
        │  Compute Hash with Same Salt   │
        │  HMACSHA512(password + salt)   │
        └────────────┬───────────────────┘
                     │
                     ▼
        ┌────────────────────────────────┐
        │  Compare with Stored Hash      │
        │  computedHash == storedHash?   │
        └────────────┬───────────────────┘
                     │
                     ▼
        ┌────────────────────────────────┐
        │  Return True/False             │
        └────────────────────────────────┘
```

## Dependency Injection Flow

```
┌─────────────────────────────────────────────────────────────────┐
│                         Program.cs                               │
│                    (Startup Configuration)                       │
└─────────────────────────────────────────────────────────────────┘
                         │
        ┌────────────────┼────────────────┐
        │                │                │
        ▼                ▼                ▼
┌──────────────┐  ┌──────────────┐  ┌──────────────┐
│  MongoDB     │  │  JWT Auth    │  │  Services    │
│  Config      │  │  Config      │  │  & Repos     │
└──────────────┘  └──────────────┘  └──────────────┘
│                │                │
│ Singleton    │ Scoped         │ Scoped
│ IMongoClient │ IMongoDatabase │ IUserRepository
│              │                │ IAuthService
│              │                │
└──────────────┴────────────────┴──────────────┐
                                               │
                                               ▼
                                    ┌──────────────────┐
                                    │  DI Container    │
                                    │  Injects into    │
                                    │  Controllers     │
                                    └──────────────────┘
```

## Project Structure Tree

```
RENTORA.API/
│
├── Controllers/
│   ├── AuthController.cs          ✅ Authentication endpoints
│   └── WeatherForecastController.cs
│
├── Models/
│   ├── BaseEntity.cs              ✅ Base entity with common properties
│   ├── Registration.cs            ✅ User entity
│   ├── Login.cs                   ✅ Login response models
│   ├── BsonCollectionAttribute.cs ✅ MongoDB collection mapping
│   │
│   ├── DTOs/
│   │   ├── LoginDTO.cs           ✅ Login request
│   │   ├── RegistrationDTO.cs    ✅ Registration request
│   │   ├── OtpData.cs            ✅ OTP data model
│   │   └── VerifyOtpRequest.cs   ✅ OTP verification request
│   │
│   ├── Enums/
│   │   └── enum.cs               ✅ Role enumeration
│   │
│   └── MongoDB/
│       ├── MongoDbSettings.cs    ✅ MongoDB configuration
│       └── JwtSettings.cs        ✅ JWT configuration
│
├── Repository/
│   ├── IRepository/
│   │   └── IUserRepository.cs    ✅ User repository interface
│   └── UserRepository.cs         ✅ User repository implementation
│
├── Services/
│   ├── IAuthService.cs           ✅ Auth service interface
│   └── AuthService.cs            ✅ Auth service implementation
│
├── Program.cs                     ✅ Application configuration
├── appsettings.json              ✅ Configuration settings
├── RENTORA.API.csproj            ✅ Project file with packages
│
└── Documentation/
    ├── README.md                  ✅ API documentation
    ├── IMPLEMENTATION_SUMMARY.md  ✅ Implementation details
    └── RENTORA_API.postman_collection.json ✅ Postman collection
```

## Technology Stack

```
┌─────────────────────────────────────────────────────────────────┐
│                      Technology Stack                            │
├─────────────────────────────────────────────────────────────────┤
│  Framework:      ASP.NET Core 8.0                               │
│  Language:       C# 12                                          │
│  Database:       MongoDB 3.5.2                                  │
│  Authentication: JWT Bearer (8.0.0)                             │
│  API Docs:       Swagger/OpenAPI                                │
│  Security:       HMACSHA512 Password Hashing                    │
│  Architecture:   Layered (Controller-Service-Repository)        │
│  Pattern:        Repository Pattern, Dependency Injection       │
└─────────────────────────────────────────────────────────────────┘
```

---
**Architecture designed for scalability, maintainability, and security**
