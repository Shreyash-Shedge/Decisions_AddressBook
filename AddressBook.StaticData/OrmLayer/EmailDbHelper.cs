﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AddressBook.Models.Models;
using AddressBook.ORM.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace AddressBook.ORM.OrmLayer
{
    public class EmailDbHelper : IEmailDbHelper
    {
        private readonly IGenericHelper _genericDbHelper;
        private readonly IDbHelper _dbHelper;

        public EmailDbHelper(IGenericHelper genericHelper, IDbHelper dbHelper)
        {
            _genericDbHelper = genericHelper;
            _dbHelper = dbHelper;
        }

        public List<EmailAddress> FetchEmailsForContact(string contactId)
        {
            var contact = _dbHelper.FetchContactById(contactId);
            if (contact == null)
            {
                throw new ArgumentNullException("Contact object is null or empty");
            }

            string emailQuery = "SELECT * FROM EmailAddresses where IsDeleted=0 AND ContactId=@ContactId";
            var emails = _genericDbHelper.FetchData<EmailAddress>(emailQuery, MapEmailAddress, new SqlParameter("@ContactId", contactId));

            return emails;
        }
        private EmailAddress FetchEmailAddressById(string emailId)
        {
            string emailQuery = "SELECT * FROM EmailAddresses where Id=@Id";
            var emailAddresses = _genericDbHelper.FetchData<EmailAddress>(emailQuery, MapEmailAddress, new SqlParameter("@Id", emailId));

            return emailAddresses.FirstOrDefault()!;
        }
        public void AddEmailAddress(EmailAddress emailAddress)
        {
            if (emailAddress == null)
            {
                throw new ArgumentNullException("Emailaddress object cannot be null or ");
            }
            string addEmailQuery = "INSERT INTO EmailAddresses (Id, ContactId, EmailType, Address)" +
                                                     "VALUES (@Id, @ContactId, @EmailType, @Address);";

            _genericDbHelper.AddRecord(addEmailQuery,
                new SqlParameter("@Id", emailAddress.Id),
                new SqlParameter("@ContactId", emailAddress.ContactId),
                new SqlParameter("@EmailType", emailAddress.EmailType),
                new SqlParameter("@Address", emailAddress.Address));
        }
        public void UpdateEmailAddress(EmailAddress emailAddress)
        {
            if(emailAddress == null)
            {
                throw new ArgumentNullException("Emailaddress object cannot be null or empty");
            }
            string updateEmailQuery = @"UPDATE EmailAddresses
                                        SET Address=@Address,
                                            EmailType=@EmailType
                                            Where Id=@Id AND ContactId=@ContactId";

            _genericDbHelper.UpdateRecord(updateEmailQuery,
                new SqlParameter("@Id", emailAddress.Id),
                new SqlParameter("@ContactId", emailAddress.ContactId),
                new SqlParameter("@Address", emailAddress.Address),
                new SqlParameter("@EmailType", emailAddress.EmailType));
        }
        public void DeleteEmailAddress(string emailId, string contactId)
        {
            var contact = _dbHelper.FetchContactById(contactId);
            if(contact == null)
            {
                throw new ArgumentNullException("Contact object is null or empty");
            }
            var emailAddress = FetchEmailAddressById(emailId);
            string updateEmailQuery = "UPDATE EmailAddresses SET IsDeleted=1 Where Id=@Id AND ContactId=@ContactId";
            _genericDbHelper.DeleteRecord(updateEmailQuery, new SqlParameter("@Id", emailId), new SqlParameter("@ContactId", contactId));
        }
        public void RestoreEmailAddress(string emailId, string contactId)
        {
            var emailAdderss = FetchEmailAddressById(emailId);

            if (emailAdderss == null)
            {
                throw new ArgumentNullException("Object is null or email address does not exist");
            }

            string updateEmailAddressQuery = "UPDATE EmailAddresses SET IsDeleted = 0 WHERE Id = @Id AND ContactId=@ContactId";
            _genericDbHelper.DeleteRecord(updateEmailAddressQuery, new SqlParameter("@Id", emailId), new SqlParameter("@ContactId", contactId));
        }
        private EmailAddress MapEmailAddress(SqlDataReader reader)
        {
            return new EmailAddress
            {
                Id = reader["Id"].ToString(),
                ContactId = reader["ContactId"].ToString(),
                Address = reader["Address"].ToString(),
                EmailType = Enum.TryParse<EmailType>(reader["EmailType"].ToString(), out var EmailType) ? EmailType : EmailType.Other
            };
        }
    }
}
