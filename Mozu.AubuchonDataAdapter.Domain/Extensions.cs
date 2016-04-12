using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Mozu.Api;
using Mozu.Api.Contracts.CommerceRuntime.Orders;
using Mozu.Api.Contracts.CommerceRuntime.Payments;
using Mozu.Api.Contracts.Customer;
using Mozu.Api.Contracts.Event;
using Mozu.Api.Contracts.ProductAdmin;
using Mozu.Api.Contracts.ProductRuntime;
using Mozu.Api.Resources.Commerce.Customer.Accounts;
using Mozu.Api.Resources.Commerce.Customer.Attributedefinition;
using Mozu.Api.Resources.Content.Documentlists;
using Mozu.AubuchonDataAdapter.Domain.Contracts;
using Mozu.AubuchonDataAdapter.Domain.Mappers;
using LocationInventory = Mozu.Api.Contracts.ProductAdmin.LocationInventory;
using Product = Mozu.Api.Contracts.ProductAdmin.Product;
using ProductOption = Mozu.Api.Contracts.ProductAdmin.ProductOption;
using ProductProperty = Mozu.Api.Contracts.ProductAdmin.ProductProperty;

namespace Mozu.AubuchonDataAdapter.Domain
{
    public static class Extensions
    {

        public static string ToXml(this Object item)
        {
            if (item == null)
                return null;

            var serializer = new XmlSerializer(item.GetType());
            var stream = new MemoryStream();

            var enc = new UTF8Encoding(false);
            var writer = new XmlTextWriter(stream, enc);

            var ns = new XmlSerializerNamespaces();
            ns.Add(String.Empty, String.Empty);

            serializer.Serialize(writer, item, ns);

            var xml = Encoding.UTF8.GetString(
                stream.GetBuffer(), 0, (int)stream.Length);
            writer.Close();
            stream.Close();
            return xml;

        }


        public static string ToTitleCase(this string str)
        {
            if (String.IsNullOrEmpty(str)) return str;
            var txtInfo = new CultureInfo("en-US", false).TextInfo;
            return txtInfo.ToTitleCase(str);
        }
        public static Payment Clone(this Payment payment)
        {
            var clone = new Payment
            {
                AmountCollected = payment.AmountCollected,
                AmountCredited = payment.AmountCredited,
                AmountRequested = payment.AmountRequested,
                BillingInfo = payment.BillingInfo,
                PaymentType = payment.PaymentType,
                OrderId = payment.OrderId,
                PaymentServiceTransactionId = payment.PaymentServiceTransactionId
            };
            return clone;
        }

        public static AubEvent ToAubEvent(this Event evt)
        {
            return new AubEvent
            {
                EntityId = evt.EntityId,
                Topic = evt.Topic,
                EventId = evt.Id,
                Id = String.Format("{0}-{1}", evt.EntityId, evt.Topic),
                QueuedDateTime = DateTime.Now
            };
        }

        public static Event ToMozuEvent(this AubEvent evt)
        {
            return new Event
            {
                EntityId = evt.EntityId,
                Topic = evt.Topic,
                Id = evt.EventId
            };
        }

        public static bool IsApproved(this Event evt)
        {
            var aubEvent = evt.ToAubEvent();
            return aubEvent.IsApproved();
        }
        public static bool IsApproved(this AubEvent evt)
        {
            return !String.IsNullOrEmpty(evt.Topic) && (evt.Topic.StartsWith("customeraccount.") || evt.Topic.Equals("order.opened"));
        }

        public static string GetOptionStringValue(this Product product, string optionName)
        {
            var option = product.GetOption(optionName);
            if (option == null) return String.Empty;
            var optValue = option.Values.FirstOrDefault();
            return optValue != null ? Convert.ToString(optValue.Value) : String.Empty;
        }

        public static bool GetOptionBooleanValue(this Product product, string optionName)
        {
            var option = product.GetOption(optionName);
            if (option == null) return false;
            var optValue = option.Values.FirstOrDefault();
            return optValue != null && Convert.ToString(optValue.Value) != "" && Convert.ToBoolean(optValue.Value);
        }

