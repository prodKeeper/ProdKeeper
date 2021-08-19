using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProdKeeper.Data;
using ProdKeeper.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;

namespace ProdKeeper.Services.Tests
{
    [TestClass()]
    public class FileSystemServiceTests
    {
        private ApplicationDbContext _context;
        private FileSystemService fileSystemService;
        private Guid viewID;
        public FileSystemServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
    .UseSqlServer("Server=localhost;Database=prodkeeper;Trusted_Connection=True;MultipleActiveResultSets=true")
    .Options;

            OperationalStoreOptions storeOptions = new OperationalStoreOptions
            {

            };

            IOptions<OperationalStoreOptions> operationalStoreOptions = Options.Create(storeOptions);


            _context = new ApplicationDbContext(options, operationalStoreOptions);
            FileSystemOption fileoption = new FileSystemOption(_context);
            fileoption.StorePath = "c:\\Temp\\FileRepo";
            fileSystemService = new FileSystemService(fileoption);
        }

        [TestMethod()]
        public void CreateFolderTest()
        {
            viewID = fileSystemService.CreateView("Test", "/Geographie/{Continent?}/{Pays?}/{Region?}/");
            fileSystemService.CreateFolder(string.Format("{0}/Geographie/Europe/France/Lorraine/", viewID.ToString()));
            fileSystemService.CreateFolder(string.Format("{0}/Geographie/Europe/France/Ile de france/", viewID.ToString()));
            fileSystemService.CreateFolder(string.Format("{0}/Geographie/Europe/Belgique/Luxembourg/", viewID.ToString()));
            fileSystemService.CreateFolder(string.Format("{0}/Geographie/Europe/Belgique/Namur/", viewID.ToString()));
            fileSystemService.CreateFolder(string.Format("{0}/Geographie/Europe/Allemagne/Sarre/", viewID.ToString()));
            var keys = _context.MetadataKey;
            var vals = _context.MetadataValues;
            if (keys.Where(k => k.Libelle == "Continent") == null || keys.Where(k => k.Libelle == "Pays") == null || keys.Where(k => k.Libelle == "Region") == null)
                Assert.Fail();
            if (vals.Where(k => k.Libelle == "Europe") == null || vals.Where(k => k.Libelle == "France") == null || vals.Where(k => k.Libelle == "Lorraine") == null)
                Assert.Fail();

        }

        [TestMethod()]
        public void SaveFileTest()
        {
            if (viewID == Guid.Empty)
                viewID = fileSystemService.CreateView("Test", "/Geographie/{Continent?}/{Pays?}/{Region?}/");
            var fileContent = System.IO.File.ReadAllBytes("c:\\temp\\testpdf.pdf");
            fileSystemService.SaveFile(string.Format("{0}/Geographie/Europe/France/Lorraine/file.pdf", viewID.ToString()), fileContent);
            var items = _context.Item;
            if (items.Where(i => i.Libelle == "file.pdf") == null)
                Assert.Fail();
        }

        [TestMethod()]
        public void GetFoldersTest()
        {
            
            if (viewID == Guid.Empty)
                viewID = fileSystemService.CreateView("Test", "/Geographie/{Continent?}/{Pays?}/{Region?}/");
            var folderFrance = fileSystemService.GetFolders(string.Format("{0}/Geographie/Europe/France/", viewID.ToString()));
            var folderBelgique = fileSystemService.GetFolders(string.Format("{0}/Geographie/Europe/Belgique/", viewID.ToString()));
            var folderAllemagne = fileSystemService.GetFolders(string.Format("{0}/Geographie/Europe/Allemagne/", viewID.ToString()));
        }

        [TestMethod()]
        public void DeleteFileTest()
        {
            if (viewID == Guid.Empty)
                viewID = fileSystemService.CreateView("Test", "/Geographie/{Continent?}/{Pays?}/{Region?}/");
            var fileContent = System.IO.File.ReadAllBytes("c:\\temp\\testpdf.pdf");
            string filePath = string.Format("{0}/Geographie/Europe/France/Lorraine/fileToDelete.pdf", viewID.ToString());
            Trace.WriteLine(filePath);
            fileSystemService.SaveFile(filePath, fileContent);
            fileSystemService.DeleteFile(filePath);
        }

        [TestMethod()]
        public void CreateViewTest()
        {
            viewID = fileSystemService.CreateView("Test", "/Geographie/{Continent?}/{Pays?}/{Region?}/");
            var repo = _context.PatternsRepository;
        }

        [TestMethod()]
        public void DeleteFolderTest()
        {
            viewID = fileSystemService.CreateView("Test", "/Geographie/{Continent?}/{Pays?}/{Region?}/");
            var folder = string.Format("{0}/Geographie/Europe/France/Nord-Pas-De-Calais/", viewID.ToString());
            fileSystemService.CreateFolder(folder);
            fileSystemService.DeleteFolder(folder);
        }

        [TestMethod()]
        public void ProcessRecycleBinTest()
        {
            fileSystemService.ProcessRecycleBin();
        }
    }
}