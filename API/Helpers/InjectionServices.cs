using API.IServices;
using API.Services;

namespace API.Helpers
{
    public static class InjectionServices
    {
        public static void ConfigureRepositories(IServiceCollection services)
        {
            services.AddScoped<IUserAccessService, UserAccessService>();
            services.AddScoped<IRoleUserService, RoleUserService>();

            services.AddScoped<ICoreService, CoreService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleUserService, RoleUserService>();
            services.AddScoped<IUserAccessService, UserAccessService>();
            services.AddScoped<IPostsService, PostsService>();
            services.AddScoped<IInvoiceService, InvoiceService>();
            services.AddScoped<IDocumentService, DocumentService>();
            services.AddScoped<IAHUService, AHUService>();
            services.AddScoped<IDataService, DataService>();
            services.AddScoped<INotarisService, NotarisService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IHistoryCertificateService, HistoryCertificateService>();
            
        }
    }
}