        public static decimal GetExtraValue(this Product product, string extraName)
        {
            var extra = product.GetExtra(extraName);
            if (extra == null) return 0;
            var extValue = extra.Values.FirstOrDefault();
            if (extValue == null) return 0;
            var deltaPrice = extValue.DeltaPrice;

            return deltaPrice != null ? deltaPrice.DeltaPrice : 0;
        }

        static string GetPropertyWithoutNamespace(this string propertyName)
        {
            var pNameWoNsArr = propertyName.Split('~');
            var pNameWoNs = String.Empty;
            if (pNameWoNsArr.Any() && pNameWoNsArr.Count() > 1)
            {
                pNameWoNs = String.Format("~{0}", pNameWoNsArr.LastOrDefault());
            }
            return pNameWoNs;
        }

        public static ProductProperty GetProperty(this Product product, string propertyName)
        {
            var pNameWoNs = propertyName.GetPropertyWithoutNamespace();
            return product.Properties == null ? null : product.Properties.FirstOrDefault(p => p.AttributeFQN.EndsWith(pNameWoNs));
        }

        public static ProductOption GetOption(this Product product, string optionName)
        {
            var optionNameWoNs = optionName.GetPropertyWithoutNamespace();
            return product.Options == null ? null : product.Options.FirstOrDefault(p => p.AttributeFQN.EndsWith(optionNameWoNs));
        }

        public static ProductExtra GetExtra(this Product product, string extraName)
        {
            var extraNameWoNs = extraName.GetPropertyWithoutNamespace();
            return product.Extras == null ? null : product.Extras.FirstOrDefault(p => p.AttributeFQN.EndsWith(extraNameWoNs));
        }

        public static IList<string> GetPropertyListValue(this Product product, string propertyName)
        {
            var prop = product.GetProperty(propertyName);
            if (prop == null) return new List<string>();
            var propValue = prop.Values;
            var vals = propValue.Select(v => Convert.ToString(v.Value)).ToList();


            return vals;
        }

        public static string GetPropertyStringLabel(this Product product, string propertyName)
        {
            var prop = product.GetProperty(propertyName);
            if (prop == null) return String.Empty;
            var propValue = prop.Values.FirstOrDefault();
            return propValue != null && propValue.AttributeVocabularyValueDetail != null ? Convert.ToString(propValue.AttributeVocabularyValueDetail.Content.StringValue) : String.Empty;
        }

        public static string GetPropertyStringValue(this Product product, string propertyName)
        {
            var prop = product.GetProperty(propertyName);
            if (prop == null) return String.Empty;
            var propValue = prop.Values.FirstOrDefault();
            return propValue != null ? Convert.ToString(propValue.Value) : String.Empty;
        }

        public static string GetPropertyValue(this Product product, string propertyName)
        {
            var prop = product.GetProperty(propertyName);
            if (prop == null) return String.Empty;
            var propValue = prop.Values.FirstOrDefault();
            return propValue != null ? Convert.ToString(propValue.Value) : String.Empty;
        }
        public static bool GetPropertyBooleanValue(this Product product, string propertyName)
        {
            var prop = product.GetProperty(propertyName);
            if (prop == null) return false;
            var propValue = prop.Values.FirstOrDefault();
            return propValue != null && (Convert.ToString(propValue.Value) == "1" || propValue.Value.Equals(true));
        }


        public static string BuildProductExportMessageAsync(this Product product, string providerCode, string xrefMerchantId)
        {
            return product != null ? product.ToEdgeProductExportMessage(providerCode, xrefMerchantId).ToXml() : String.Empty;
        }

        public static string BuildInventoryExportMessageAsync(this Product product, string providerCode, string xrefMerchantId, IList<LocationInventory> locations)
        {
            return product != null ? product.ToEdgeInventoryExportMessage(providerCode, xrefMerchantId, locations).ToXml() : String.Empty;
        }

        public static string BuildFileUploadExportMessageAsync(this List<ProductLocalizedImage> images, string providerCode, string xrefMerchantId)
        {
            return images != null ? images.ToEdgeFileuploadExportMessage(providerCode, xrefMerchantId).ToXml() : String.Empty;
        }

