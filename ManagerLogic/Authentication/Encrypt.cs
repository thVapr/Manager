using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace ManagerLogic.Authentication
{
    public class Encrypt : IEncrypt
    {
        public string HashPassword(string password, string salt)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password,
                Encoding.ASCII.GetBytes(salt),
                KeyDerivationPrf.HMACSHA512,
                500,
                64)) ;
        }
    }
}
