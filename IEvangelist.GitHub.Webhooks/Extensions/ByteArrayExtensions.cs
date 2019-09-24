namespace IEvangelist.GitHub.Webhooks.Extensions
{
    static class ByteArrayExtensions
    {
        internal static string ToHexString(this byte[] bytes)
        {
            var builder = new System.Text.StringBuilder(bytes.Length * 2);
            foreach (var b in bytes)
            {
                builder.AppendFormat("{0:x2}", b);
            }

            return builder.ToString();
        }
    }
}