        public static async Task FillProductLocalizedImages(this Product product, IApiContext apiContext)
        {

            var productImages = new List<ProductLocalizedImage>();
            var images = product.Content.ProductImages;
            if (images == null) return;
            var documentResource = new DocumentResource(apiContext);
            foreach (var image in images)
            {
                var imageUrl = image.ImageUrl ??
                               String.Format("//cdn-sb.mozu.com/{0}-m1/cms/files/{1}", apiContext.TenantId,
                                   image.CmsId);

                var document = await documentResource.GetDocumentAsync("files@mozu", image.CmsId);
                productImages.Add(new ProductLocalizedImage
                {

                    CmsId = image.CmsId,
                    ImageLabel = document.Name,
                    ImageUrl = imageUrl.Replace(image.CmsId, document.Name)
                });
            }
            product.Content.ProductImages = productImages;
        } 
    }

    public static class ParallelExtensions
    {
        public static Task ForEachAsync<T>(this IEnumerable<T> source, int dop, Func<T, Task> body)
        {
            return Task.WhenAll(
                from partition in Partitioner.Create(source).GetPartitions(dop)
                select Task.Run(async delegate
                {
                    using (partition)
                        while (partition.MoveNext())
                            await body(partition.Current);
                }));
        }

        public static Task ForEachAsync<T>(this IEnumerable<T> source, Func<T, Task> body)
        {
            return Task.WhenAll(
                from item in source
                select Task.Run(() => body(item)));
        }
    }

    public static class OrderExtensions
    {
        public static OrderAttribute GetAttribute(this Order order, string attributeName)
        {
            return order.Attributes == null ? null : order.Attributes.FirstOrDefault(a => String.Equals(a.FullyQualifiedName, attributeName, StringComparison.CurrentCultureIgnoreCase));
        }

    }


    public static class AccountExtensions
    {
        public static async Task<bool> IsPreLiveCustomer(this CustomerAccount customerAccount, IApiContext apiContext)
        {
            return
                await CheckBooleanAttributeIsSet(customerAccount, apiContext, AttributeConstants.ExistingAccountPreLive);
        }

        public static bool IsPreLiveCustomer(this CustomerAccount customerAccount)
        {
            if (customerAccount.Attributes == null || !customerAccount.Attributes.Any())
                return false;

            var isPrelive =
                customerAccount.Attributes.Any(
                    a =>
                        a.FullyQualifiedName.Equals(AttributeConstants.ExistingAccountPreLive,
                            StringComparison.InvariantCultureIgnoreCase) && a.Values != null &&
                        a.Values.Any(v => v.Equals(true)));
            return isPrelive;
        }
        public static async Task<bool> IsEdgeImportCustomer(this CustomerAccount customerAccount, IApiContext apiContext)
        {
            return
              await CheckBooleanAttributeIsSet(customerAccount, apiContext, AttributeConstants.EdgeImportCustomer);
        }

        public static bool IsEdgeImportCustomer(this CustomerAccount customerAccount)
        {
            if (customerAccount.Attributes == null || !customerAccount.Attributes.Any())
                return false;

            var isEdgeImport =
                customerAccount.Attributes.Any(
                    a =>
                        a.FullyQualifiedName.Equals(AttributeConstants.EdgeImportCustomer,
                            StringComparison.InvariantCultureIgnoreCase) && a.Values != null &&
                        a.Values.Any(v => v.Equals(true)));
            return isEdgeImport;
        }

        public static CustomerAccount FormatAccount(this CustomerAccount account)
        {
            account.FirstName = account.FirstName.ToTitleCase();
            account.LastName = account.LastName.ToTitleCase();
            if (!account.Contacts.Any()) return account;
            foreach (var contact in account.Contacts)
            {
                contact.FirstName = contact.FirstName.ToTitleCase();
                contact.MiddleNameOrInitial = contact.MiddleNameOrInitial.ToTitleCase();
                contact.LastNameOrSurname = contact.LastNameOrSurname.ToTitleCase();
            }
            return account;
        }

