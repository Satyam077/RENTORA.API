# 🎨 Tenant Login System - Visual Architecture

## 📊 System Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────────────┐
│                         RENTORA TENANT SYSTEM                            │
└─────────────────────────────────────────────────────────────────────────┘

┌──────────────┐                                          ┌──────────────┐
│              │                                          │              │
│   OWNER      │                                          │   TENANT     │
│   (Web UI)   │                                          │   (Web UI)   │
│              │                                          │              │
└──────┬───────┘                                          └──────┬───────┘
       │                                                         │
       │ 1. Add Tenant                                          │ 5. Login
       │    (POST /api/tenants)                                 │    (POST /api/auth/login)
       │                                                         │
       ▼                                                         ▼
┌─────────────────────────────────────────────────────────────────────────┐
│                          API LAYER (C# .NET)                             │
│  ┌────────────────────┐              ┌────────────────────┐             │
│  │ TenantsController  │              │  AuthController    │             │
│  │                    │              │                    │             │
│  │ - CreateTenant()   │              │ - Login()          │             │
│  │ - UpdateTenant()   │              │ - Register()       │             │
│  │ - GetTenant()      │              │ - SendOtp()        │             │
│  └────────┬───────────┘              └──────────┬─────────┘             │
│           │                                     │                        │
│           │ 2. Generate Password                │                        │
│           │    "Xy7!mK9@pL2#"                   │                        │
│           │                                     │                        │
│           ▼                                     ▼                        │
│  ┌─────────────────────────────────────────────────────────┐            │
│  │            BUSINESS LOGIC LAYER                          │            │
│  │  ┌──────────────┐    ┌──────────────┐   ┌────────────┐ │            │
│  │  │ UserRepo     │    │ TenantRepo   │   │ EmailSvc   │ │            │
│  │  │              │    │              │   │            │ │            │
│  │  └──────┬───────┘    └──────┬───────┘   └─────┬──────┘ │            │
│  └─────────┼────────────────────┼──────────────────┼────────┘            │
└────────────┼────────────────────┼──────────────────┼─────────────────────┘
             │                    │                  │
             │ 3. Create User     │ 4. Create Tenant │ 6. Send Email
             │                    │                  │
             ▼                    ▼                  ▼
┌─────────────────────────────────────────────────────────────────────────┐
│                        DATA LAYER (MongoDB)                              │
│                                                                          │
│  ┌──────────────────────────────┐    ┌──────────────────────────────┐  │
│  │  REGISTRATION (Users)        │    │  TENANTS                     │  │
│  │  Collection                  │    │  Collection                  │  │
│  ├──────────────────────────────┤    ├──────────────────────────────┤  │
│  │ _id: "user_xyz789"           │◄──►│ _id: "tenant_abc123"         │  │
│  │ fullName: "John Doe"         │    │ userId: "user_xyz789"        │  │
│  │ email: "john@example.com"    │    │ ownerId: "owner123"          │  │
│  │ mobile: "+1234567890"        │    │ propertyId: "prop456"        │  │
│  │ passwordHash: "hashed..."    │    │ unitId: "unit789"            │  │
│  │ passwordSalt: "salt..."      │    │ rentAmount: 1500.00          │  │
│  │ gender: "Male"               │    │ securityDeposit: 3000.00     │  │
│  │ dateOfBirth: "1990-05-15"    │    │ agreementStartDate: "..."    │  │
│  │ role: 4 (Tenants)            │    │ agreementEndDate: "..."      │  │
│  │ tenantId: "tenant_abc123"    │    │ permanentAddress: "..."      │  │
│  │ ownerId: "owner123"          │    │ currentAddress: "..."        │  │
│  │ isActive: true               │    │ documents: [...]             │  │
│  │ createdAt: "2024-12-09..."   │    │ idProofType: "Aadhaar"       │  │
│  └──────────────────────────────┘    │ idProofNumber: "1234..."     │  │
│                                      │ isActiveTenant: true         │  │
│                                      │ isMovedOut: false            │  │
│                                      │ createdAt: "2024-12-09..."   │  │
│                                      └──────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────────────┘
                                      │
                                      │ 7. Email Delivered
                                      ▼
                              ┌───────────────┐
                              │   SendGrid    │
                              │   Email API   │
                              └───────┬───────┘
                                      │
                                      ▼
                              ┌───────────────┐
                              │  Tenant's     │
                              │  Email Inbox  │
                              │               │
                              │ 📧 Welcome!   │
                              │ Login:        │
                              │ john@...      │
                              │ Pass: Xy7!... │
                              └───────────────┘
```

## 🔄 Data Flow Sequence

```
STEP 1: Owner Adds Tenant
═══════════════════════════
Owner → Frontend → POST /api/tenants → TenantsController
                                              │
                                              ├─ Validate Input
                                              ├─ Check Email Exists (UserRepo)
                                              ├─ Check Mobile Exists (UserRepo)
                                              └─ All Valid ✓

STEP 2: Generate Password
═══════════════════════════
TenantsController → PasswordGenerator.GenerateSecurePassword()
                                              │
                                              └─ Returns: "Xy7!mK9@pL2#"

STEP 3: Create User Account
═══════════════════════════
TenantsController → UserRepository.CreateUserAsync()
                                              │
                                              ├─ Hash Password (PBKDF2)
                                              ├─ Set Role = Tenants
                                              ├─ Set OwnerId
                                              └─ Insert into Registration Collection
                                                        │
                                                        └─ Returns: User Object

STEP 4: Create Tenant Record
═══════════════════════════
TenantsController → TenantsRepository.CreateAsync()
                                              │
                                              ├─ Set UserId (link to User)
                                              ├─ Set Property/Unit/Rent data
                                              └─ Insert into Tenants Collection
                                                        │
                                                        └─ Returns: Tenant Object

STEP 5: Link Records
═══════════════════════════
TenantsController → UserRepository.UpdateUserAsync()
                                              │
                                              └─ Set User.TenantId = Tenant.Id

STEP 6: Send Email
═══════════════════════════
TenantsController → EmailService.SendTemplateEmailAsync()
                                              │
                                              ├─ Get Template: "TenantsRegistration"
                                              ├─ Replace Tokens (Name, Email, Password)
                                              └─ SendGrid API → Tenant's Email

STEP 7: Tenant Receives Email
═══════════════════════════
Tenant Inbox ← Email with Login Credentials

STEP 8: Tenant Logs In
═══════════════════════════
Tenant → Frontend → POST /api/auth/login → AuthController
                                              │
                                              ├─ Find User by Email
                                              ├─ Verify Password Hash
                                              ├─ Check IsActive
                                              └─ Generate JWT Token
                                                        │
                                                        └─ Returns: Token + User Info

STEP 9: Access Tenant Portal
═══════════════════════════
Tenant → Frontend (with JWT) → GET /api/tenants/{id}
                                              │
                                              └─ Returns: Tenant Data
```

## 🗂️ Field Distribution (No Redundancy)

```
┌─────────────────────────────────────────────────────────────────┐
│                    FIELD DISTRIBUTION                            │
└─────────────────────────────────────────────────────────────────┘

REGISTRATION (Users) Collection          TENANT Collection
┌────────────────────────────┐          ┌────────────────────────────┐
│ AUTHENTICATION & PERSONAL  │          │ TENANT BUSINESS DATA       │
├────────────────────────────┤          ├────────────────────────────┤
│ ✓ Email                    │          │ ✓ PropertyId               │
│ ✓ Mobile                   │          │ ✓ UnitId                   │
│ ✓ PasswordHash             │          │ ✓ OwnerId                  │
│ ✓ PasswordSalt             │          │ ✓ UserId (link)            │
│ ✓ FullName                 │          │ ✓ PermanentAddress         │
│ ✓ Gender                   │          │ ✓ CurrentAddress           │
│ ✓ DateOfBirth              │          │ ✓ RentAmount               │
│ ✓ Role (Tenants)           │          │ ✓ SecurityDeposit          │
│ ✓ TenantId (link)          │          │ ✓ RentDueDay               │
│ ✓ OwnerId                  │          │ ✓ AgreementStartDate       │
│ ✓ IsEmailVerified          │          │ ✓ AgreementEndDate         │
│ ✓ IsMobileVerified         │          │ ✓ IsAgreementExpired       │
│ ✓ IsActive                 │          │ ✓ Documents[]              │
│ ✓ ProfileImageUrl          │          │ ✓ IdProofType              │
│                            │          │ ✓ IdProofNumber            │
│                            │          │ ✓ IsActiveTenant           │
│                            │          │ ✓ IsRentPending            │
│                            │          │ ✓ IsMovedOut               │
│                            │          │ ✓ MoveInDate               │
│                            │          │ ✓ MoveOutDate              │
│                            │          │ ✓ Notes                    │
└────────────────────────────┘          └────────────────────────────┘
         ▲                                           ▲
         │                                           │
         └───────────── Linked via IDs ──────────────┘
                UserId ↔ TenantId
```

## 🔐 Password Generation Flow

```
┌────────────────────────────────────────────────────────────┐
│              SECURE PASSWORD GENERATION                     │
└────────────────────────────────────────────────────────────┘

PasswordGenerator.GenerateSecurePassword(12, true)
         │
         ├─ Step 1: Define Character Sets
         │          ├─ Lowercase: a-z
         │          ├─ Uppercase: A-Z
         │          ├─ Digits: 0-9
         │          └─ Special: !@#$%^&*
         │
         ├─ Step 2: Ensure Diversity
         │          ├─ Pick 1 lowercase
         │          ├─ Pick 1 uppercase
         │          ├─ Pick 1 digit
         │          └─ Pick 1 special char
         │
         ├─ Step 3: Fill Remaining (8 chars)
         │          └─ Random from all sets
         │
         ├─ Step 4: Cryptographic Shuffle
         │          └─ RandomNumberGenerator.Create()
         │
         └─ Result: "Xy7!mK9@pL2#"
                    │
                    ├─ Length: 12 characters ✓
                    ├─ Has Uppercase: X, K, L ✓
                    ├─ Has Lowercase: y, m, p ✓
                    ├─ Has Digits: 7, 9, 2 ✓
                    └─ Has Special: !, @, # ✓

Then:
PasswordHelper.CreatePasswordHash("Xy7!mK9@pL2#", out hash, out salt)
         │
         ├─ Algorithm: PBKDF2
         ├─ Iterations: 10000
         ├─ Hash Length: 256 bits
         │
         └─ Stored in DB:
            ├─ PasswordHash: "base64_encoded_hash"
            └─ PasswordSalt: "base64_encoded_salt"
```

## 📧 Email Template Flow

```
┌────────────────────────────────────────────────────────────┐
│                EMAIL TEMPLATE PROCESSING                    │
└────────────────────────────────────────────────────────────┘

EmailService.SendTemplateEmailAsync(...)
         │
         ├─ Step 1: Fetch Template from DB
         │          └─ Template Name: "TenantsRegistration"
         │
         ├─ Step 2: Prepare Tokens
         │          ├─ {{TenantName}} → "John Doe"
         │          ├─ {{Email}} → "john@example.com"
         │          ├─ {{Password}} → "Xy7!mK9@pL2#"
         │          ├─ {{PropertyId}} → "prop456"
         │          └─ {{LoginUrl}} → "https://app.com/login"
         │
         ├─ Step 3: Replace Tokens in HTML
         │          └─ "Hello {{TenantName}}" → "Hello John Doe"
         │
         ├─ Step 4: Send via SendGrid
         │          ├─ From: "noreply@rentora.com"
         │          ├─ To: "john@example.com"
         │          ├─ Subject: "Welcome to Rentora..."
         │          └─ Body: HTML with credentials
         │
         └─ Step 5: Delivery
                    └─ Tenant receives email ✓
```

## 🎯 Update Operation Flow

```
┌────────────────────────────────────────────────────────────┐
│              UPDATE TENANT OPERATION                        │
└────────────────────────────────────────────────────────────┘

PUT /api/tenants/{id}
         │
         ├─ Step 1: Validate Input
         │
         ├─ Step 2: Fetch Existing Tenant
         │          └─ TenantsRepository.GetByIdAsync(id)
         │
         ├─ Step 3: Fetch Linked User
         │          └─ UserRepository.GetByIdAsync(tenant.UserId)
         │
         ├─ Step 4: Update User (Personal Data)
         │          ├─ FullName = "Jane Doe"
         │          ├─ Email = "jane@example.com"
         │          ├─ Mobile = "+0987654321"
         │          ├─ Gender = "Female"
         │          └─ DateOfBirth = "1992-03-20"
         │
         ├─ Step 5: Update Tenant (Business Data)
         │          ├─ RentAmount = 1600.00
         │          ├─ PropertyId = "prop789"
         │          ├─ AgreementEndDate = "2026-01-01"
         │          └─ Notes = "Updated notes"
         │
         └─ Step 6: Return Both Updated Records
                    ├─ User: { id, email, mobile, fullName }
                    └─ Tenant: { id, userId, rentAmount, ... }
```

---

**Visual Guide Version:** 1.0  
**Created:** December 9, 2025  
**Purpose:** Understanding Tenant Login Architecture
