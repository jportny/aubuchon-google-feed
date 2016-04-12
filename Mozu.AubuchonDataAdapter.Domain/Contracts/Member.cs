using System.Xml.Serialization;

namespace Mozu.AubuchonDataAdapter.Domain.Contracts
{



    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.18020")]
    [System.SerializableAttribute]
    [System.Diagnostics.DebuggerStepThroughAttribute]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    [XmlRootAttribute(ElementName = "message", Namespace = "", IsNullable = false)]
    public class MemberMessage
    {

        private object[] _itemsField;

        /// <remarks/>
        [XmlElementAttribute("body", typeof(MemberMessageBody), Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [XmlElementAttribute("head", typeof(MessageHead), Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public object[] Items
        {
            get
            {
                return _itemsField;
            }
            set
            {
                _itemsField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.18020")]
    [System.SerializableAttribute]
    [System.Diagnostics.DebuggerStepThroughAttribute]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class MemberMessageBody
    {

        private ImportMessageBodyMember[] _memberimportField;

        /// <remarks/>
        [XmlElementAttribute("member", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public ImportMessageBodyMember[] Memberimport
        {
            get
            {
                return _memberimportField;
            }
            set
            {
                _memberimportField = value;
            }
        }
    }


    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute]
    [System.Diagnostics.DebuggerStepThroughAttribute]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    [XmlRootAttribute(ElementName = "member", Namespace = "", IsNullable = false)]
    public class ImportMessageBodyMember
    {

        private string _dateOfBirthField;

        private string _emailField;

        private string _genderField;

        private string _localeField;

        private string _memberIdField;

        private string _memberNumberField;

        private string _nameFirstField;

        private string _nameLastField;

        private string _nameMiddleField;

        private string _organizationField;

        private string _projectCodeField;

        private string _typeCodeField;

        private DynamicField[] _dynamicFieldField;

        private MessageBodyMemberStatus[] _statusField;

        private MessageBodyMemberAddressesAddress[] _addressesField;
        private MessageBodyMemberGroups _groupsField;

        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string AffiliateCode { get; set; }
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string AffiliateDate { get; set; }
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string BudgetBalance { get; set; }
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string BudgetFrequency { get; set; }
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string BudgetRestore { get; set; }
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CreditStatus { get; set; }
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string EmailFormatCode { get; set; }
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string EndDate { get; set; }
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ExternMemberXRef { get; set; }
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Income { get; set; }
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string IsAffiliate { get; set; }
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string LastLoginDate { get; set; }
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string LocaleCode { get; set; }
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]

        public string MarketingPermissionExt { get; set; }
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string MarketingPermissionInt { get; set; }
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Occupation { get; set; }
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string OriginCode { get; set; }
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Password { get; set; }
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string PrefCommunication { get; set; }
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string PrefShipMethod { get; set; }
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string PriceListCode { get; set; }
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string PromotionCode { get; set; }
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Salutation { get; set; }
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string SourceCode { get; set; }
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string StartDate { get; set; }
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string StorePaymentInfo { get; set; }
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string TaxNumber { get; set; }
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Username { get; set; }

        /// <remarks/>
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DateOfBirth
        {
            get
            {
                return _dateOfBirthField;
            }
            set
            {
                _dateOfBirthField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Email
        {
            get
            {
                return _emailField;
            }
            set
            {
                _emailField = value;
            }
        }


        /// <remarks/>
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Gender
        {
            get
            {
                return _genderField;
            }
            set
            {
                _genderField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Locale
        {
            get
            {
                return _localeField;
            }
            set
            {
                _localeField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string MemberId
        {
            get
            {
                return _memberIdField;
            }
            set
            {
                _memberIdField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string MemberNumber
        {
            get
            {
                return _memberNumberField;
            }
            set
            {
                _memberNumberField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string NameFirst
        {
            get
            {
                return _nameFirstField;
            }
            set
            {
                _nameFirstField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string NameLast
        {
            get
            {
                return _nameLastField;
            }
            set
            {
                _nameLastField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string NameMiddle
        {
            get
            {
                return _nameMiddleField;
            }
            set
            {
                _nameMiddleField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Organization
        {
            get
            {
                return _organizationField;
            }
            set
            {
                _organizationField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ProjectCode
        {
            get
            {
                return _projectCodeField;
            }
            set
            {
                _projectCodeField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string TypeCode
        {
            get
            {
                return _typeCodeField;
            }
            set
            {
                _typeCodeField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute("status", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public MessageBodyMemberStatus[] Status
        {
            get
            {
                return _statusField;
            }
            set
            {
                _statusField = value;
            }
        }

        /// <remarks/>
        [XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [XmlArrayItemAttribute("address", typeof(MessageBodyMemberAddressesAddress), Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
        public MessageBodyMemberAddressesAddress[] Addresses
        {
            get
            {
                return _addressesField;
            }
            set
            {
                _addressesField = value;
            }
        }


        //[XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        //[XmlArrayItemAttribute("groups", typeof(MessageBodyMemberGroupsGroup), Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
        [XmlElement("Groups")]
        public MessageBodyMemberGroups GroupList
        {
            get
            {
                return _groupsField;
            }
            set
            {
                _groupsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("dynamicField")]
        public DynamicField[] DynamicField
        {
            get
            {
                return _dynamicFieldField;
            }
            set
            {
                _dynamicFieldField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute]
    [System.Diagnostics.DebuggerStepThroughAttribute]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class MessageBodyMemberStatus
    {

        private string _statusCodeField;

        /// <remarks/>
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string StatusCode
        {
            get
            {
                return _statusCodeField;
            }
            set
            {
                _statusCodeField = value;
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute]
    [System.Diagnostics.DebuggerStepThroughAttribute]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class MessageBodyMemberGroups
    {
        private MessageBodyMemberGroupsGroup[] _groups;
        

        [XmlElementAttribute(ElementName = "Group", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public MessageBodyMemberGroupsGroup[] Groups
        {
            get { return _groups; }
            set { _groups = value; }
        }

    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.18020")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public class MessageBodyMemberGroupsGroup
    {

        private string _groupIdField;

        private string _groupTypeNameField;

        private string _groupNameField;

        private string _groupCodeField;

        private string _promotionCodeField;

        private string _startDateField;

        private string _endDateField;

        private string _budgetField;

        private string _dynamicFieldsField;

        private status[] _statusField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string groupID
        {
            get
            {
                return this._groupIdField;
            }
            set
            {
                this._groupIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string groupTypeName
        {
            get
            {
                return this._groupTypeNameField;
            }
            set
            {
                this._groupTypeNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string groupName
        {
            get
            {
                return this._groupNameField;
            }
            set
            {
                this._groupNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string groupCode
        {
            get
            {
                return this._groupCodeField;
            }
            set
            {
                this._groupCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string promotionCode
        {
            get
            {
                return this._promotionCodeField;
            }
            set
            {
                this._promotionCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string startDate
        {
            get
            {
                return this._startDateField;
            }
            set
            {
                this._startDateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string endDate
        {
            get
            {
                return this._endDateField;
            }
            set
            {
                this._endDateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string budget
        {
            get
            {
                return this._budgetField;
            }
            set
            {
                this._budgetField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string dynamicFields
        {
            get
            {
                return this._dynamicFieldsField;
            }
            set
            {
                this._dynamicFieldsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("status")]
        public status[] status
        {
            get
            {
                return this._statusField;
            }
            set
            {
                this._statusField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute]
    [System.Diagnostics.DebuggerStepThroughAttribute]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class MessageBodyMemberAddressesAddress
    {

        private string _address1Field;

        private string _address2Field;

        private string _address3Field;

        private string _addressIdField;

        private string _addressNameField;

        private string _addressObjTypeCodeField;

        private string _addressTypeCodeField;

        private string _cityField;

        private string _countryCodeField;

        private string _dtResidentialField;

        private string _emailField;

        private string _faxField;

        private string _isResidentialField;

        private string _nameMiddleField;

        private string _nameFirstField;

        private string _nameLastField;

        private string _organizationField;

        private string _phoneCellField;

        private string _phoneHomeField;

        private string _phoneOtherField;

        private string _phonePagerField;

        private string _phoneWorkField;

        private string _phoneWorkExtField;

        private string _postalCodeField;

        private string _primaryForTypeField;

        private string _salutationField;

        private MessageBodyMemberAddressesAddressStateProvinceCode[] _stateProvinceCodeField;
        private MessageBodyMemberAddressesAddressStatus[] _status;

        /// <remarks/>
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Address1
        {
            get
            {
                return _address1Field;
            }
            set
            {
                _address1Field = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Address2
        {
            get
            {
                return _address2Field;
            }
            set
            {
                _address2Field = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Address3
        {
            get
            {
                return _address3Field;
            }
            set
            {
                _address3Field = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string AddressId
        {
            get
            {
                return _addressIdField;
            }
            set
            {
                _addressIdField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string AddressName
        {
            get
            {
                return _addressNameField;
            }
            set
            {
                _addressNameField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string AddressObjTypeCode
        {
            get
            {
                return _addressObjTypeCodeField;
            }
            set
            {
                _addressObjTypeCodeField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string AddressTypeCode
        {
            get
            {
                return _addressTypeCodeField;
            }
            set
            {
                _addressTypeCodeField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string City
        {
            get
            {
                return _cityField;
            }
            set
            {
                _cityField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CountryCode
        {
            get
            {
                return _countryCodeField;
            }
            set
            {
                _countryCodeField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DtResidential
        {
            get
            {
                return _dtResidentialField;
            }
            set
            {
                _dtResidentialField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Email
        {
            get
            {
                return _emailField;
            }
            set
            {
                _emailField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Fax
        {
            get
            {
                return _faxField;
            }
            set
            {
                _faxField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string IsResidential
        {
            get
            {
                return _isResidentialField;
            }
            set
            {
                _isResidentialField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string NameMiddle
        {
            get
            {
                return _nameMiddleField;
            }
            set
            {
                _nameMiddleField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string NameFirst
        {
            get
            {
                return _nameFirstField;
            }
            set
            {
                _nameFirstField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string NameLast
        {
            get
            {
                return _nameLastField;
            }
            set
            {
                _nameLastField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Organization
        {
            get
            {
                return _organizationField;
            }
            set
            {
                _organizationField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string PhoneCell
        {
            get
            {
                return _phoneCellField;
            }
            set
            {
                _phoneCellField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string PhoneHome
        {
            get
            {
                return _phoneHomeField;
            }
            set
            {
                _phoneHomeField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string PhoneOther
        {
            get
            {
                return _phoneOtherField;
            }
            set
            {
                _phoneOtherField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string PhonePager
        {
            get
            {
                return _phonePagerField;
            }
            set
            {
                _phonePagerField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string PhoneWork
        {
            get
            {
                return _phoneWorkField;
            }
            set
            {
                _phoneWorkField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string PhoneWorkExt
        {
            get
            {
                return _phoneWorkExtField;
            }
            set
            {
                _phoneWorkExtField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string PostalCode
        {
            get
            {
                return _postalCodeField;
            }
            set
            {
                _postalCodeField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string PrimaryForType
        {
            get
            {
                return _primaryForTypeField;
            }
            set
            {
                _primaryForTypeField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Salutation
        {
            get
            {
                return _salutationField;
            }
            set
            {
                _salutationField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute("stateProvinceCode", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
        public MessageBodyMemberAddressesAddressStateProvinceCode[] StateProvinceCode
        {
            get
            {
                return _stateProvinceCodeField;
            }
            set
            {
                _stateProvinceCodeField = value;
            }
        }

        [XmlElementAttribute("status", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
        public MessageBodyMemberAddressesAddressStatus[] Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute]
    [System.Diagnostics.DebuggerStepThroughAttribute]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class MessageBodyMemberAddressesAddressStateProvinceCode
    {

        private string _valueField;

        /// <remarks/>
        [XmlTextAttribute]
        public string Value
        {
            get
            {
                return _valueField;
            }
            set
            {
                _valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute]
    [System.Diagnostics.DebuggerStepThroughAttribute]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class MessageBodyMemberAddressesAddressStatus
    {

        private string _statusCode;

        /// <remarks/>
        [XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string StatusCode
        {
            get
            {
                return _statusCode;
            }
            set
            {
                _statusCode = value;
            }
        }
    }


}
