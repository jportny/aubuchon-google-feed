﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.5485
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Xml.Serialization;

// 
// This source code was auto-generated by xsd, Version=2.0.50727.3038.
// 
using Mozu.AubuchonDataAdapter.Domain.Contracts;


/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class status {
    
    private string statusIDField;
    
    private string statusCodeField;
    
    private string statusNameField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string statusID {
        get {
            return this.statusIDField;
        }
        set {
            this.statusIDField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string statusCode {
        get {
            return this.statusCodeField;
        }
        set {
            this.statusCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string statusName {
        get {
            return this.statusNameField;
        }
        set {
            this.statusNameField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false, ElementName="Message")]
public partial class EdgeMessage {
    
    private object[] itemsField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("Body", typeof(EdgeMessageBody), Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    [System.Xml.Serialization.XmlElementAttribute("Head", typeof(EdgeMessageHead), Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    [System.Xml.Serialization.XmlElementAttribute("status", typeof(status))]
    public object[] Items {
        get {
            return this.itemsField;
        }
        set {
            this.itemsField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class EdgeMessageBody {
    
    private EdgeMessageBodyMember[] memberField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("Member", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public EdgeMessageBodyMember[] Member {
        get {
            return this.memberField;
        }
        set {
            this.memberField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class EdgeMessageBodyMember {
    
    private string affiliateCodeField;
    
    private string affiliationDateField;
    
    private string ageField;
    
    private string badLoginAttemptsField;
    
    private string budgetBalanceField;
    
    private string budgetFrequencyField;
    
    private string budgetLastUpdatedField;
    
    private string budgetRestoreField;
    
    private string createDateField;
    
    private string creditStatusField;
    
    private string dateOfBirthField;
    
    private string downloadTotalField;
    
    private string emailField;
    
    private string emailFormatCodeField;
    
    private string emailValidationFailedField;
    
    private string endDateField;
    
    private string externMemberXRefField;
    
    private string genderField;
    
    private string incomeField;
    
    private string isAffiliateField;
    
    private string lastLoginDateField;
    
    private string lastUpdatedField;
    
    private string localeField;
    
    private string marketingPermissionExtField;
    
    private string marketingPermissionIntField;
    
    private string memberIDField;
    
    private string memberNumberField;
    
    private string nameFirstField;
    
    private string nameLastField;
    
    private string nameMiddleField;
    
    private string occupationField;
    
    private string organizationField;
    
    private string originCodeField;
    
    private string passwordChangeDateField;
    
    private string prefCommunicationField;
    
    private string prefShipMethodField;
    
    private string priceListCodeField;
    
    private string projectCodeField;
    
    private string promotionCodeField;
    
    private string salutationField;
    
    private string sourceCodeField;
    
    private string startDateField;
    
    private string storePaymentInfoField;
    
    private string taxNumberField;
    
    private string totalItemsField;
    
    private string totalSalesField;
    
    private string totalTransactionField;
    
    private string typeCodeField;
    
    private string usernameField;
    
    private status[] statusField;
    
    private EdgeMessageBodyMemberAddressesAddress[] addressesField;
    private MessageBodyMemberGroupsGroup[] groupsField;
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string affiliateCode {
        get {
            return this.affiliateCodeField;
        }
        set {
            this.affiliateCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string affiliationDate {
        get {
            return this.affiliationDateField;
        }
        set {
            this.affiliationDateField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string age {
        get {
            return this.ageField;
        }
        set {
            this.ageField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string badLoginAttempts {
        get {
            return this.badLoginAttemptsField;
        }
        set {
            this.badLoginAttemptsField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string budgetBalance {
        get {
            return this.budgetBalanceField;
        }
        set {
            this.budgetBalanceField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string budgetFrequency {
        get {
            return this.budgetFrequencyField;
        }
        set {
            this.budgetFrequencyField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string budgetLastUpdated {
        get {
            return this.budgetLastUpdatedField;
        }
        set {
            this.budgetLastUpdatedField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string budgetRestore {
        get {
            return this.budgetRestoreField;
        }
        set {
            this.budgetRestoreField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string createDate {
        get {
            return this.createDateField;
        }
        set {
            this.createDateField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string creditStatus {
        get {
            return this.creditStatusField;
        }
        set {
            this.creditStatusField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string dateOfBirth {
        get {
            return this.dateOfBirthField;
        }
        set {
            this.dateOfBirthField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string downloadTotal {
        get {
            return this.downloadTotalField;
        }
        set {
            this.downloadTotalField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string email {
        get {
            return this.emailField;
        }
        set {
            this.emailField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string emailFormatCode {
        get {
            return this.emailFormatCodeField;
        }
        set {
            this.emailFormatCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string emailValidationFailed {
        get {
            return this.emailValidationFailedField;
        }
        set {
            this.emailValidationFailedField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string endDate {
        get {
            return this.endDateField;
        }
        set {
            this.endDateField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string externMemberXRef {
        get {
            return this.externMemberXRefField;
        }
        set {
            this.externMemberXRefField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string gender {
        get {
            return this.genderField;
        }
        set {
            this.genderField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string income {
        get {
            return this.incomeField;
        }
        set {
            this.incomeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string isAffiliate {
        get {
            return this.isAffiliateField;
        }
        set {
            this.isAffiliateField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string lastLoginDate {
        get {
            return this.lastLoginDateField;
        }
        set {
            this.lastLoginDateField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string lastUpdated {
        get {
            return this.lastUpdatedField;
        }
        set {
            this.lastUpdatedField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string locale {
        get {
            return this.localeField;
        }
        set {
            this.localeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string marketingPermissionExt {
        get {
            return this.marketingPermissionExtField;
        }
        set {
            this.marketingPermissionExtField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string marketingPermissionInt {
        get {
            return this.marketingPermissionIntField;
        }
        set {
            this.marketingPermissionIntField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string memberID {
        get {
            return this.memberIDField;
        }
        set {
            this.memberIDField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string memberNumber {
        get {
            return this.memberNumberField;
        }
        set {
            this.memberNumberField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string nameFirst {
        get {
            return this.nameFirstField;
        }
        set {
            this.nameFirstField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string nameLast {
        get {
            return this.nameLastField;
        }
        set {
            this.nameLastField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string nameMiddle {
        get {
            return this.nameMiddleField;
        }
        set {
            this.nameMiddleField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string occupation {
        get {
            return this.occupationField;
        }
        set {
            this.occupationField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string organization {
        get {
            return this.organizationField;
        }
        set {
            this.organizationField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string originCode {
        get {
            return this.originCodeField;
        }
        set {
            this.originCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string passwordChangeDate {
        get {
            return this.passwordChangeDateField;
        }
        set {
            this.passwordChangeDateField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string prefCommunication {
        get {
            return this.prefCommunicationField;
        }
        set {
            this.prefCommunicationField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string prefShipMethod {
        get {
            return this.prefShipMethodField;
        }
        set {
            this.prefShipMethodField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string priceListCode {
        get {
            return this.priceListCodeField;
        }
        set {
            this.priceListCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string projectCode {
        get {
            return this.projectCodeField;
        }
        set {
            this.projectCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string promotionCode {
        get {
            return this.promotionCodeField;
        }
        set {
            this.promotionCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string salutation {
        get {
            return this.salutationField;
        }
        set {
            this.salutationField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string sourceCode {
        get {
            return this.sourceCodeField;
        }
        set {
            this.sourceCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string startDate {
        get {
            return this.startDateField;
        }
        set {
            this.startDateField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string storePaymentInfo {
        get {
            return this.storePaymentInfoField;
        }
        set {
            this.storePaymentInfoField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string taxNumber {
        get {
            return this.taxNumberField;
        }
        set {
            this.taxNumberField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string totalItems {
        get {
            return this.totalItemsField;
        }
        set {
            this.totalItemsField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string totalSales {
        get {
            return this.totalSalesField;
        }
        set {
            this.totalSalesField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string totalTransaction {
        get {
            return this.totalTransactionField;
        }
        set {
            this.totalTransactionField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string typeCode {
        get {
            return this.typeCodeField;
        }
        set {
            this.typeCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string username {
        get {
            return this.usernameField;
        }
        set {
            this.usernameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("status")]
    public status[] status {
        get {
            return this.statusField;
        }
        set {
            this.statusField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    [System.Xml.Serialization.XmlArrayItemAttribute("address", typeof(EdgeMessageBodyMemberAddressesAddress), Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
    public EdgeMessageBodyMemberAddressesAddress[] addresses {
        get {
            return this.addressesField;
        }
        set {
            this.addressesField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    [System.Xml.Serialization.XmlArrayItemAttribute("group", typeof(MessageBodyMemberGroupsGroup), Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
    public MessageBodyMemberGroupsGroup[] groups
    {
        get
        {
            return this.groupsField;
        }
        set
        {
            this.groupsField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class EdgeMessageBodyMemberAddressesAddress {
    
    private string address1Field;
    
    private string address2Field;
    
    private string address3Field;
    
    private string addressIDField;
    
    private string addressNameField;
    
    private string addressObjTypeCodeField;
    
    private string addressTypeCodeField;
    
    private string cityField;
    
    private string countryCodeField;
    
    private string createDateField;
    
    private string dtResidentialField;
    
    private string emailField;
    
    private string faxField;
    
    private string isResidentialField;
    
    private string lastUpdatedField;
    
    private string nameFirstField;
    
    private string nameLastField;
    
    private string nameMiddleField;
    
    private string organizationField;
    
    private string phoneCellField;
    
    private string phoneHomeField;
    
    private string phoneOtherField;
    
    private string phonePagerField;
    
    private string phoneWorkField;
    
    private string phoneWorkExtField;
    
    private string postalCodeField;
    
    private string primaryForTypeField;
    
    private string salutationField;
    
    private string stateProvinceCodeField;
    
    private status[] statusField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string address1 {
        get {
            return this.address1Field;
        }
        set {
            this.address1Field = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string address2 {
        get {
            return this.address2Field;
        }
        set {
            this.address2Field = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string address3 {
        get {
            return this.address3Field;
        }
        set {
            this.address3Field = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string addressID {
        get {
            return this.addressIDField;
        }
        set {
            this.addressIDField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string addressName {
        get {
            return this.addressNameField;
        }
        set {
            this.addressNameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string addressObjTypeCode {
        get {
            return this.addressObjTypeCodeField;
        }
        set {
            this.addressObjTypeCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string addressTypeCode {
        get {
            return this.addressTypeCodeField;
        }
        set {
            this.addressTypeCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string city {
        get {
            return this.cityField;
        }
        set {
            this.cityField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string countryCode {
        get {
            return this.countryCodeField;
        }
        set {
            this.countryCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string createDate {
        get {
            return this.createDateField;
        }
        set {
            this.createDateField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string dtResidential {
        get {
            return this.dtResidentialField;
        }
        set {
            this.dtResidentialField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string email {
        get {
            return this.emailField;
        }
        set {
            this.emailField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string fax {
        get {
            return this.faxField;
        }
        set {
            this.faxField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string isResidential {
        get {
            return this.isResidentialField;
        }
        set {
            this.isResidentialField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string lastUpdated {
        get {
            return this.lastUpdatedField;
        }
        set {
            this.lastUpdatedField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string nameFirst {
        get {
            return this.nameFirstField;
        }
        set {
            this.nameFirstField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string nameLast {
        get {
            return this.nameLastField;
        }
        set {
            this.nameLastField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string nameMiddle {
        get {
            return this.nameMiddleField;
        }
        set {
            this.nameMiddleField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string organization {
        get {
            return this.organizationField;
        }
        set {
            this.organizationField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string phoneCell {
        get {
            return this.phoneCellField;
        }
        set {
            this.phoneCellField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string phoneHome {
        get {
            return this.phoneHomeField;
        }
        set {
            this.phoneHomeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string phoneOther {
        get {
            return this.phoneOtherField;
        }
        set {
            this.phoneOtherField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string phonePager {
        get {
            return this.phonePagerField;
        }
        set {
            this.phonePagerField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string phoneWork {
        get {
            return this.phoneWorkField;
        }
        set {
            this.phoneWorkField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string phoneWorkExt {
        get {
            return this.phoneWorkExtField;
        }
        set {
            this.phoneWorkExtField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string postalCode {
        get {
            return this.postalCodeField;
        }
        set {
            this.postalCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string primaryForType {
        get {
            return this.primaryForTypeField;
        }
        set {
            this.primaryForTypeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string salutation {
        get {
            return this.salutationField;
        }
        set {
            this.salutationField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string stateProvinceCode {
        get {
            return this.stateProvinceCodeField;
        }
        set {
            this.stateProvinceCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("status")]
    public status[] status {
        get {
            return this.statusField;
        }
        set {
            this.statusField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class EdgeMessageHead {
    
    private string messageIDField;
    
    private string dateField;
    
    private string messageTypeField;
    
    private EdgeMessageHeadSender[] senderField;
    
    private EdgeMessageHeadRecipient[] recipientField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string MessageID {
        get {
            return this.messageIDField;
        }
        set {
            this.messageIDField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string Date {
        get {
            return this.dateField;
        }
        set {
            this.dateField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string MessageType {
        get {
            return this.messageTypeField;
        }
        set {
            this.messageTypeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("Sender", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public EdgeMessageHeadSender[] Sender {
        get {
            return this.senderField;
        }
        set {
            this.senderField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("Recipient", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public EdgeMessageHeadRecipient[] Recipient {
        get {
            return this.recipientField;
        }
        set {
            this.recipientField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class EdgeMessageHeadSender {
    
    private string systemIDField;
    
    private string companyIDField;
    
    private string replyToQField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string SystemID {
        get {
            return this.systemIDField;
        }
        set {
            this.systemIDField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string CompanyID {
        get {
            return this.companyIDField;
        }
        set {
            this.companyIDField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string ReplyToQ {
        get {
            return this.replyToQField;
        }
        set {
            this.replyToQField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class EdgeMessageHeadRecipient {
    
    private string systemIDField;
    
    private string companyIDField;
    
    private string replyToQField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string SystemID {
        get {
            return this.systemIDField;
        }
        set {
            this.systemIDField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string CompanyID {
        get {
            return this.companyIDField;
        }
        set {
            this.companyIDField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string ReplyToQ {
        get {
            return this.replyToQField;
        }
        set {
            this.replyToQField = value;
        }
    }
}