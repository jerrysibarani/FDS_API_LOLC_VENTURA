using API.Data.Entities;
using API.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {

        public DbSet<BRANCH> Branches { get; set; }
        public DbSet<CUSTOMER> Customers { get; set; }
        public DbSet<CLIENT> Clients { get; set; }
        public DbSet<CLIENT_ACCOUNT> ClientAccounts { get; set; }
        public DbSet<CLIENT_FEE> ClientFees { get; set; }


        public DbSet<ACCESS> Access { get; set; }
        public DbSet<ACCESSROLES> AccessRoles { get; set; }
        public DbSet<AHUACCOUNT> AhuAccounts { get; set; }
        public DbSet<NOTARIS> Notaris { get; set; }
        public DbSet<POSTS> Posts { get; set; }
        public DbSet<INVOICE> Invoices { get; set; }
        public DbSet<INVOICE_HISTORY> InvoiceHistories { get; set; }
        public DbSet<PNBP> Pnbps { get; set; }
        public DbSet<DOCUMENTS> Documents { get; set; }
        public DbSet<DOCUMENTS_CONFIG> DocumentsConfig { get; set; }
        public DbSet<DOCUMENTS_FILE> DocumentsFile { get; set; }
        public DbSet<DOCUMENTS_IMPORT> DocumentsImport { get; set; }
        public DbSet<HISTORY_CERTIFICATE> HistoriesCertificates { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // Ensure base method is called
            builder.Entity<ResponseTotal>().HasNoKey();
            builder.Entity<PriceDataDocument>().HasNoKey();
            builder.Entity<ColumnNameModel>().HasNoKey();


        }
    }
}
