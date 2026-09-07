namespace AutoRent.Services
{
    public static class PrivacyHelper
    {
        public static string MaskContact(string contact)
        {
            if (string.IsNullOrWhiteSpace(contact) || contact.Length <= 3)
            {
                return "***";
            }

            var visible = contact.Substring(contact.Length - 3);
            return new string('*', contact.Length - 3) + visible;
        }
    }
}