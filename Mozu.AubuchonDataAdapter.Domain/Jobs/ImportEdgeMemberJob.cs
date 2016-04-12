using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.IO;
using Quartz;
using Common.Logging;
using Mozu.Api;
using Mozu.AubuchonDataAdapter.Domain.Handlers;
using Mozu.Api.Resources.Commerce.Customer;
using Mozu.Api.Contracts.Customer;
using Mozu.Api.Resources.Commerce.Customer.Accounts;
using Mozu.Api.ToolKit.Config;

namespace Mozu.AubuchonDataAdapter.Domain.Jobs
{
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    public class ImportEdgeMemberJob : BaseInterruptableJob
    {
        public enum AddressTypes
        {
            Billto,
            Shipto
        }

        private readonly ILog _logger = LogManager.GetLogger(typeof(ImportEdgeMemberJob));
        private readonly IAppSetting _appSetting;
        private readonly IAubuchonAccountHandler _aubuchonAccountHandler;
        private bool _isLive;
        public ImportEdgeMemberJob(IAppSetting appSetting, ILogHandler logHandler, IAubuchonAccountHandler aubuchonAccountHandler)
            : base(logHandler)
        {
            _appSetting = appSetting;
            _aubuchonAccountHandler = aubuchonAccountHandler;
        }

        public override async void ExecuteCore(IJobExecutionContext context)
        {
            _isLive = _appSetting.Settings.ContainsKey("IsLive") && Convert.ToBoolean(_appSetting.Settings["IsLive"]);
            _logger.Info("Edge Member Import Job");
            _logger.Info("IsLive " + _isLive);

            var processedMembers = new ConcurrentBag<String>();
            var tenantId = 0;
            var siteId = 0;
            if (context.MergedJobDataMap.Count > 0)
            {
                tenantId = int.Parse(context.MergedJobDataMap["tenantId"].ToString());
                siteId = int.Parse(context.MergedJobDataMap["siteId"].ToString());
            }
            var apiContext = new ApiContext(tenantId, siteId);




            try
            {
                var sftpHandler = new SftpHandler(_appSetting);
                var processedFiles = new List<string>(Directory.GetFiles(ProcessedFolder).Select(Path.GetFileName)
                               .ToArray());

                if (!processedFiles.Any())
                {
                    _logger.Info("Processing files from sftp/local");
                    var customerContactResource = new CustomerContactResource(apiContext);
                    var customerAccountResource = new CustomerAccountResource(apiContext);

                    var fileNames =
                        new List<string>(Directory.GetFiles(EdgeImportTempDirectory).Select(Path.GetFileName)
                            .ToArray());

                    if (!fileNames.Any())
                    {
                        fileNames = new List<string>();
                        sftpHandler.GetFiles(EdgeImportTempDirectory
                            , EdgeFtpHostDirectory
                            , fileNames);
                        _logger.Info(String.Format("Downloaded {0} files from SFTP", fileNames.Count));
                    }
                    else
                    {

                        _logger.Info(String.Format("Found {0} downloaded files to process", fileNames.Count));
                    }


                    #region Process files

                    await fileNames.ForEachAsync(15, async file =>
                    //foreach (var file in fileNames)
                    {
                        try
                        {
                            var serializer = new XmlSerializer(typeof(EdgeMessage));
                            var reader = new StreamReader(String.Format("{0}{1}", EdgeImportTempDirectory, @file));
                            var edgeMessage = (EdgeMessage)serializer.Deserialize(reader);
                            reader.Close();

                            var memberBody =
                                (EdgeMessageBody)
                                    edgeMessage.Items.Where(em => em is EdgeMessageBody)
                                        .Select(em => em)
                                        .FirstOrDefault();
                            if (memberBody == null) return;

                            var member = memberBody.Member.Any() ? memberBody.Member.First() : null;
                            if (member == null) return;

                            //var attributeResource = new AttributeResource(apiContext);
                            //var edgeImportAttribute = await attributeResource.GetAttributeAsync(AttributeConstants.EdgeImportCustomer);

                            //foreach (var member in memberBody.Member.AsEnumerable())
                            //{
                            var memberNumber = member.memberNumber;
                            var customerAccount = new CustomerAccount();

                            if (String.IsNullOrEmpty(memberNumber))
                            {
                                _logger.Info(String.Format("Member number is missing. Member Id: {0}", member.memberID));
                            }
                            else
                            {
                                if (processedMembers.Any() && processedMembers.Any(f => f.Equals(memberNumber))) return;
                                processedMembers.Add(memberNumber);
                                var accounts =
                                    await
                                        customerAccountResource.GetAccountsAsync(filter:
                                            String.Format("ExternalId eq {0}", memberNumber.Trim()));
                                customerAccount = accounts.Items.FirstOrDefault() ?? new CustomerAccount();
                            }


                            //Removed this line to ensure that Imported edge customers are added to mozu
                            //even if they dont have Aubuchon Id
                            //if (customerAccount.Id != 0)
                            //{
                            //    var customer = await customerAccountResource.GetAccountAsync(customerAccount.Id, "Id");
                            //    if (customer == null)
                            //    {
                            //        continue;
                            //    }
                            //}


                            customerAccount.FirstName = member.nameFirst.ToTitleCase();
                            customerAccount.LastName = member.nameLast.ToTitleCase();
                            customerAccount.EmailAddress = member.email;
                            customerAccount.TaxId = member.taxNumber;
                            if (!String.IsNullOrEmpty(member.taxNumber))
                            {
                                customerAccount.TaxExempt = true;
                            }
                            customerAccount.CompanyOrOrganization = member.organization;
                            customerAccount.LocaleCode = member.locale;
                            customerAccount.ExternalId = member.memberNumber;
                            //customerAccount.Attributes = new List<CustomerAttribute>
                            //    {
                            //        new CustomerAttribute
                            //        {
                            //            AttributeDefinitionId = edgeImportAttribute.Id,
                            //            FullyQualifiedName = AttributeConstants.EdgeImportCustomer,
                            //            Values = new List<object> {"true"}
                            //        }
                            //    };



                            if (customerAccount.Id != 0)
                            {
                                // Set the “ImportEdgeCustomer” attribute to true
                                //await customerAccount.ToEdgeImportCustomer(apiContext);
                                await
                                    customerAccountResource.UpdateAccountAsync(customerAccount, customerAccount.Id,
                                        "Contacts");
                            }
                            else
                            {

                                var accounts =
                                   await
                                       customerAccountResource.GetAccountsAsync(filter:
                                           String.Format("ExternalId eq {0}", memberNumber.Trim()));

                                if (accounts.Items.Any()) return;

                                customerAccount = await customerAccountResource.AddAccountAsync(customerAccount);
                                // Set the “ImportEdgeCustomer” attribute to true
                                //await customerAccount.ToEdgeImportCustomer(apiContext);
                                customerAccount = await customerAccountResource.GetAccountAsync(customerAccount.Id);
                                //Missing attributes in the previous return value
                                /*if (_isLive)
                                {
                                    var customerLoginInfo = new CustomerLoginInfo
                                    {
                                        Username = member.username,
                                        EmailAddress = member.email,
                                        IsImport = true,
                                        Password = GeneratePassword()
                                    };
                                    await
                                        customerAccountResource.AddLoginToExistingCustomerAsync(customerLoginInfo,
                                            customerAccount.Id);

                                }*/

                            }

                            //Add segments if any
                            foreach (var group in member.groups)
                            {
                                await
                                    _aubuchonAccountHandler.AddToSegment(apiContext, customerAccount.Id,
                                        group.groupCode.ToUpper());
                            }


                            // Set the “ImportEdgeCustomer” attribute to true
                            await customerAccount.ToEdgeImportCustomer(apiContext);

                            if (!await customerAccount.IsPreLiveCustomer(apiContext))
                            {
                                if (!_isLive)
                                {
                                    await customerAccount.ToPreLiveCustomer(apiContext);
                                }
                                else
                                {
                                    await customerAccount.ToPostLiveEdgeAccount(apiContext);
                                }
                            }


                            //Add or update contacts
                            foreach (var contact in customerAccount.Contacts)
                            {
                                await customerContactResource.DeleteAccountContactAsync(customerAccount.Id, contact.Id);
                            }
                            foreach (var address in member.addresses)
                            {
                                //var addressType = address.addressObjTypeCode;

                                var customerContact = new CustomerContact
                                {
                                    AccountId = customerAccount.Id,
                                    Address = new Api.Contracts.Core.Address(),
                                    PhoneNumbers = new Api.Contracts.Core.Phone(),
                                    AuditInfo = new Api.Contracts.Core.AuditInfo(),
                                    Types = new List<ContactType>
                                    {
                                        new ContactType
                                        {
                                            Name =
                                                address.addressObjTypeCode.Equals("BillTo",
                                                    StringComparison.CurrentCultureIgnoreCase)
                                                    ? "Billing"
                                                    : "Shipping",
                                            IsPrimary =
                                                address.primaryForType.Equals("true",
                                                    StringComparison.InvariantCultureIgnoreCase)
                                        }
                                    }
                                };
                                //if (addressType.Equals("Billto", StringComparison.InvariantCultureIgnoreCase))
                                //{
                                //    customerContact =
                                //               customerAccount.Contacts.FirstOrDefault(
                                //                   a => (a.Types != null && a.Types.Any(t => t != null && t.Name == "Billing")));
                                //}
                                //else if (addressType.Equals("Shipto", StringComparison.InvariantCultureIgnoreCase))
                                //{
                                //    customerContact =
                                //               customerAccount.Contacts.FirstOrDefault(
                                //                   a => (a.Types != null && a.Types.Any(t => t != null && t.Name == "Shipping")));
                                //}

                                //if (customerContact == null && customerAccount.Contacts.Any())
                                //    customerContact = customerAccount.Contacts.FirstOrDefault();





                                customerContact.Address.Address1 = address.address1;
                                customerContact.Address.Address2 = address.address2;
                                customerContact.Address.Address3 = address.address3;
                                customerContact.Address.CityOrTown = address.city;
                                customerContact.Address.StateOrProvince = address.stateProvinceCode;
                                customerContact.Address.CountryCode = address.countryCode;
                                customerContact.Address.PostalOrZipCode = address.postalCode;
                                customerContact.Email = address.email;
                                customerContact.FirstName = address.nameFirst.ToTitleCase();
                                customerContact.LastNameOrSurname = address.nameLast.ToTitleCase();
                                customerContact.MiddleNameOrInitial = address.nameMiddle.ToTitleCase();
                                customerContact.CompanyOrOrganization = address.organization;
                                customerContact.PhoneNumbers.Home = address.phoneHome;
                                customerContact.PhoneNumbers.Mobile = address.phoneCell;
                                customerContact.PhoneNumbers.Work = address.phoneWork;
                                customerContact.FaxNumber = address.fax;
                                customerContact.Address.AddressType = bool.Parse(address.isResidential)
                                    ? "Residential"
                                    : "";
                                customerContact.Label = address.salutation;
                                customerContact.AuditInfo.CreateDate = Convert.ToDateTime(address.createDate);
                                customerContact.AuditInfo.UpdateDate = Convert.ToDateTime(address.lastUpdated);

                                //if (customerContact.Id != 0)
                                //    await
                                //        customerContactResource.UpdateAccountContactAsync(customerContact,
                                //            customerAccount.Id, customerContact.Id);
                                //else
                                await
                                    customerContactResource.AddAccountContactAsync(customerContact,
                                        customerAccount.Id);
                            }


                            MoveProcessedFile(file);

                            //processedFiles.Add(file);




                            //}
                        }

                        catch (Exception ex)
                        {
                            _logger.Error(String.Format("Error processing files in EdgeMemberImport: {0}", file), ex);

                            var destinationFile = String.Format(@"{0}\{1}", FailedFolder, file);

                            if (File.Exists(destinationFile))
                                File.Delete(destinationFile);

                            File.Move(
                                String.Format("{0}{1}", EdgeImportTempDirectory, file),
                                destinationFile
                                );
                        }
                    });
                }

                    #endregion

                #region Deleting files
                processedFiles = new List<string>(Directory.GetFiles(ProcessedFolder).Select(Path.GetFileName)
                                 .ToArray());

                _logger.Info(String.Format("Deleting {0} processed files", processedFiles.Count));


                sftpHandler.RemoveFiles(processedFiles, EdgeFtpHostDirectory, ProcessedFolder);




                var failedFiles = new List<string>(Directory.GetFiles(FailedFolder).Select(Path.GetFileName)
                                      .ToArray());
                var ftpFailedFolder = String.Format("{0}/failed", EdgeFtpHostDirectory);
                sftpHandler.MoveFiles(failedFiles, EdgeFtpHostDirectory, ftpFailedFolder, FailedFolder);
                #endregion

                _logger.Info("Clearing processed members");
                //Clear processed members list
                while (!processedMembers.IsEmpty)
                {
                    string pitem;
                    processedMembers.TryTake(out pitem);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error in EdgeMemberImport", ex);
                //throw;
            }



            //context.JobDetail.JobDataMap.Put("ProcessedDownloadedFiles", true);
            //Logger.Info("Processed Downloaded Files" + context.MergedJobDataMap.GetBooleanValue("ProcessedDownloadedFiles"));
        }

        private void MoveProcessedFile(string file)
        {
            var processedFile = String.Format(@"{0}\{1}", ProcessedFolder, file);
            //_logger.Info(String.Format("Moving processed file {0}, Processed File exists {1}, Original file exists {2}", processedFile, File.Exists(processedFile), File.Exists(file)));

            if (File.Exists(processedFile))
                File.Delete(processedFile);


            File.Move(
                String.Format("{0}{1}", EdgeImportTempDirectory, file),
                processedFile
                );
        }

        public override void InterruptCore()
        {

        }



        private static string GeneratePassword()
        {
            var characters = CreateRandomPassword("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmonpqrstuvwxyz", 8);
            var numbers = CreateRandomPassword("0123456789", 4);
            return String.Format("{0}{1}", characters, numbers);
        }

        private static string CreateRandomPassword(string chars, int passwordLength)
        {
            var random = new Random();
            return new string(Enumerable.Repeat(chars, passwordLength)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }

}
