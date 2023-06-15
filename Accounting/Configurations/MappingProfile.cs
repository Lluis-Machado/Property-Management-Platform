using AccountingAPI.Models;
using AccountingAPI.DTOs;
using AutoMapper;

namespace AccountingAPI.Configurations
{
    public class MappingProfile :Profile
    {
        public MappingProfile()
        {
            // Tenant
            CreateMap<Tenant, TenantDTO>();
            CreateMap<CreateTenantDTO, Tenant>();

            //Business Partner
            CreateMap<BusinessPartner, BusinessPartnerDTO>();
            CreateMap<CreateBusinessPartnerDTO, BusinessPartner>();

            // Invoice
            CreateMap<Invoice, InvoiceDTO>();
            CreateMap<InvoiceDTO, Invoice>();
            CreateMap<CreateInvoiceDTO, Invoice>();

            // Invoice Line
            CreateMap<InvoiceLine, InvoiceLineDTO>();
            CreateMap<CreateInvoiceLineDTO, InvoiceLine>();
            CreateMap<CreateInvoiceLineDTO, UpdateInvoiceLineDTO>();
            CreateMap<UpdateInvoiceLineDTO, InvoiceLine>();

            // Expense Category
            CreateMap<ExpenseCategory, ExpenseCategoryDTO>();
            CreateMap<CreateExpenseCategoryDTO, ExpenseCategory>();

            // Fixed Asset
            CreateMap<FixedAsset,FixedAssetDTO>();
            CreateMap<UpdateFixedAssetDTO, FixedAsset>();
            CreateMap<CreateFixedAssetDTO, FixedAsset>();

            //Loan
            CreateMap<Loan, LoanDTO>();
            CreateMap<CreateLoanDTO, Loan>();
        }
    }
}
