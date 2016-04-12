using System.Xml.Serialization;

namespace Mozu.AubuchonDataAdapter.Domain.Contracts
{
   

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.18020")]
    [System.SerializableAttribute]
    [System.Diagnostics.DebuggerStepThroughAttribute]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    [XmlRootAttribute(ElementName = "head", Namespace = "", IsNullable = false)]
    public class MessageHead
    {

        private string _messageidField;

        private string _dateField;

        private string _messagetypeField;

        private string _autoShipOrderHeadId;
        private string _destinationProviderCode;

        private MessageHeadSender[] _senderField;

        private MessageHeadRecipient[] _recipientField;
        private string _responseDestinationProviderCode;
        private string _interceptorName;

        /// <remarks/>
        [XmlElementAttribute(ElementName = "messageID", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Messageid
        {
            get
            {
                return _messageidField;
            }
            set
            {
                _messageidField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(ElementName = "date",Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Date
        {
            get
            {
                return _dateField;
            }
            set
            {
                _dateField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(ElementName = "autoshiporderheadid", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string AutoShipOrderHeadId
        {
            get
            {
                return _autoShipOrderHeadId;
            }
            set
            {
                _autoShipOrderHeadId = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(ElementName = "messageType", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Messagetype
        {
            get
            {
                return _messagetypeField;
            }
            set
            {
                _messagetypeField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(ElementName = "destinationprovidercode", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DestinationProviderCode
        {
            get
            {
                return _destinationProviderCode;
            }
            set
            {
                _destinationProviderCode = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(ElementName = "responsedestinationprovidercode", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ResponseDestinationProviderCode
        {
            get { return _responseDestinationProviderCode; }
            set { _responseDestinationProviderCode = value; }
        }

        /// <remarks/>
        [XmlElementAttribute(ElementName = "interceptorname", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
     
        public string InterceptorName
        {
            get { return _interceptorName; }
            set { _interceptorName = value; }
        }

        /// <remarks/>
        [XmlElementAttribute("sender", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public MessageHeadSender[] Sender
        {
            get
            {
                return _senderField;
            }
            set
            {
                _senderField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute("recipient", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public MessageHeadRecipient[] Recipient
        {
            get
            {
                return _recipientField;
            }
            set
            {
                _recipientField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.18020")]
    [System.SerializableAttribute]
    [System.Diagnostics.DebuggerStepThroughAttribute]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    [XmlRootAttribute(ElementName = "sender", Namespace = "", IsNullable = false)]
    public class MessageHeadSender
    {

        private string _systemidField;

        private string _companyidField;

        private string _replytoqField;

        /// <remarks/>
        [XmlElementAttribute(ElementName = "systemid",Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Systemid
        {
            get
            {
                return _systemidField;
            }
            set
            {
                _systemidField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(ElementName = "companyID",Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Companyid
        {
            get
            {
                return _companyidField;
            }
            set
            {
                _companyidField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(ElementName = "replyToQ", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Replytoq
        {
            get
            {
                return _replytoqField;
            }
            set
            {
                _replytoqField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.18020")]
    [System.SerializableAttribute]
    [System.Diagnostics.DebuggerStepThroughAttribute]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true)]
    [XmlRootAttribute(ElementName = "recipient", Namespace = "", IsNullable = false)]
    public class MessageHeadRecipient
    {

        private string _systemidField;

        private string _companyidField;

        private string _replytoqField;

        /// <remarks/>
        [XmlElementAttribute(ElementName = "systemID", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Systemid
        {
            get
            {
                return _systemidField;
            }
            set
            {
                _systemidField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(ElementName = "companyID",Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Companyid
        {
            get
            {
                return _companyidField;
            }
            set
            {
                _companyidField = value;
            }
        }

        /// <remarks/>
        [XmlElementAttribute(ElementName = "replyToQ",Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Replytoq
        {
            get
            {
                return _replytoqField;
            }
            set
            {
                _replytoqField = value;
            }
        }
    }
}
