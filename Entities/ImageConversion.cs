using System;
using System.Net;

namespace pokemon.Entities
{
    public class ImageConversion
    {
        public static string GetImageBase64ByUrlEncode(string url)
        {
            WebClient webClientencode = new WebClient();
            return Convert.ToBase64String(webClientencode.DownloadData(url));
        }

        public static byte[] GetImageBase64ByUrlDecode(string url)
        {
            WebClient webClientdecode = new WebClient();
            return Convert.FromBase64String(url);
        }
    }
}
