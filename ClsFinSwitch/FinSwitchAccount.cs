using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M4.ClsFinSwitch
{
    public class FinSwitchAccount
    {
        public string InvestorType;
        public string InvestorNumber;
        public string EntityName;
        public string TradingName;
        public string CompanyType;
        public string SpecifyOtherType;
        public string ContactName;
        public string RegistrationNum;
        public string Country;
        public bool IsRegisteredTaxPayer;
        public string IncomeTaxNum;
        public string VATRegNum;
        public string ResAddress;
        public string ResCode;
        public string PhysicalAddress;
        public string PhysicalCode;
        public string HeadAddress;
        public string HeadCode;
        public string PostalAddress;
        public string PostalCode;
        public string EntityTelephoneNum;
        public string EntityFaxNum;
        public string EntityCellNum;
        public string EntityEmail;
        public string EntitySourceFunds;

        public string Rep_Name;
        public string Rep_IDNumber;
        public string Rep_DOB;
        public bool Rep_IsSAResident;
        public bool Rep_IsMale;
        public string Rep_PostAddress;
        public string Rep_PostCode;
        public string Rep_ResidentAddress;
        public string Rep_ResCode;
        public string Rep_HomeTelNum;
        public string Rep_WorkTelNum;
        public string Rep_CelllNum;
        public string Rep_FaxNum;
        public string Rep_Email;
        public string Rep_Capacity;

        public FinSwitchAccount()
        {
            InvestorType = string.Empty;
            InvestorNumber = string.Empty;
            EntityName = string.Empty;
            TradingName = string.Empty;
            CompanyType = string.Empty;
            SpecifyOtherType = string.Empty;
            ContactName = string.Empty;
            RegistrationNum = string.Empty;
            Country = string.Empty;
            IsRegisteredTaxPayer = false;
            IncomeTaxNum = string.Empty;
            ResAddress = string.Empty;
            ResCode = string.Empty;
            PhysicalAddress = string.Empty;
            PhysicalCode = string.Empty;
            HeadAddress = string.Empty;
            HeadCode = string.Empty;
            PostalAddress = string.Empty;
            PostalCode = string.Empty;
            EntityTelephoneNum = string.Empty;
            EntityFaxNum = string.Empty;
            EntityCellNum = string.Empty;
            EntityEmail = string.Empty;
            EntitySourceFunds = string.Empty;

            Rep_Name = string.Empty;
            Rep_IDNumber = string.Empty;
            Rep_DOB = string.Empty;
            Rep_IsSAResident = false;
            Rep_IsMale = false;
            Rep_PostAddress = string.Empty;
            Rep_PostCode = string.Empty;
            Rep_ResidentAddress = string.Empty;
            Rep_ResCode = string.Empty;
            Rep_HomeTelNum = string.Empty;
            Rep_WorkTelNum = string.Empty;
            Rep_CelllNum = string.Empty;
            Rep_FaxNum = string.Empty;
            Rep_Email = string.Empty;
            Rep_Capacity = string.Empty;
        }

    }
}
