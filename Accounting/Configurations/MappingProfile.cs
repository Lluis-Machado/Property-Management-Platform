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

            // AP Invoice
            CreateMap<APInvoice, APInvoiceDTO>();
            CreateMap<APInvoiceDTO, APInvoice>();
            CreateMap<CreateAPInvoiceDTO, APInvoice>();

            // AP Invoice Line
            CreateMap<APInvoiceLine, APInvoiceLineDTO>();
            CreateMap<CreateAPInvoiceLineDTO, APInvoiceLine>();
            CreateMap<CreateAPInvoiceLineDTO, UpdateAPInvoiceLineDTO>();
            CreateMap<UpdateAPInvoiceLineDTO, APInvoiceLine>();

            // AR Invoice
            CreateMap<ARInvoice, ARInvoiceDTO>();
            CreateMap<ARInvoiceDTO, ARInvoice>();
            CreateMap<CreateARInvoiceDTO, ARInvoice>();

            // AP Invoice Line
            CreateMap<ARInvoiceLine, ARInvoiceLineDTO>();
            CreateMap<CreateARInvoiceLineDTO, ARInvoiceLine>();
            CreateMap<CreateARInvoiceLineDTO, UpdateARInvoiceLineDTO>();
            CreateMap<UpdateARInvoiceLineDTO, ARInvoiceLine>();

            // Expense Category
            CreateMap<ExpenseCategory, ExpenseCategoryDTO>();
            CreateMap<CreateExpenseCategoryDTO, ExpenseCategory>();

            // Fixed Asset
            CreateMap<FixedAsset,FixedAssetDTO>();
            CreateMap<UpdateFixedAssetDTO, FixedAsset>();
            CreateMap<CreateFixedAssetDTO, FixedAsset>();

            // Fixed Asset
            CreateMap<Period, PeriodDTO>();
            CreateMap<UpdatePeriodDTO, Period>();
            CreateMap<CreatePeriodDTO, Period>();

            //Loan
            CreateMap<Loan, LoanDTO>();
            CreateMap<CreateLoanDTO, Loan>();
        }
    }
}
