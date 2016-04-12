using System;
using System.Globalization;
using System.Linq;
using Mozu.Api.Contracts.ProductAdmin;
using Mozu.AubuchonDataAdapter.Domain.Contracts;

namespace Mozu.AubuchonDataAdapter.Domain.Mappers
{
    public static class ProductMapper
    {
        public static CatalogMessage ToEdgeProductExportMessage(this Product product, string providerCode, string xrefMerchantId)
        {

            var message = new CatalogMessage();

            var head = new MessageHead
            {
                Sender = new MessageHeadSender[1],
                Recipient = new MessageHeadRecipient[1],
                Messageid = Guid.NewGuid().ToString(),
                Messagetype = "Catalogimport",
                Date = DateTime.Now.ToString(CultureInfo.InvariantCulture)
            };
            var sender = new MessageHeadSender { Systemid = providerCode };
            head.Sender[0] = sender;

            var recipient = new MessageHeadRecipient { Companyid = xrefMerchantId };
            head.Recipient[0] = recipient;
            message.Items = new object[2];
            message.Items[0] = head;
            
            var body = new CatalogMessageBody { Catalogimport = new MessageBodyCatalogimport[1] };
            
            var status = StatusCode.Active;
            var prodInCatatlog = product.ProductInCatalogs.FirstOrDefault();
            if (prodInCatatlog != null)
            {
                status = prodInCatatlog.IsActive.GetValueOrDefault() ? StatusCode.Active : StatusCode.Inactive;
            }

            var taxcode = product.GetPropertyValue(AttributeConstants.TaxCode);
            var catalogImportField = new MessageBodyCatalogimport
            {
                Skus = new[] { new MessageBodyCatalogimportSkus { Sku = product.ProductCode } },
                Itemtypename = String.Empty,
                Uomname = String.Join(",",product.GetPropertyListValue(AttributeConstants.UnitOfMeasure)),
                Filenames = new MessageBodyCatalogimportFilenamesFilename[] { },
                Projects = new[] { new MessageBodyCatalogimportProjectsProject { Value = "Aubuchon" } },
                Categories = new MessageBodyCatalogimportCategoriesCategoryCode[] { },
                Prerequisites = new Prerequisites[] { },
                Deliverymethodcodes = new[] { new MessageBodyCatalogimportDeliverymethodcodes { Deliverymethodcode = "Ship" } },
                PriceLists = new MessageBodyCatalogimportPriceListsPriceList[] { },
                Dynamicfields = new MessageBodyCatalogimportDynamicfieldsDynamicfield[3],
                CatalogTaxClassCode = String.IsNullOrEmpty(taxcode) ? "P0000000" : taxcode,
                Catalogitem = product.ProductCode,
                Catalogname = product.Content.ProductName,
                Catalognotes = String.Empty,
                Shortdescription = System.Security.SecurityElement.Escape(product.Content.ProductShortDescription),
                Longdescription = System.Security.SecurityElement.Escape(product.Content.ProductFullDescription),
                Retailprice = Convert.ToString(product.Price.Price),
                Downloadable = "0",
                Taxable = product.IsTaxable.GetValueOrDefault() ? "1" : "0",
                Minorderqty = String.Empty,
                Maxorderqty = String.Empty,
                Iselectronic = "0",
                Allowautoship = "0",
                Chargeshipping = "1",
                Taxresellers = "1",
                Budgetitem = "0",
                Startdate = DateTime.Today.ToString("yyyy-MM-dd"),
                Searchable = "1",
                Statuscode = Enum.GetName(typeof(StatusCode), status),
                Custodianusername = "defaultAdmin",
                Defaultdeliverymethodcode = "Ship",
                Shippingamount = product.GetPropertyStringValue(AttributeConstants.ShippingSurcharge) ?? String.Empty,
                Handlingamount = String.Empty,
                Salesaccount = String.Empty,
                Sortorder = String.Empty,//TODO:What should be the value?
                Enddate = String.Empty,
                Catalogcode = String.Empty,
                Dutytax = String.Empty,
                Orderquantity = String.Empty
            };



            if (product.Content.ProductImages != null)
            {
                catalogImportField.Filenames =
                    new MessageBodyCatalogimportFilenamesFilename[product.Content.ProductImages.Count];



                for (var i = 0; product.Content.ProductImages != null && i < product.Content.ProductImages.Count; i++)
                {
                    var fileName = new MessageBodyCatalogimportFilenamesFilename
                    {
                        Value = product.Content.ProductImages[i].ImageLabel

                    };

                    if (i == 0) fileName.Default = "true";
                    catalogImportField.Filenames[i] = fileName;
                }
            }

            var freeAssembly = product.GetExtraValue(AttributeConstants.FreeAssembly);
            var instantReward = product.GetPropertyStringValue(AttributeConstants.InstantRewards) ?? String.Empty;
            var instantRewardAdmin = product.GetPropertyStringValue(AttributeConstants.InstantRewardsAdmin) ?? String.Empty;
            for (var i = 0; i < 3; i++)
            {
                if (freeAssembly > 0)
                {
                    catalogImportField.Dynamicfields[i] = new MessageBodyCatalogimportDynamicfieldsDynamicfield
                    {
                        Dynamicfieldcode = "assembleable",
                        DynamicFieldLabel = "Eligible for Assembly?",
                        Dynamicfieldvalue = Convert.ToString(freeAssembly)
                    };
                    i++;
                }
                if (!String.IsNullOrEmpty(instantReward))
                {
                    catalogImportField.Dynamicfields[i] = new MessageBodyCatalogimportDynamicfieldsDynamicfield
                    {
                        Dynamicfieldcode = "instant-reward",
                        DynamicFieldLabel = "Instant Reward",
                        Dynamicfieldvalue = instantReward
                    };
                    i++;
                }
                if (String.IsNullOrEmpty(instantRewardAdmin)) continue;
                catalogImportField.Dynamicfields[i] = new MessageBodyCatalogimportDynamicfieldsDynamicfield
                {
                    Dynamicfieldcode = "instant_reward_admin_catalog",
                    DynamicFieldLabel = "Instant Reward Admin",
                    Dynamicfieldvalue = instantRewardAdmin
                };
                i++;
            }



            body.Catalogimport[0] = catalogImportField;

            message.Items[1] = body;

            return message;
        }
    }
}
