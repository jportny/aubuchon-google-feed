﻿using System;
using System.Xml;

namespace Mozu.AubuchonDataAdapter.Domain.Contracts
{
    //------------------------------------------------------------------------------
    // <auto-generated>
    //     This code was generated by a tool.
    //     Runtime Version:4.0.30319.34209
    //
    //     Changes to this file may cause incorrect behavior and will be lost if
    //     the code is regenerated.
    // </auto-generated>
    //------------------------------------------------------------------------------

    // 
    // This source code was auto-generated by xsd, Version=4.0.30319.18020.
    // 


    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.18020")]
    [System.SerializableAttribute]
    [System.Diagnostics.DebuggerStepThroughAttribute]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public class Filegroups
    {

        private FilegroupsFilegroupGroupcode[] _filegroupField;

        private FilegroupsGroupCode[] _groupCodeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("groupcode", typeof(FilegroupsFilegroupGroupcode), Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public FilegroupsFilegroupGroupcode[] Filegroup
        {
            get
            {
                return _filegroupField;
            }
            set
            {
                _filegroupField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("groupCode", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true)]
        public FilegroupsGroupCode[] GroupCode
        {
            get
            {
                return _groupCodeField;
            }
            set
            {
                _groupCodeField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.18020")]
    [System.SerializableAttribute]
    [System.Diagnostics.DebuggerStepThroughAttribute]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public class FilegroupsFilegroupGroupcode
    {

        private string _valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute]
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.18020")]
    [System.SerializableAttribute]
    [System.Diagnostics.DebuggerStepThroughAttribute]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public class FilegroupsGroupCode
    {

        private string _valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute]
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.18020")]
    [System.SerializableAttribute]
    [System.Diagnostics.DebuggerStepThroughAttribute]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", ElementName = "message", IsNullable = false)]
    public class FileuploadMessage
    {

        private object[] _itemsField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("body", typeof(FileuploadMessageBody), Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlElementAttribute("filegroups", typeof(Filegroups))]
        [System.Xml.Serialization.XmlElementAttribute("head", typeof(MessageHead), Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
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
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public class FileuploadMessageBody
    {

        private MessageBodyFileupload[] _fileuploadField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("fileupload", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public MessageBodyFileupload[] Fileupload
        {
            get
            {
                return _fileuploadField;
            }
            set
            {
                _fileuploadField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.18020")]
    [System.SerializableAttribute]
    [System.Diagnostics.DebuggerStepThroughAttribute]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public class MessageBodyFileupload
    {

        private Filegroups[] _filegroupsField;

        private MessageBodyFileuploadFilesFile[] _filesField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("filegroups")]
        public Filegroups[] Filegroups
        {
            get
            {
                return _filegroupsField;
            }
            set
            {
                _filegroupsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("file", typeof(MessageBodyFileuploadFilesFile), Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
        public MessageBodyFileuploadFilesFile[] Files
        {
            get
            {
                return _filesField;
            }
            set
            {
                _filesField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.18020")]
    [System.SerializableAttribute]
    [System.Diagnostics.DebuggerStepThroughAttribute]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public class MessageBodyFileuploadFilesFile
    {

        private string _titleField;

        private string _filenameField;

        private string _fileclassField;

        private string _descriptionField;

        private string _fileLinkField;

        private string _base64BinaryField;

        private Filegroups[] _filegroupsField;

        private MessageBodyFileuploadFilesFileLocalesLocale[] _localesField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Title
        {
            get
            {
                return _titleField;
            }
            set
            {
                _titleField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Filename
        {
            get
            {
                return _filenameField;
            }
            set
            {
                _filenameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Fileclass
        {
            get
            {
                return _fileclassField;
            }
            set
            {
                _fileclassField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Description
        {
            get
            {
                return _descriptionField;
            }
            set
            {
                _descriptionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]

        public string FileLink
        {
            get { return _fileLinkField; }
            set
            {

                _fileLinkField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Base64Binary
        {
            get
            {
                return _base64BinaryField;
            }
            set
            {
                _base64BinaryField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("filegroups")]
        public Filegroups[] Filegroups
        {
            get
            {
                return _filegroupsField;
            }
            set
            {
                _filegroupsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("Locale", typeof(MessageBodyFileuploadFilesFileLocalesLocale), Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = false)]
        public MessageBodyFileuploadFilesFileLocalesLocale[] Locales
        {
            get
            {
                return _localesField;
            }
            set
            {
                _localesField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.18020")]
    [System.SerializableAttribute]
    [System.Diagnostics.DebuggerStepThroughAttribute]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public class MessageBodyFileuploadFilesFileLocalesLocale
    {

        private string _localeCodeField;

        private string _titleField;

        private string _descriptionField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string LocaleCode
        {
            get
            {
                return _localeCodeField;
            }
            set
            {
                _localeCodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Title
        {
            get
            {
                return _titleField;
            }
            set
            {
                _titleField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Description
        {
            get
            {
                return _descriptionField;
            }
            set
            {
                _descriptionField = value;
            }
        }
    }

}
