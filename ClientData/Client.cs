using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M4.ClientData
{
    [Serializable]
    public class Client
    {
        private string _clientNumber;
        public string ClientNumber
        {
            get { return _clientNumber; }
            set { _clientNumber = value; }
        }
        private string _firstName;
        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }
        private string _middleName;
        public string MiddleName
        {
            get { return _middleName; }
            set { _middleName = value; }
        }
        private string _lastname;
        public string Lastname
        {
            get { return _lastname; }
            set { _lastname = value; }
        }
        private string _company;
        public string Company
        {
            get { return _company; }
            set { _company = value; }
        }
        private string _namePreference;
        public string NamePreference
        {
            get { return _namePreference; }
            set { _namePreference = value; }
        }
        private string _address1;
        public string Address1
        {
            get { return _address1; }
            set { _address1 = value; }
        }
        private string _address2;
        public string Address2
        {
            get { return _address2; }
            set { _address2 = value; }
        }
        private string _state;
        public string State
        {
            get { return _state; }
            set { _state = value; }
        }
        private string _zipCode;
        public string ZipCode
        {
            get { return _zipCode; }
            set { _zipCode = value; }
        }
        private string _country;
        public string Country
        {
            get { return _country; }
            set { _country = value; }
        }
        private string _workPhone;
        public string WorkPhone
        {
            get { return _workPhone; }
            set { _workPhone = value; }
        }
        private string _homePhone;
        public string HomePhone
        {
            get { return _homePhone; }
            set { _homePhone = value; }
        }
        private string _cellPhone;
        public string CellPhone
        {
            get { return _cellPhone; }
            set { _cellPhone = value; }
        }
        private string _fax;
        public string Fax
        {
            get { return _fax; }
            set { _fax = value; }
        }
        private string _email;
        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }
        private string _contactMethod;
        public string ContactMethod
        {
            get { return _contactMethod; }
            set { _contactMethod = value; }
        }
        private string _city;
        public string City
        {
            get { return _city; }
            set { _city = value; }
        }
        private string _notes;
        public string Notes
        {
            get { return _notes; }
            set { _notes = value; }
        }

        private string _RealAccount;
        public string RealAccount
        {
            get { return _RealAccount; }
            set { _RealAccount = value; }
        }
        private List<Symbol> _SymbolList;
        public List<Symbol> SymbolList
        {
            get { return _SymbolList; }
            set { _SymbolList = value; }
        }

        private DateTime dateofbirth;
        public DateTime Dateofbirth
        {
            get { return dateofbirth; }
            set { dateofbirth = value; }
        }      

       
        private string _clientGroup;
        public string ClientGroup
        {
            get { return _clientGroup; }
            set { _clientGroup = value; }
        }
        private List<Account> _accountList;
        public List<Account> AccountList
        {
            get { return _accountList; }
            set { _accountList = value; }
        }

        private string _clientName;
        public string ClientName
        {
            get { return _clientName; }
            set { _clientName = value; }
        }

        public RiskProfile _RiskProfile;
        public Client()
        {
            _clientNumber = string.Empty;
            _firstName = string.Empty;
            _middleName = string.Empty;
            _lastname = string.Empty;
            _company = string.Empty;
            _namePreference = string.Empty;
            _address1 = string.Empty;
            _address2 = string.Empty;
            _state = string.Empty;
            _zipCode = string.Empty;
            _country = string.Empty;
            _workPhone = string.Empty;
            _homePhone = string.Empty;
            _cellPhone = string.Empty;
            _fax = string.Empty;
            _email = string.Empty;           
            _contactMethod = string.Empty;            
            _city = string.Empty;
            _notes = string.Empty;
            _clientGroup = string.Empty;
            AccountList = new List<Account>();
            SymbolList = new List<Symbol>();
            Dateofbirth = DateTime.Now;
            _RiskProfile = new RiskProfile();
        }
        public void AddAccount(Account account, long AccNum)
        {            
            account.AccountNumber = AccNum;
            account.InvestmentNumber = AccNum + 100;
            if (AccountList.FirstOrDefault(i => i.AccountNumber == AccNum) == null)
                AccountList.Add(account);
        }

        public void RemoveAccount(long accountNo)
        {
            if (ClientDataManager.Instance.UniqueAccountList.Contains(accountNo))
            {
                ClientDataManager.Instance.UniqueAccountList.Remove(accountNo);
                //ClientDataManager.UniqueAccountNum = ClientDataManager.Instance.UniqueAccountList.Last();
                Account account = AccountList.FirstOrDefault(i => i.AccountNumber == accountNo);
                if (account != null)
                    AccountList.Remove(account);
            }

        }

        public void UpdateAccount(Account account)
        {
            if (AccountList.Exists(i => i.AccountNumber == account.AccountNumber))
            {
                int ind = AccountList.FindIndex(i => i.AccountNumber == account.AccountNumber);
                AccountList[ind] = account;
            }
            else
            {
                account.AccountNumber = AccountList.First().AccountNumber + 1;
                account.InvestmentNumber = AccountList.First().InvestmentNumber + 1;
                AccountList.Add(account);
            }
        }
        public List<Account> GetAllAccounts()
        {
            return AccountList;
        }
        public Account GetAccount(long accountNo)
        {
            Account account = null;
            if (AccountList.Count > 0)
            {
                account = AccountList.FirstOrDefault(i => i.AccountNumber == accountNo);
            }
            return account;
        }    

    }
}
