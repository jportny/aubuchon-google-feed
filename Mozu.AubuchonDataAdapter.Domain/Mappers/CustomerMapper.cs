using System;
using System.Globalization;
using System.Linq;
using Mozu.Api.Contracts.Customer;
using Mozu.AubuchonDataAdapter.Domain.Contracts;

namespace Mozu.AubuchonDataAdapter.Domain.Mappers
{
    public static class CustomerMapper
    {
        public static MemberMessage ToEdgeCustomerExportMessage(this CustomerAccount account, string providerCode, string xrefMerchantId, StatusCode statusCode)
        {

            var message = new MemberMessage();
            
            var head = new MessageHead
            {
                Sender = new MessageHeadSender[1],
                Recipient = new MessageHeadRecipient[1],
                Messageid = Guid.NewGuid().ToString(),
                Messagetype = "MemberImport",
                Date = DateTime.Now.ToString(CultureInfo.InvariantCulture)
            };

           
            var sender = new MessageHeadSender { Systemid = providerCode };
            head.Sender[0] = sender;

            var recipient = new MessageHeadRecipient { Companyid = xrefMerchantId };
            head.Recipient[0] = recipient;
            message.Items = new object[2];
            message.Items[0] = head;

            var memberNode = new ImportMessageBodyMember
            {
                AffiliateCode = String.Empty,
                AffiliateDate = String.Empty,
                BudgetBalance = String.Empty,
                BudgetFrequency = String.Empty,
                BudgetRestore = String.Empty,
                CreditStatus = String.Empty,
                DateOfBirth = String.Empty,
                Email = account.EmailAddress,
                EmailFormatCode = String.Empty,
                EndDate = String.Empty,
                ExternMemberXRef = Convert.ToString(account.Id),
                Gender = String.Empty,
                Income = String.Empty,
                IsAffiliate = String.Empty,
                LastLoginDate = String.Empty,
                LocaleCode = account.LocaleCode ?? "en-US",
                MarketingPermissionExt = String.Empty,
                MarketingPermissionInt = String.Empty,
                MemberId = String.Empty,
                MemberNumber = Convert.ToString(account.ExternalId),
                NameFirst = account.FirstName,
                NameLast = account.LastName,
                NameMiddle = String.Empty,
                Occupation = String.Empty,
                Organization = account.CompanyOrOrganization,
                OriginCode = String.Empty,
                PrefCommunication = String.Empty,
                PrefShipMethod = String.Empty,
                PriceListCode = String.Empty,
                ProjectCode = "Aubuchon",
                PromotionCode = String.Empty,
                Salutation = String.Empty,
                SourceCode = String.Empty,
                StartDate = DateTime.Today.ToString("yyyy-MM-dd"),
                StorePaymentInfo = "0",
                TaxNumber = account.TaxId,
                TypeCode = "Customer",//TODO:Revisit
                Username = account.EmailAddress,
                Status = new[] { new MessageBodyMemberStatus { StatusCode = Enum.GetName(typeof(StatusCode), statusCode) } }
            };

            var preferredStore =
                account.Attributes.FirstOrDefault(
                    p =>
                        p.FullyQualifiedName.Equals(AttributeConstants.CustomerPreferredStore,
                            StringComparison.InvariantCultureIgnoreCase));

            var preferredStoreValue = preferredStore != null ? preferredStore.Values.FirstOrDefault() : String.Empty;
            
            memberNode.DynamicField = new DynamicField[2];

            memberNode.DynamicField[0] = new DynamicField
            {
                DynamicFieldCode = "aubuchon_customer_id",
                DynamicFieldLabel = "Aubuchon Customer ID",
                DynamicFieldValue = account.ExternalId ?? ""
            };
            if (!String.IsNullOrEmpty(Convert.ToString(preferredStoreValue)))
            {
                memberNode.DynamicField[1] = new DynamicField
                {
                    DynamicFieldCode = "preferred_store",
                    DynamicFieldLabel = "Preferred Store",
                    DynamicFieldValue = Convert.ToString(preferredStoreValue)
                };
            }



            if (account.Segments != null && account.Segments.Any())
            {
                memberNode.GroupList = new MessageBodyMemberGroups
                {
                    Groups = new MessageBodyMemberGroupsGroup[account.Segments.Count]
                };
                for (var i = 0; i < account.Segments.Count; i++)
                {
                    memberNode.GroupList.Groups[i] = new MessageBodyMemberGroupsGroup
                    {
                        groupCode = account.Segments[i].Code
                    }; 
                }
            }

            var addressCount = account.Contacts.Count;
            memberNode.Addresses = new MessageBodyMemberAddressesAddress[addressCount];
            for (var i = 0; i < addressCount; i++)
            {
                //Ignore malformed addresses
                if(String.IsNullOrEmpty(account.Contacts[i].Address.Address1) || String.IsNullOrEmpty(account.Contacts[i].Email) || String.IsNullOrEmpty(account.Contacts[i].Address.PostalOrZipCode)) continue;

                //var primaryType = account.Contacts[i].Types.FirstOrDefault(t => t.Name == "Billing" || t.Name == "Shipping");
                var primaryType = account.Contacts[i].Types.FirstOrDefault(t => t.Name == "Billing");
                
                var isPrimaryType = primaryType != null && primaryType.IsPrimary ? "1" : "0";
                memberNode.Addresses[i] = new MessageBodyMemberAddressesAddress
                {
                    Address1 = account.Contacts[i].Address.Address1,
                    Address2 = account.Contacts[i].Address.Address2,
                    Address3 = account.Contacts[i].Address.Address3,
                    City = account.Contacts[i].Address.CityOrTown,
                    AddressName = String.Format("{0}{1}", account.Contacts[i].Address.AddressType,account.Contacts[i].Id.ToString(CultureInfo.InvariantCulture)),
                    //AUB-1184. Only one State or province code per address node.
                    StateProvinceCode = new MessageBodyMemberAddressesAddressStateProvinceCode[1]
                };
                memberNode.Addresses[i].StateProvinceCode[0] = new MessageBodyMemberAddressesAddressStateProvinceCode
                {
                    Value = account.Contacts[i].Address.StateOrProvince
                };
                memberNode.Addresses[i].CountryCode = account.Contacts[i].Address.CountryCode;
                memberNode.Addresses[i].PostalCode = account.Contacts[i].Address.PostalOrZipCode;
                memberNode.Addresses[i].Email = account.Contacts[i].Email;
                memberNode.Addresses[i].NameFirst = account.Contacts[i].FirstName;
                memberNode.Addresses[i].NameLast = account.Contacts[i].LastNameOrSurname;
                memberNode.Addresses[i].NameMiddle = account.Contacts[i].MiddleNameOrInitial;
                memberNode.Addresses[i].Organization = account.Contacts[i].CompanyOrOrganization;
                memberNode.Addresses[i].PhoneHome = account.Contacts[i].PhoneNumbers.Home;
                memberNode.Addresses[i].PhoneCell = account.Contacts[i].PhoneNumbers.Mobile;
                memberNode.Addresses[i].PhoneWork = account.Contacts[i].PhoneNumbers.Work;
                memberNode.Addresses[i].PhoneWorkExt = String.Empty;
                memberNode.Addresses[i].PhonePager = String.Empty;
                memberNode.Addresses[i].PhoneOther = String.Empty;
                memberNode.Addresses[i].PrimaryForType = isPrimaryType;
                memberNode.Addresses[i].Fax = account.Contacts[i].FaxNumber;
                memberNode.Addresses[i].IsResidential = account.Contacts[i].Address.AddressType == "Residential"
                    ? "1"
                    : "0";
                memberNode.Addresses[i].Salutation = account.Contacts[i].Label;
                memberNode.Addresses[i].AddressObjTypeCode = "both";
                memberNode.Addresses[i].AddressTypeCode = "Shipping";
                memberNode.Addresses[i].Status = new[]
                {
                    new MessageBodyMemberAddressesAddressStatus { StatusCode = "Active"}
                };

                if (String.IsNullOrEmpty(account.Contacts[i].PhoneNumbers.Work))
                {
                    memberNode.Addresses[i].PhoneWork = !String.IsNullOrEmpty(account.Contacts[i].PhoneNumbers.Mobile)
                        ? account.Contacts[i].PhoneNumbers.Mobile
                        : account.Contacts[i].PhoneNumbers.Home;
                }

                var hasBoth = account.Contacts[i].Types.Count() > 1 &&
                              account.Contacts[i].Types.Any(ty => ty.Name == "Shipping" || ty.Name == "Billing");
                if (hasBoth)
                {
                    memberNode.Addresses[i].AddressObjTypeCode = "both";
                    var contactType = account.Contacts[i].Types.FirstOrDefault();
                    if (contactType != null)
                        memberNode.Addresses[i].AddressTypeCode = contactType.Name;
                }
                else
                {
                    foreach (var t in account.Contacts[i].Types)
                    {
                        switch (t.Name)
                        {
                            case "Shipping":
                                memberNode.Addresses[i].AddressObjTypeCode = "shipto";
                                break;
                            case "Billing":
                                memberNode.Addresses[i].AddressObjTypeCode = "billto";
                                break;
                        }
                        memberNode.Addresses[i].AddressTypeCode = t.Name;
                    }
                }

            }

            var body = new MemberMessageBody { Memberimport = new[] { memberNode } };
            
            message.Items[1] = body;

            return message;
        }

        //static int GetNewAddressNameIndex(string addressType, IEnumerable<MessageBodyMemberAddressesAddress> addresses)
        //{
        //    var filtered = addresses.Where(a => a != null && a.AddressName.StartsWith(addressType)).ToList();

        //    if (!filtered.Any()) return 1;

        //    var max = (from address in filtered select address.AddressName.Replace(addressType, "") 
        //                   into indexValue where !String.IsNullOrEmpty(indexValue) 
        //               select Convert.ToInt32(indexValue)).Concat(new[] {0}).Max();
        //    return max + 1;
        //}
    }
}
