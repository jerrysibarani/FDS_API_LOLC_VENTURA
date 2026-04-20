using API.Data;
using API.Helpers;
using API.IServices;
using API.Models;
using API.Models.Params;
using Microsoft.AspNetCore.Mvc;
using API.Data.Entities;
using API.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace API.Services
{
    public class FileService(
        AppDbContext DBContext,
        IOptions<MySettings> options
    ) : IFileService
    {
        private readonly AppDbContext _dbContext = DBContext;
        private readonly MySettings _settings = options.Value;
        
        public async Task<bool> SaveCertificate(Principal UserCurrent, [FromBody] ParamFile Param, CancellationToken cancellationToken = default)
        {
            var clientCode = UserCurrent.ClientCode;

            if (UserCurrent.UserType != ConstantaData.INTERNAL)
            {
                var document = await _dbContext.Documents
                    .Where(x => x.ID == Param.ID && x.CUSTOMER_CODE == Param.CustomerCode)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cancellationToken);

                clientCode = document?.CLIENT_CODE ?? clientCode;
            }

            var basePath = _settings.DocumentsFiles!;
            var dateFolder = DateTime.Now.ToString("yyyyMMdd");

            var subDir = Path.Combine(clientCode!, Param.CustomerCode, ConstantaData.DOCTYPECERTIFICATE);
            var directoryPath = Path.Combine(basePath!, subDir, dateFolder);
            var subPath = Path.Combine(subDir, dateFolder);

            // Ensure the directory exists
            Directory.CreateDirectory(directoryPath);

            var filePath = Path.Combine(directoryPath, Param.FileName);
            await File.WriteAllBytesAsync(filePath, Param.Content);

            var fileExtension = Path.GetExtension(Param.FileName);
            var fileNameServer = $"{Guid.NewGuid()}{fileExtension}";

            var documentFile = new DOCUMENTS_FILE
            {
                DOCUMENT_ID = Param.ID,
                CUSTOMER_CODE = Param.CustomerCode,
                DOCUMENT_TYPE = ConstantaData.DOCTYPECERTIFICATE,
                FILE_PATH = subPath,
                FILE_NAME_CLIENT = Param.FileName,
                FILE_NAME_SERVER = fileNameServer,
                MIME_TYPE = UtilityClass.GetContentType(Param.FileName),
                SIZE_BYTES = Param.Content.Length,
                ISACTIVE = true,
                CREATED_DATE = DateTime.Now,
                CREATED_BY = UserCurrent.Email
            };

            await _dbContext.DocumentsFile.AddAsync(documentFile);
            await _dbContext.SaveChangesAsync(cancellationToken);
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

        public async Task<bool> SaveMinuta(Principal UserCurrent, [FromBody] ParamFile Param, CancellationToken cancellationToken = default)
        {
            var clientCode = UserCurrent.ClientCode;

            if (UserCurrent.UserType != ConstantaData.INTERNAL)
            {
                var document = await _dbContext.Documents
                    .Where(x => x.ID == Param.ID && x.CUSTOMER_CODE == Param.CustomerCode)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cancellationToken);

                clientCode = document?.CLIENT_CODE ?? clientCode;
            }

            var basePath = _settings.DocumentsFiles!;
            var dateFolder = DateTime.Now.ToString("yyyyMMdd");

            var subDir = Path.Combine(clientCode!, Param.CustomerCode, ConstantaData.DOCTYPEMINUTA);
            var directoryPath = Path.Combine(basePath!, subDir, dateFolder);
            var subPath = Path.Combine(subDir, dateFolder);

            // Ensure the directory exists
            Directory.CreateDirectory(directoryPath);

            var filePath = Path.Combine(directoryPath, Param.FileName);
            await File.WriteAllBytesAsync(filePath, Param.Content);

            var fileExtension = Path.GetExtension(Param.FileName);
            var fileNameServer = $"{Guid.NewGuid()}{fileExtension}";

            var dtFile = new DOCUMENTS_FILE
            {
                DOCUMENT_ID = Param.ID,
                CUSTOMER_CODE = Param.CustomerCode,
                DOCUMENT_TYPE = ConstantaData.DOCTYPEMINUTA,
                FILE_PATH = subPath,
                FILE_NAME_CLIENT = Param.FileName,
                FILE_NAME_SERVER = fileNameServer,
                MIME_TYPE = UtilityClass.GetContentType(Param.FileName),
                SIZE_BYTES = Param.Content.Length,
                ISACTIVE = true,
                CREATED_DATE = DateTime.Now,
                CREATED_BY = UserCurrent.Email
            };


            await _dbContext.DocumentsFile.AddAsync(dtFile);
            await _dbContext.SaveChangesAsync(cancellationToken);
            _dbContext.ChangeTracker.Clear();
            return true;
        }
    }
}
