using System;
using Bridge;

namespace BOA.Common.Types
{
    [Serializable]
    public class GenericResponse<T> : ResponseBase
    {
        #region Public Properties
        public T Value { get; set; }
        #endregion
    }



    [Serializable]
    public class BranchContract : ContractBase
    {
        public short BranchId
        {
            get;
            set;
        }
        public int ExtendedBranchId
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        public string DisplayName
        {
            get;
            set;
        }
        public string Type
        {
            get;
            set;
        }
        public short Region
        {
            get;
            set;
        }
        public DateTime OfficalReportDate
        {
            get;
            set;
        }
        public DateTime JournalDate
        {
            get;
            set;
        }
        public short City
        {
            get;
            set;
        }
        public int ClearingOffice
        {
            get;
            set;
        }
        public int Customerid
        {
            get;
            set;
        }
        public decimal CaseLimit
        {
            get;
            set;
        }
        public DateTime ReceiptDate
        {
            get;
            set;
        }
        public short Country
        {
            get;
            set;
        }
        public byte OrderinCountry
        {
            get;
            set;
        }
        public string ServerName
        {
            get;
            set;
        }
        public string Channels
        {
            get;
            set;
        }
        public string Category
        {
            get;
            set;
        }
        public short HRBranchId
        {
            get;
            set;
        }
        public string ShortName
        {
            get;
            set;
        }
        public DateTime OpenDate
        {
            get;
            set;
        }
        public DateTime CloseDate
        {
            get;
            set;
        }
        public byte ReginalOffice
        {
            get;
            set;
        }
        public string Voip
        {
            get;
            set;
        }
        public string BranchType
        {
            get;
            set;
        }
        public byte isActive
        {
            get;
            set;
        }
        public string PhoneNumber
        {
            get;
            set;
        }
        public string Address
        {
            get;
            set;
        }
        public string FaxNumber
        {
            get;
            set;
        }
        public short IsInFreeZone
        {
            get;
            set;
        }
        public string Email
        {
            get;
            set;
        }
        public string Latitude
        {
            get;
            set;
        }
        public string Longitude
        {
            get;
            set;
        }
        public override string ToString()
        {
            return this.Name;
        }
    }
}