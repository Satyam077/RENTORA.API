# 🚀 Tenant API Quick Reference

## 📌 Create Tenant (Owner Action)

### Endpoint
```
POST /api/tenants
Authorization: Bearer {owner_token}
```

### Request Body
```json
{
  "ownerId": "owner123",
  "propertyId": "prop456",
  "unitId": "unit789",
  
  // Personal Information (stored in Registration)
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "mobile": "+1234567890",
  "gender": "Male",
  "fatherName": "James Doe",
  "dateOfBirth": "1990-05-15",
  
  // Tenant-Specific Information (stored in Tenant)
  "permanentAddress": "123 Main Street, City, State 12345",
  "currentAddress": "456 Oak Avenue, Apt 2B, City, State 12345",
  "rentAmount": 1500.00,
  "securityDeposit": 3000.00,
  "rentDueDay": 5,
  "agreementStartDate": "2024-01-01",
  "agreementEndDate": "2025-01-01",
  "documents": [
    "https://storage.com/aadhar.pdf",
    "https://storage.com/pan.pdf"
  ],
  "idProofType": "Aadhaar",
  "idProofNumber": "1234-5678-9012",
  "moveInDate": "2024-01-01",
  "notes": "Preferred tenant, no pets",
  "createdBy": "owner123"
}
```

### Success Response (201 Created)
```json
{
  "success": true,
  "status": 201,
  "message": "Tenant created successfully. Login credentials sent to email.",
  "data": {
    "tenant": {
      "id": "tenant_abc123",
      "userId": "user_xyz789",
      "ownerId": "owner123",
      "propertyId": "prop456",
      "unitId": "unit789",
      "permanentAddress": "123 Main Street...",
      "currentAddress": "456 Oak Avenue...",
      "rentAmount": 1500.00,
      "securityDeposit": 3000.00,
      "rentDueDay": 5,
      "agreementStartDate": "2024-01-01T00:00:00Z",
      "agreementEndDate": "2025-01-01T00:00:00Z",
      "isActiveTenant": true,
      "isMovedOut": false,
      "createdAt": "2024-12-09T18:00:00Z"
    },
    "user": {
      "id": "user_xyz789",
      "email": "john.doe@example.com",
      "mobile": "+1234567890",
      "fullName": "John Doe"
    },
    "temporaryPassword": "Xy7!mK9@pL2#"  // REMOVE IN PRODUCTION
  }
}
```

### Error Responses

**400 Bad Request - Email Exists**
```json
{
  "success": false,
  "status": 400,
  "message": "User with email 'john.doe@example.com' already exists"
}
```

**400 Bad Request - Mobile Exists**
```json
{
  "success": false,
  "status": 400,
  "message": "User with mobile '+1234567890' already exists"
}
```

**400 Bad Request - Validation Error**
```json
{
  "success": false,
  "status": 400,
  "message": "Invalid tenant data",
  "data": {
    "Email": ["The Email field is required."],
    "RentAmount": ["The RentAmount field is required."]
  }
}
```

---

## 📝 Update Tenant

### Endpoint
```
PUT /api/tenants/{id}
Authorization: Bearer {owner_token}
```

### Request Body
```json
{
  "id": "tenant_abc123",
  "ownerId": "owner123",
  "propertyId": "prop456",
  "unitId": "unit789",
  
  // Personal Information (updates Registration)
  "firstName": "Jane",
  "lastName": "Doe",
  "email": "jane.doe@example.com",
  "mobile": "+1234567890",
  "gender": "Female",
  "fatherName": "James Doe",
  "dateOfBirth": "1990-05-15",
  
  // Tenant-Specific Information (updates Tenant)
  "permanentAddress": "123 Main Street...",
  "currentAddress": "456 Oak Avenue...",
  "rentAmount": 1600.00,
  "securityDeposit": 3200.00,
  "rentDueDay": 5,
  "agreementStartDate": "2024-01-01",
  "agreementEndDate": "2025-01-01",
  "isAgreementExpired": false,
  "documents": ["https://storage.com/aadhar.pdf"],
  "idProofType": "Aadhaar",
  "idProofNumber": "1234-5678-9012",
  "isActiveTenant": true,
  "isRentPending": false,
  "isMovedOut": false,
  "moveInDate": "2024-01-01",
  "moveOutDate": null,
  "notes": "Updated notes",
  "isActive": true,
  "updatedBy": "owner123"
}
```

### Success Response (200 OK)
```json
{
  "success": true,
  "status": 200,
  "message": "Tenant updated successfully",
  "data": {
    "tenant": { ... },
    "user": {
      "id": "user_xyz789",
      "email": "jane.doe@example.com",
      "mobile": "+1234567890",
      "fullName": "Jane Doe"
    }
  }
}
```

---

## 🔍 Get Tenant by ID

### Endpoint
```
GET /api/tenants/{id}
Authorization: Bearer {token}
```

### Success Response (200 OK)
```json
{
  "success": true,
  "status": 200,
  "message": "Tenant retrieved successfully",
  "data": {
    "id": "tenant_abc123",
    "userId": "user_xyz789",
    "ownerId": "owner123",
    "propertyId": "prop456",
    "unitId": "unit789",
    "permanentAddress": "123 Main Street...",
    "rentAmount": 1500.00,
    // ... other tenant fields
  }
}
```

**Note:** To get personal information (name, email, mobile), make a separate call to `/api/users/{userId}`

---

## 📋 Get All Tenants by Owner

