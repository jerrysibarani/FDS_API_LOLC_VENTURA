namespace API.IServices
{
    public interface IConnectionDB
    {
        public string DecryptionDB(string encryptedConnectionString);
    }
}