        public static async Task<bool> IsRewardsAssigned(this CustomerAccount customerAccount, IApiContext apiContext)
        {
            var attributeResource = new AttributeResource(apiContext);
            var attribute = await attributeResource.GetAttributeAsync(AttributeConstants.RewardsAssigned);

            var customerAttributeResource = new CustomerAttributeResource(apiContext);
            var attributeCustomer =
                await customerAttributeResource.GetAccountAttributeAsync(customerAccount.Id, attribute.AttributeFQN);
            return attributeCustomer != null && attributeCustomer.Values.Any(a => a.Equals(true));
        }

        public static bool IsRewardsAssigned(this CustomerAccount customerAccount)
        {
            if (customerAccount.Attributes == null || !customerAccount.Attributes.Any())
                return false;

            var isEdgeImport =
                customerAccount.Attributes.Any(
                    a =>
                        a.FullyQualifiedName.Equals(AttributeConstants.RewardsAssigned,
                            StringComparison.InvariantCultureIgnoreCase) && a.Values != null &&
                        a.Values.Any(v => v.Equals(true)));
            return isEdgeImport;
        }
        public static async Task ResetEdgeImportFlag(this CustomerAccount customerAccount, IApiContext apiContext)
        {
            await UpdateBooleanAttribute(customerAccount, apiContext, AttributeConstants.EdgeImportCustomer, false);
        }

        public static async Task SetRewardsAssignedFlag(this CustomerAccount customerAccount, IApiContext apiContext)
        {
            await UpdateBooleanAttribute(customerAccount, apiContext, AttributeConstants.RewardsAssigned, true);
        }
        public static async Task ToEdgeImportCustomer(this CustomerAccount customerAccount, IApiContext apiContext)
        {
            await UpdateBooleanAttribute(customerAccount, apiContext, AttributeConstants.EdgeImportCustomer, true);
        }
        public static async Task ToPreLiveCustomer(this CustomerAccount customerAccount, IApiContext apiContext)
        {
            await UpdateBooleanAttribute(customerAccount, apiContext, AttributeConstants.ExistingAccountPreLive, true);
        }

        public static async Task ToPostLiveEdgeAccount(this CustomerAccount customerAccount, IApiContext apiContext)
        {
            await UpdateBooleanAttribute(customerAccount, apiContext, AttributeConstants.PostLiveEdgeAccount, true);
        }


        static async Task<bool> CheckBooleanAttributeIsSet(CustomerAccount customerAccount, IApiContext apiContext, string attributeName)
        {
            var attributeResource = new AttributeResource(apiContext);
            var attribute = await attributeResource.GetAttributeAsync(attributeName);
            attribute.AttributeFQN = attribute.AttributeFQN.ToLower();
            var customerAttributeResource = new CustomerAttributeResource(apiContext);
            var attributeCustomer =
                await customerAttributeResource.GetAccountAttributeAsync(customerAccount.Id, attribute.AttributeFQN);
            return attributeCustomer != null && attributeCustomer.Values.Any(a => a.Equals(true));
        }

        static async Task UpdateBooleanAttribute(CustomerAccount customerAccount, IApiContext apiContext,
            string attributeName, bool attributeValue)
        {
            var attributeResource = new AttributeResource(apiContext);
            var attribute = await attributeResource.GetAttributeAsync(attributeName);
            var customerAttributeResource = new CustomerAttributeResource(apiContext);
            var customerAttribute =
                await customerAttributeResource.GetAccountAttributeAsync(customerAccount.Id, attribute.AttributeFQN) ??
                new CustomerAttribute { AttributeDefinitionId = attribute.Id, FullyQualifiedName = attribute.AttributeFQN };
            attribute.AttributeFQN = attribute.AttributeFQN.ToLower();
            customerAttribute.Values = new List<object> { attributeValue };

            if (customerAccount.Attributes.All(s => s.AttributeDefinitionId != attribute.Id))
                await customerAttributeResource.AddAccountAttributeAsync(customerAttribute, customerAccount.Id, attribute.AttributeFQN);
            else
                await customerAttributeResource.UpdateAccountAttributeAsync(customerAttribute, customerAccount.Id, attribute.AttributeFQN);
        }
    }
}
