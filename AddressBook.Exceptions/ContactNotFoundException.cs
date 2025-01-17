﻿
namespace AddressBook.ORM.OrmLayer
{
    [Serializable]
    public class ContactNotFoundException : Exception
    {
        public ContactNotFoundException()
        {
        }

        public ContactNotFoundException(string? message) : base(message)
        {
        }

        public ContactNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}