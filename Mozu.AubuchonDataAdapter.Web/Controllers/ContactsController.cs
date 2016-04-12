using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Mozu.Api;
using Mozu.Api.Resources.Commerce.Customer;
using Mozu.AubuchonDataAdapter.Domain;
using Mozu.AubuchonDataAdapter.Domain.Handlers;

namespace Mozu.AubuchonDataAdapter.Web.Controllers
{

    [AubCors(headers: "origin, accept, x-vol-user-claims, x-vol-master-catalog, x-vol-app-claims, x-vol-currency, x-vol-site, content-type, x-vol-tenant, x-vol-locale, x-vol-catalog,x-vol-accountid", methods: "POST")]
    //[EnableCors(origins: "*", headers: "*", methods: "POST")]
    [ApiContextFilter]
    public class ContactsController : ApiController
    {
        
        private readonly IAccountPhoneHandler _accountPhoneHandler;
        public ContactsController(IAccountPhoneHandler accountPhoneHandler)
        {
            _accountPhoneHandler = accountPhoneHandler;
            
        }

        [HttpPost]
        [ValidateUserClaimFilter]
        public async Task<HttpResponseMessage> Teams(int tenantId, int siteId, int accountId)
        {
            var accounResource = new CustomerAccountResource(new ApiContext(tenantId, siteId));
            var account = await accounResource.GetAccountAsync(accountId);
            var ht = "0";
            var htp = "0";
            if (account.Segments.Any())
            {
                ht = account.Segments.FirstOrDefault(s => s.Code.Equals(SegmentConstants.HomeTeam, StringComparison.InvariantCultureIgnoreCase)) != null ? "1" : "0";
                htp = account.Segments.FirstOrDefault(s => s.Code.Equals(SegmentConstants.HomeTeamPro, StringComparison.InvariantCultureIgnoreCase)) != null ? "1" : "0";
            }
            var teams = new { S1 = ht, S2 = htp };
            return Request.CreateResponse(HttpStatusCode.OK, teams);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> List(int tenantId, int siteId, string phoneNumber)
        {
            var apiContext = new ApiContext(tenantId, siteId);

            var customerAccountResource = new CustomerAccountResource(apiContext);
            var contactList = new List<object>();

            var contacts = await _accountPhoneHandler.GetAccountsWithPhone(tenantId, phoneNumber);
            if (!contacts.Any()) return Request.CreateResponse(HttpStatusCode.OK, contactList);
            await contacts.ForEachAsync(10, async contact =>
            {
                var account =
                    await
                        customerAccountResource.GetAccountAsync(contact.AccountId,
                            "Id,FirstName,LastName,EmailAddress,IsAnonymous,Attributes,Contacts,Segments");
                //if (!account.IsPreLiveCustomer()) return;
                var billingAddr = account.Contacts.Any()
                    ? account.Contacts.FirstOrDefault(
                        c =>
                            c.Types.Any(
                                t =>
                                    t.IsPrimary && t.Name.Equals("Billing", StringComparison.InvariantCultureIgnoreCase)))
                    : null
                    ;

                //Pick the first address if there is no primary billing address
                if (billingAddr == null && account.Contacts.Any())
                {
                    billingAddr = account.Contacts.First();
                }

                var homeUser =
                    account.Segments.FirstOrDefault(
                        s => s.Code.Equals(SegmentConstants.HomeTeam, StringComparison.InvariantCultureIgnoreCase)) !=
                    null
                        ? "1"
                        : "0";
                var homeUserPro =
                    account.Segments.FirstOrDefault(
                        s => s.Code.Equals(SegmentConstants.HomeTeamPro, StringComparison.InvariantCultureIgnoreCase)) !=
                    null
                        ? "1"
                        : "0";
                var ct =
                    new
                    {
                        Id = contact.AccountId,
                        S = new[] {homeUser, homeUserPro},
                        FN = account.FirstName,
                        LN = account.LastName,
                        EM = account.EmailAddress,
                        R = account.IsAnonymous ? "0" : "1",
                        CY = billingAddr != null ? billingAddr.Address.CityOrTown : "",
                        ST = billingAddr != null ? billingAddr.Address.StateOrProvince : ""
                    };

                contactList.Add(ct);
            });

            return Request.CreateResponse(HttpStatusCode.OK, contactList);
        }
        [HttpPost]
        public async Task<HttpResponseMessage> External(Models.ExternalLookupModel model)
        {
            try
            {
                object apiCtxtObj;

                Request.Properties.TryGetValue("ApiContext", out apiCtxtObj);

                var apiContext = (ApiContext)apiCtxtObj;

                //var apiContext = new ApiContext(model.TenantId, siteId: model.SiteId);
                var customerResource = new Mozu.Api.Resources.Commerce.Customer.CustomerAccountResource(apiContext);

                if (String.IsNullOrWhiteSpace(model.AubuchonId))
                    throw new Exception("Missing customer code");

                var customers = await customerResource.GetAccountsAsync(filter: string.Format("ExternalId eq '{0}'", model.AubuchonId));

                if (customers.TotalCount != 1)
                    throw new Exception("Error looking up code");

                var customer = customers.Items.FirstOrDefault();
                if (customer == null)
                    throw new Exception("Not found");

                 if (!customer.IsAnonymous)
                   throw new Exception("Customer already registered");

                return Request.CreateResponse(HttpStatusCode.OK,
                    new
                    {
                        id = customer.Id,
                        firstName = customer.FirstName,
                        email = customer.EmailAddress
                    });
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Associate(Models.AssociateAccountModel model)
        {
            try
            {
                object apiCtxtObj;
                object accountIdObj;

                Request.Properties.TryGetValue("ApiContext", out apiCtxtObj);
                Request.Properties.TryGetValue("AccountId", out accountIdObj);

                var apiContext = (ApiContext)apiCtxtObj;

                if (accountIdObj == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "AccountId is missing");
                }


                var accountId = Convert.ToInt32(accountIdObj);

               
                var customerResource = new CustomerAccountResource(apiContext);

                
                var customer = await customerResource.GetAccountAsync(accountId);
                //get accounts with user passed email and id is not equal to current user
                var existingCustomerResults = await customerResource.GetAccountsAsync(filter: String.Format("EmailAddress eq '{0}'", model.EmailAddress));

                //filter collection for non anon account (or should we allow this?)
                var existingCustomer = existingCustomerResults.Items.FirstOrDefault(c => !c.IsAnonymous && c.Id != customer.Id);
                
                if(existingCustomer != null)
                    throw new Exception("Email in use.");

                var customerLoginInfo = new Api.Contracts.Customer.CustomerLoginInfo()
                {
                    EmailAddress = model.EmailAddress,
                    Password = model.Password,
                    Username = model.EmailAddress,
                    IsImport = false
                };

                //Check if login already exists so we don't override an existing acct
                if (!customer.IsAnonymous)
                    throw new Exception("Login already exists");
                //force email address to update
                customer.EmailAddress = model.EmailAddress;
                
                var updatedCustomer = await customerResource.UpdateAccountAsync(customer, customer.Id, "Id");
                var result = await customerResource.AddLoginToExistingCustomerAsync(customerLoginInfo, accountId);
                
                return Request.CreateResponse(HttpStatusCode.OK, result.CustomerAccount.EmailAddress);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }

        }
    }
}
