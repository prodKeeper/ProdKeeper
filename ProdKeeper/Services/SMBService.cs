using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PocSMB.Adapters;
using ProdKeeper.Entity.Data;
using ProdKeeper.VirtualFileSystem;
using SMBLibrary.Adapters;
using SMBLibrary.Authentication.GSSAPI;
using SMBLibrary.Authentication.NTLM;
using SMBLibrary.Server;
using System;
using System.Collections.Generic;
using System.IO;
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
            FileSystemOption fileoption = new FileSystemOption(_dbContext);
            fileoption.StorePath = "c:\\Temp\\FileRepo";
            var fileSystemService = new FileSystemService(fileoption);
            var pka = new ProdKeeperSMBAdapter(fileSystemService);
            FileSystemShare share = new FileSystemShare("ProdKeeper",new NTFileSystemAdapter(pka));
            shares.Add(share);
            GSSProvider securityProvider = new GSSProvider(authenticationMechanism);
            SMBServer server = new SMBServer(shares, securityProvider);
            server.Start(System.Net.IPAddress.Any, SMBLibrary.SMBTransportType.DirectTCPTransport, false, true, true);
            //server.LogEntryAdded += Server_LogEntryAdded;
            while (!stoppingToken.IsCancellationRequested)
            {


            }
        }

        private void Server_LogEntryAdded(object sender, Utilities.LogEntry e)
        {
            using (FileStream fs = new FileStream("c:\\temp\\Logs.txt", FileMode.OpenOrCreate|FileMode.Append, FileAccess.Write))
            {
                string Message = string.Format("{0}:{1}\t{2}\t{3}\n", e.Time, e.Severity, e.Source, e.Message);
                fs.Write(System.Text.Encoding.UTF8.GetBytes(Message));
                fs.Flush();
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
