using AccountingAPI.DTOs;
using AccountingAPI.Models;
using AutoMapper;

namespace AccountingAPI.Configurations
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Tenant
            CreateMap<Tenant, TenantDTO>();
            CreateMap<CreateTenantDTO, Tenant>();
            CreateMap<UpdateTenantDTO, Tenant>();

            // Period
            CreateMap<Period, PeriodDTO>();
            CreateMap<CreatePeriodDTO, Period>();
            CreateMap<UpdatePeriodDTO, Period>();

            //Loan
            CreateMap<Loan, LoanDTO>();
            CreateMap<CreateLoanDTO, Loan>();
            CreateMap<UpdateLoanDTO, Loan>();

            //Business Partner
            CreateMap<BusinessPartner, BusinessPartnerDTO>();
            CreateMap<CreateBusinessPartnerDTO, BusinessPartner>();
            CreateMap<UpdateBusinessPartnerDTO, BusinessPartner>();
            CreateMap<BusinessPartnerDTO, BasicBusinessPartnerDTO>();

            // AP Invoice
            CreateMap<APInvoice, APInvoiceDTO>();
            CreateMap<CreateAPInvoiceDTO, APInvoice>();
            CreateMap<UpdateAPInvoiceDTO, APInvoice>();

            // AP Invoice Line
            CreateMap<APInvoiceLine, APInvoiceLineDTO>();
            CreateMap<CreateAPInvoiceLineDTO, APInvoiceLine>();
            CreateMap<UpdateAPInvoiceLineDTO, APInvoiceLine>();

            // AR Invoice
            CreateMap<ARInvoice, ARInvoiceDTO>();
            CreateMap<CreateARInvoiceDTO, ARInvoice>();
            CreateMap<UpdateARInvoiceDTO, ARInvoice>();

            // AP Invoice Line
            CreateMap<ARInvoiceLine, ARInvoiceLineDTO>();
            CreateMap<CreateARInvoiceLineDTO, ARInvoiceLine>();
            CreateMap<UpdateARInvoiceLineDTO, ARInvoiceLine>();

            // Expense Category
            CreateMap<ExpenseCategory, ExpenseCategoryDTO>();
            CreateMap<CreateExpenseCategoryDTO, ExpenseCategory>();
            CreateMap<UpdateExpenseCategoryDTO, ExpenseCategory>();
            CreateMap<ExpenseCategoryDTO, BasicExpenseCategoryDTO>();

            // Fixed Asset
            CreateMap<FixedAsset, FixedAssetDTO>();
            CreateMap<CreateFixedAssetDTO, FixedAsset>();
            CreateMap<UpdateFixedAssetDTO, FixedAsset>();
            CreateMap<FixedAssetDTO, FixedAssetYearDetailsDTO>();

            // Fixed Asset
            CreateMap<Period, PeriodDTO>();
            CreateMap<CreatePeriodDTO, Period>();
            CreateMap<UpdatePeriodDTO, Period>();

            // Depreciation
            CreateMap<Depreciation, DepreciationDTO>();
            CreateMap<CreateDepreciationDTO, Depreciation>();
            CreateMap<UpdateDepreciationDTO, Depreciation>();

        }
    }
}
