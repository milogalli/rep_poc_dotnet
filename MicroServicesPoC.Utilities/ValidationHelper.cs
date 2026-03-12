using System.Net.Mail;

namespace MicroServicePoC.Utilities
{
    public static class ValidationHelpers
    {
        public static bool IsValidEmail(string email)
        {
            return !string.IsNullOrWhiteSpace(email);
        }
    }
}
