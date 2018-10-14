using System;
using System.Security.Cryptography;
using System.Text;

namespace docker_netgen.Utils
{
    public static class StringExtensions
    {
        public static string Sha1(this string input)
        {
            var enc = Encoding.UTF8;
            var buffer = enc.GetBytes(input);

            var hasher = SHA1.Create();
            return enc.GetString(hasher.ComputeHash(buffer));
        }
    }
}