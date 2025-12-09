# Tenant Login Architecture - No Redundancy Design

## 📋 Overview

This document explains how tenants added by owners can log in to the system without data redundancy between the `Registration` (Users) and `Tenant` collections.

## 🏗️ Architecture Design

### **Two-Collection System**

1. **Registration Collection (Users)**
   - Stores authentication and personal data
   - Fields: Email, Password, Mobile, FullName, Gender, DateOfBirth, Role, etc.
   - Used for login authentication

2. **Tenant Collection**
   - Stores tenant-specific business data
   - Fields: RentAmount, PropertyId, UnitId, AgreementDates, Documents, etc.
   - Linked to Registration via `UserId` field

### **No Data Redundancy**
- Personal data (name, email, mobile, password, gender, DOB) stored **ONLY** in Registration
- Tenant-specific data (rent, property, agreements) stored **ONLY** in Tenant
- Both collections linked via `UserId` and `TenantId` fields

## 🔄 Data Flow

### **When Owner Adds a Tenant:**

```
1. Owner submits TenantCreateDTO with personal + tenant data
   ↓
2. System generates secure random password
   ↓
3. Create User account in Registration collection
   - Email, Mobile, Name, Password (hashed)
   - Role = Tenants
   - OwnerId = Owner's ID
   ↓
4. Create Tenant record in Tenant collection
   - UserId = Created User's ID
   - PropertyId, UnitId, RentAmount, etc.
   ↓
5. Link both records
   - User.TenantId = Tenant.Id
   - Tenant.UserId = User.Id
   ↓
6. Send welcome email with credentials
   - Email: tenant@example.com
   - Password: Auto-generated secure password
```

### **When Tenant Logs In:**

```
1. Tenant enters email/mobile + password
   ↓
2. AuthService authenticates against Registration collection
   ↓
3. If valid, JWT token generated with:
   - UserId
   - Role = Tenants
   - TenantId (from Registration.TenantId)
   ↓
4. Frontend can fetch tenant-specific data using TenantId
```

## 📊 Database Schema

### **Registration (Users) Collection**
```json
{
  "_id": "user123",
  "FullName": "John Doe",
  "Email": "john@example.com",
  "Mobile": "+1234567890",
  "PasswordHash": "hashed_password",
  "PasswordSalt": "salt",
  "Gender": "Male",
  "DateOfBirth": "1990-01-01",
  "Role": 4, // Tenants
  "TenantId": "tenant456",
  "OwnerId": "owner789",
  "IsActive": true
}
```

### **Tenant Collection**
```json
{
  "_id": "tenant456",
  "UserId": "user123",
  "OwnerId": "owner789",
  "PropertyId": "prop001",
  "UnitId": "unit001",
  "PermanentAddress": "123 Main St",
  "CurrentAddress": "456 Oak Ave",
  "RentAmount": 1500.00,
  "SecurityDeposit": 3000.00,
  "AgreementStartDate": "2024-01-01",
  "AgreementEndDate": "2025-01-01",
  "IsActiveTenant": true
}
```

## 🔐 Security Features

1. **Auto-Generated Passwords**
   - 12 characters minimum
   - Includes uppercase, lowercase, numbers, special characters
   - Cryptographically secure random generation

2. **Password Hashing**
   - Uses PBKDF2 with salt
   - Stored as Base64 strings in database

3. **Email Notification**
   - Welcome email sent with credentials
   - Uses email template: `TenantsRegistration`
   - Includes login URL and temporary password

## 📧 Email Template Tokens

The `TenantsRegistration` email template should include these tokens:

- `{{TenantName}}` - Full name of the tenant
- `{{Email}}` - Login email address
- `{{Password}}` - Auto-generated password
- `{{PropertyId}}` - Property they're assigned to
- `{{LoginUrl}}` - Link to login page

## 🔧 API Endpoints

### **Create Tenant (POST /api/tenants)**

**Request:**
```json
{
  "ownerId": "owner789",
  "propertyId": "prop001",
  "unitId": "unit001",
  "firstName": "John",
  "lastName": "Doe",
  "mobile": "+1234567890",
  "email": "john@example.com",
  "gender": "Male",
  "dateOfBirth": "1990-01-01",
  "permanentAddress": "123 Main St",
  "rentAmount": 1500.00,
  "agreementStartDate": "2024-01-01",
  "agreementEndDate": "2025-01-01",
  "createdBy": "owner789"
}
```

**Response:**
```json
{
  "success": true,
  "status": 201,
  "message": "Tenant created successfully. Login credentials sent to email.",
  "data": {
    "tenant": { ... },
    "user": {
      "id": "user123",
      "email": "john@example.com",
      "mobile": "+1234567890",
      "fullName": "John Doe"
    },
    "temporaryPassword": "Abc123!@#Xyz" // Remove in production
  }
}
```

### **Update Tenant (PUT /api/tenants/{id})**

Updates both Registration and Tenant records to maintain data consistency.

## 🎯 Benefits of This Architecture

1. ✅ **No Data Redundancy** - Each field exists in only one collection
2. ✅ **Single Source of Truth** - Personal data in Registration, business data in Tenant
3. ✅ **Easy Authentication** - Standard login flow using Registration collection
4. ✅ **Proper Normalization** - Follows database best practices
5. ✅ **Scalable** - Easy to add more roles (Agents, Managers) using same pattern
6. ✅ **Maintainable** - Updates to personal info only need to touch Registration
7. ✅ **Secure** - Auto-generated passwords, proper hashing, email delivery

## 🔄 Update Flow

When updating a tenant:
1. Update personal fields (name, email, mobile) in **Registration** collection
2. Update tenant-specific fields (rent, property) in **Tenant** collection
3. Both updates happen in same transaction for consistency

## 🚨 Important Notes

1. **Password Changes** - Should go through separate "Change Password" flow
2. **Email Template** - Ensure `TenantsRegistration` template exists in database
3. **Login URL** - Update the hardcoded URL in TenantsController.cs line 245
4. **Production** - Remove `TemporaryPassword` from API response in production
5. **Validation** - Email/Mobile uniqueness checked in Registration collection

## 📝 Files Modified

1. `Models/Tenant.cs` - Removed redundant fields, added UserId
2. `Models/DTOs/TenantDTO.cs` - Updated DTOs to include personal fields for creation
3. `Controllers/TenantsController.cs` - Dual-record creation and update
4. `Repository/TenantsRepository.cs` - Removed email/mobile existence checks
5. `Helpers/PasswordGenerator.cs` - New utility for secure password generation

## 🎓 Usage Example

```csharp
// Owner creates tenant
var createDTO = new TenantCreateDTO
{
    FirstName = "John",
    LastName = "Doe",
    Email = "john@example.com",
    Mobile = "+1234567890",
    // ... other fields
};

// System automatically:
// 1. Generates password: "Xy7!mK9@pL2#"
// 2. Creates user account with Role.Tenants
// 3. Creates tenant record linked to user
// 4. Sends email: "Your login: john@example.com, Password: Xy7!mK9@pL2#"

// Tenant can now login
var loginDTO = new LoginDTO
{
    EmailOrMobile = "john@example.com",
    Password = "Xy7!mK9@pL2#"
};
// Returns JWT token with tenant access
```

---

**Last Updated:** December 9, 2025  
**Version:** 1.0  
**Author:** Rentora Development Team
