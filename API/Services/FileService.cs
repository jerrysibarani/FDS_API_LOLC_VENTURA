using API.Data;
using API.Helpers;
using API.IServices;
using API.Models;
using API.Models.Params;
using Microsoft.AspNetCore.Mvc;
using API.Data.Entities;
using API.Utils;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class FileService(
        AppDbContext dbContext,
        IConfiguration configuration
    ) : IFileService
    {
        private readonly AppDbContext _dbContext = dbContext;
        private readonly IConfiguration _configuration = configuration;

        public async Task<bool> SaveCertificate(Principal currentUser, [FromBody] ParamFile param)
        {
            var clientCode = currentUser.ClientCode;

            if (currentUser.UserType != ConstantaData.INTERNAL)
            {
                var document = await _dbContext.Documents
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ID == param.ID && x.CUSTOMER_CODE == param.CustomerCode);

                clientCode = document?.CLIENT_CODE ?? clientCode;
            }

            var basePath = _configuration["MySettings:DocumentsFiles"];
            var dateFolder = DateTime.Now.ToString("yyyyMMdd");

            var subDir = Path.Combine(clientCode!, param.CustomerCode, ConstantaData.DOCTYPECERTIFICATE);
            var directoryPath = Path.Combine(basePath!, subDir, dateFolder);
            var subPath = Path.Combine(subDir, dateFolder);

            // Ensure the directory exists
            Directory.CreateDirectory(directoryPath);

            var filePath = Path.Combine(directoryPath, param.FileName);
            await File.WriteAllBytesAsync(filePath, param.Content);

            var fileExtension = Path.GetExtension(param.FileName);
            var fileNameServer = $"{Guid.NewGuid()}{fileExtension}";

            var documentFile = new DOCUMENTS_FILE
            {
                DOCUMENT_ID = param.ID,
                CUSTOMER_CODE = param.CustomerCode,
                DOCUMENT_TYPE = ConstantaData.DOCTYPECERTIFICATE,
                FILE_PATH = subPath,
                FILE_NAME_CLIENT = param.FileName,
                FILE_NAME_SERVER = fileNameServer,
                MIME_TYPE = UtilityClass.GetContentType(param.FileName),
                SIZE_BYTES = param.Content.Length,
                ISACTIVE = true,
                CREATED_DATE = DateTime.Now,
                CREATED_BY = currentUser.Email
            };

            await _dbContext.DocumentsFile.AddAsync(documentFile);
            await _dbContext.SaveChangesAsync();
            _dbContext.ChangeTracker.Clear();

            return true;

            // OLD
            //var PathDocsFiles = _configuration["MySettings:DocumentsFiles"];
            //// Generate the directory path
            //var dateFolder = DateTime.Now.ToString("yyyyMMdd");

            //var subDir = "\\" + ClientCode + "\\" + param.CustomerCode + "\\" + ConstantaData.DOCTYPECERTIFICATE;
            //var dirPath = PathDocsFiles + subDir;
            //var directoryPath = Path.Combine(dirPath, dateFolder);
            //var subPath = Path.Combine(subDir, dateFolder);

            //// Ensure the directory exists
            //if (!Directory.Exists(directoryPath))
            //{
            //    Directory.CreateDirectory(directoryPath);
            //}

            //string filePath = Path.Combine(directoryPath, param.FileName);
            //await System.IO.File.WriteAllBytesAsync(filePath, param.Content);
            //long fileSize = param.Content.Length;
            //var fileContentType = UtilityClass.GetContentType(param.FileName);
            //var fileExtension = Path.GetExtension(param.FileName);
            //var realName = Path.GetFileNameWithoutExtension(param.FileName);
            //var fileNameServer = Guid.NewGuid().ToString() + fileExtension;
        }

        public async Task<bool> SaveMinuta(Principal currentUser, [FromBody] ParamFile param)
        {
            var clientCode = currentUser.ClientCode;

            if (currentUser.UserType != ConstantaData.INTERNAL)
            {
                var document = await _dbContext.Documents
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ID == param.ID && x.CUSTOMER_CODE == param.CustomerCode);

                clientCode = document?.CLIENT_CODE ?? clientCode;
            }

            var basePath = _configuration["MySettings:DocumentsFiles"];
            var dateFolder = DateTime.Now.ToString("yyyyMMdd");

            var subDir = Path.Combine(clientCode!, param.CustomerCode, ConstantaData.DOCTYPEMINUTA);
            var directoryPath = Path.Combine(basePath!, subDir, dateFolder);
            var subPath = Path.Combine(subDir, dateFolder);

            // Ensure the directory exists
            Directory.CreateDirectory(directoryPath);

            var filePath = Path.Combine(directoryPath, param.FileName);
            await File.WriteAllBytesAsync(filePath, param.Content);

            var fileExtension = Path.GetExtension(param.FileName);
            var fileNameServer = $"{Guid.NewGuid()}{fileExtension}";

            var dtFile = new DOCUMENTS_FILE
            {
                DOCUMENT_ID = param.ID,
                CUSTOMER_CODE = param.CustomerCode,
                DOCUMENT_TYPE = ConstantaData.DOCTYPEMINUTA,
                FILE_PATH = subPath,
                FILE_NAME_CLIENT = param.FileName,
                FILE_NAME_SERVER = fileNameServer,
                MIME_TYPE = UtilityClass.GetContentType(param.FileName),
                SIZE_BYTES = param.Content.Length,
                ISACTIVE = true,
                CREATED_DATE = DateTime.Now,
                CREATED_BY = currentUser.Email
            };


            await _dbContext.DocumentsFile.AddAsync(dtFile);
            await _dbContext.SaveChangesAsync();
            _dbContext.ChangeTracker.Clear();
            return true;
        }
    }
}
