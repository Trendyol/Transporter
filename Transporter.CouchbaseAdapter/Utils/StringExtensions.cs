namespace Transporter.CouchbaseAdapter.Utils
{
    public static class StringExtensions
    {
        public static string SurroundWith(this string text, string ends) => ends + text + ends;
    }
}