### Endpoint
```
GET /api/tenants/owner/{ownerId}
Authorization: Bearer {owner_token}
```

### Success Response (200 OK)
```json
{
  "success": true,
  "status": 200,
  "message": "Tenants retrieved successfully",
  "data": [
    {
      "id": "tenant_abc123",
      "userId": "user_xyz789",
      "ownerId": "owner123",
      "propertyId": "prop456",
      "rentAmount": 1500.00,
      // ... other fields
    },
    {
      "id": "tenant_def456",
      "userId": "user_uvw012",
      "ownerId": "owner123",
      "propertyId": "prop789",
      "rentAmount": 1800.00,
      // ... other fields
    }
  ]
}
```

---

## 🏢 Get Tenants by Property

### Endpoint
```
GET /api/tenants/property/{propertyId}
Authorization: Bearer {token}
```

---

## 🏠 Get Tenants by Unit

### Endpoint
```
GET /api/tenants/unit/{unitId}
Authorization: Bearer {token}
```

---

## 🗑️ Delete Tenant

### Endpoint
```
DELETE /api/tenants/{id}
Authorization: Bearer {owner_token}
```

### Success Response (200 OK)
```json
{
  "success": true,
  "status": 200,
  "message": "Tenant deleted successfully"
}
```

**Note:** This only deletes the Tenant record. Consider also deleting or deactivating the User account.

---

## 🔐 Tenant Login

### Endpoint
```
POST /api/auth/login
```

### Request Body
```json
{
  "emailOrMobile": "john.doe@example.com",
  "password": "Xy7!mK9@pL2#"
}
```

### Success Response (200 OK)
```json
{
  "success": true,
  "message": "Login successful",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": "user_xyz789",
    "fullName": "John Doe",
    "email": "john.doe@example.com",
    "mobile": "+1234567890",
    "role": 4,  // Tenants
    "profileImageUrl": null,
    "isEmailVerified": false,
    "isMobileVerified": false
  }
}
```

### Error Response (401 Unauthorized)
```json
{
  "success": false,
  "message": "Invalid credentials"
}
```

---

## 🔄 Data Relationship

```
Registration (Users) Collection          Tenant Collection
┌─────────────────────────┐             ┌─────────────────────────┐
│ id: "user_xyz789"       │◄───────────►│ id: "tenant_abc123"     │
│ email: "john@..."       │             │ userId: "user_xyz789"   │
│ mobile: "+123..."       │             │ propertyId: "prop456"   │
│ fullName: "John Doe"    │             │ rentAmount: 1500        │
│ password: "hashed..."   │             │ agreementStartDate: ... │
│ role: Tenants (4)       │             │ securityDeposit: 3000   │
│ tenantId: "tenant_..."  │             │ isActiveTenant: true    │
└─────────────────────────┘             └─────────────────────────┘
```

---

## 📧 Email Sent to Tenant

**Subject:** Welcome to Rentora - Your Account is Ready

**Content:**
- Tenant name
- Login email
- Temporary password
- Property ID
- Login URL
- Security warnings
- Call-to-action button

**Template Tokens:**
- `{{TenantName}}` - John Doe
- `{{Email}}` - john.doe@example.com
- `{{Password}}` - Xy7!mK9@pL2#
- `{{PropertyId}}` - prop456
- `{{LoginUrl}}` - https://yourapp.com/login

---

## ⚙️ Configuration Required

### 1. Email Template in Database
```json
{
  "templateName": "TenantsRegistration",
  "emailSubject": "Welcome to Rentora - Your Account is Ready",
  "emailBody": "<html>... (use TenantsRegistration.html) ...</html>",
  "applicableFor": "Tenants",
  "isActive": true
}
```

### 2. Update Login URL
File: `TenantsController.cs` line ~245
```csharp
{ "LoginUrl", "https://your-actual-domain.com/login" }
```

### 3. SendGrid Configuration
File: `appsettings.json`
```json
{
  "EmailSettings": {
    "ApiKey": "your-sendgrid-api-key",
    "From": "noreply@yourapp.com",
    "FromName": "Rentora"
  }
}
```

---

## 🧪 Testing with Postman/Curl

### Create Tenant
```bash
curl -X POST https://api.yourapp.com/api/tenants \
  -H "Authorization: Bearer {owner_token}" \
  -H "Content-Type: application/json" \
  -d '{
    "ownerId": "owner123",
    "propertyId": "prop456",
    "unitId": "unit789",
    "firstName": "John",
    "lastName": "Doe",
    "email": "john@test.com",
    "mobile": "+1234567890",
    "rentAmount": 1500,
    "agreementStartDate": "2024-01-01",
    "agreementEndDate": "2025-01-01",
    "createdBy": "owner123"
  }'
```

### Tenant Login
```bash
curl -X POST https://api.yourapp.com/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "emailOrMobile": "john@test.com",
    "password": "Xy7!mK9@pL2#"
  }'
```

---

## 🎯 Key Points

1. ✅ **No Password in CreateDTO** - Auto-generated by system
2. ✅ **Email Sent Automatically** - Tenant receives credentials
3. ✅ **Two Collections Updated** - Registration + Tenant
4. ✅ **Bidirectional Link** - User.TenantId ↔ Tenant.UserId
5. ✅ **Standard Login** - Uses existing auth endpoint
6. ✅ **Role-Based Access** - Role.Tenants (4)
7. ✅ **Secure Passwords** - 12 chars, cryptographically random

---

**Last Updated:** December 9, 2025  
**API Version:** 1.0
