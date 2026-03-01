using API.IServices;
using API.Utils;

namespace API.Services
{
    public class ConnectionDB : IConnectionDB
    {
        public string DecryptionDB(string encryptedConnectionString)
        {
            try
            {
                string decryptString = EncrDecrRsa.DecryptionRSA(encryptedConnectionString);
                return decryptString; // Replace with actual decryption logic
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
