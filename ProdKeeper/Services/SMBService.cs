using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PocSMB.Adapters;
using ProdKeeper.Data;
using SMBLibrary.Authentication.GSSAPI;
using SMBLibrary.Authentication.NTLM;
using SMBLibrary.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProdKeeper.Services
{
    public class SMBService : BackgroundService
    {
        private readonly ILogger<SMBService> _logger;
        private readonly ApplicationDbContext _dbContext;

        public SMBService(ILogger<SMBService> logger, IServiceScopeFactory factory)
        {
            _dbContext = factory.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            NTLMAuthenticationProviderBase authenticationMechanism = new IndependentNTLMAuthenticationProvider(GetUserPassword);
            SMBShareCollection shares = new SMBShareCollection();
            FileSystemShare share = new FileSystemShare("Json", new JSONFileSystemAdapter());
            shares.Add(share);
            GSSProvider securityProvider = new GSSProvider(authenticationMechanism);
            SMBServer server = new SMBServer(shares, securityProvider);
            server.Start(System.Net.IPAddress.Any, SMBLibrary.SMBTransportType.DirectTCPTransport, false, true, true);
            while (!stoppingToken.IsCancellationRequested)
            {


            }
        }
        public string GetUserPassword(string accountName)
        {
            var password = (from u in _dbContext.Users where u.Email == accountName select u.PasswordHash).FirstOrDefault();
            if (password==null)
                return null;
            return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(password));
        }
    }
}
