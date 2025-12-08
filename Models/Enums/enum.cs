using System.ComponentModel.DataAnnotations;

namespace RENTORA.API.Models.Enums
{
    public enum Role
    {
        SuperAdmin = 1,
        Admin = 2,
        Landlords = 3,
        Tenants = 4,
        Agents = 5,
        Manager = 6

    }
    public enum ApplicableFor
    {
        [Display(Name = "Super Admin")]
        SuperAdmin = 1,
        [Display(Name = "Admin")]
        Admin = 2,
        [Display(Name = "Landlords")]
        Landlords = 3,
        [Display(Name = "Tenants")]
        Tenants = 4,
        [Display(Name = "Agents")]
        Agents = 5,
        [Display(Name = "Manager")]
        Manager = 6
    }

    public enum EmailTemplateName
    {
        [Display(Name = "Super Admin Registration")]
        SuperAdminRegistration,

        [Display(Name = "Admin Registration")]
        AdminRegistration,

        [Display(Name = "Landlords Registration")]
        LandlordsRegistration,

        [Display(Name = "Agents Registration")]
        AgentRegistration,

        [Display(Name = "Tenants Registration")]
        TenantsRegistration,

        [Display(Name = "User Login Otp")]
        UserLoginOtp,

        [Display(Name = "Helpdesk Query")]
        HelpdeskQuery,
    }
    public enum PropertyType
    {
        Apartment = 1,
        IndependentHouse = 2,
        PG = 3,
        CommercialShop = 4,
        OfficeSpace = 5,
        Warehouse = 6,
        Land = 7,
        Other = 8

    }
    public enum IdProofType
    {
        Aadhaar = 1,
        PAN = 2,
        Passport = 3,
        DrivingLicense = 4 ,
        VoterId = 5,
        Other = 6
    }
}

