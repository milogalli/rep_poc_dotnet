using System;
using System.Net.Mail;

namespace MicroServicePoC.Utilities
{
    public static class ValidationHelpers
    {
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidGuid(string guid)
        {
            return Guid.TryParse(guid, out _);
        }
    }
}