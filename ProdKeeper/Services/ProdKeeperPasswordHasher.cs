using CertificateManager;
using EncryptDecryptLib;
using Microsoft.AspNetCore.Identity;
using ProdKeeper.Models;
using SMBLibrary.Authentication.NTLM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProdKeeper.Services
{
    public class ProdKeeperPasswordHasher<TUser> : IPasswordHasher<TUser> where TUser : class
    {
        public ProdKeeperPasswordHasher()
        {
        }
        public string HashPassword(TUser user, string password)
        {
            ApplicationUser appUser = user as ApplicationUser;
            if (appUser == null)
                throw new ArgumentException("User must be application user type");

            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password)); 
        }

        public PasswordVerificationResult VerifyHashedPassword(TUser user, string hashedPassword, string providedPassword)
        {
            ApplicationUser appUser = user as ApplicationUser;
            if (appUser == null)
                throw new ArgumentException("User must be application user type");
            var passwordHash = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(providedPassword));
            if (passwordHash == hashedPassword)
                return PasswordVerificationResult.Success;
            else
                return PasswordVerificationResult.Failed;
        }
    }
}
