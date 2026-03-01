using System.Security.Cryptography;
using System.Text;

namespace API.Utils
{
    public class EncrDecrRsa
    {
        static RSACryptoServiceProvider? rsa;

        private static readonly string Public_Key = "<RSAKeyValue><Modulus>viv+VfLhB2d8Eo7Le9WzZ9UGns4rQcSmlogfLAlWae3FweJXFfJD0YydgkxprwGfLGF5d2VrqkUXl5ex7HZOcrczD/vl0a3fVw3AtcEwGsn1m99xweA48vPZ3WzIFnY7yv8ZILAfiDinVuvnd5qXRMJYQ2NC2kttVcIHXc+oXzp0GJrD6ThVr+SmM/P5uTexGr3woI+zkDD7TeH0YPI+oAU7yNP5M9VMmpQMTk441iPHQCj22RqTI9sNR6FedshTiinbxTiv8Ifcb65/KIq/rBaJ8Npau1tgaHTZq4poIfWkW+dPbWSO2DY87lYaytOsuBoCjbz+12USHuYWNOHnBzZRZ0vW4fCo5xOgbZPAKq3GltPkdL4+dbqcaieF8NE9PEPQ+lRtZ02BUi1/8GE8vnJfljcZJ+a6JwkI/vWfh3tqy4CFle5jRa8276tDSV6nL2v9RCzhulDN76wdFDsjNF7m6KWw8DQGfLcI1Uk/WDqT3tSysvrT8kJKhgK9RfniS99WsUoLWhs6/5iQqiwVQRmrO3pqqdI2qrPag04ovsbXwFJaf4r8Yf+d0V/r/ys+tjxk+M1D8ognClFAksysMa9L1ZAiQSolxgqvU3D4RTIAe3rUjKEPEsdCLom4jCUBJIdFVy6sQm3XYfoFr1OmJvLby6De23ld7PkD+Jtq30k=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        private static readonly string Private_Key = "<RSAKeyValue><Modulus>viv+VfLhB2d8Eo7Le9WzZ9UGns4rQcSmlogfLAlWae3FweJXFfJD0YydgkxprwGfLGF5d2VrqkUXl5ex7HZOcrczD/vl0a3fVw3AtcEwGsn1m99xweA48vPZ3WzIFnY7yv8ZILAfiDinVuvnd5qXRMJYQ2NC2kttVcIHXc+oXzp0GJrD6ThVr+SmM/P5uTexGr3woI+zkDD7TeH0YPI+oAU7yNP5M9VMmpQMTk441iPHQCj22RqTI9sNR6FedshTiinbxTiv8Ifcb65/KIq/rBaJ8Npau1tgaHTZq4poIfWkW+dPbWSO2DY87lYaytOsuBoCjbz+12USHuYWNOHnBzZRZ0vW4fCo5xOgbZPAKq3GltPkdL4+dbqcaieF8NE9PEPQ+lRtZ02BUi1/8GE8vnJfljcZJ+a6JwkI/vWfh3tqy4CFle5jRa8276tDSV6nL2v9RCzhulDN76wdFDsjNF7m6KWw8DQGfLcI1Uk/WDqT3tSysvrT8kJKhgK9RfniS99WsUoLWhs6/5iQqiwVQRmrO3pqqdI2qrPag04ovsbXwFJaf4r8Yf+d0V/r/ys+tjxk+M1D8ognClFAksysMa9L1ZAiQSolxgqvU3D4RTIAe3rUjKEPEsdCLom4jCUBJIdFVy6sQm3XYfoFr1OmJvLby6De23ld7PkD+Jtq30k=</Modulus><Exponent>AQAB</Exponent><P>2xuOnWIroiAYF3PTjdRNUAnFdhdErXXJWS4bOjXaQkEiJkx0DVfg2PhlJ2kQNo1MHcuzM2fHJKs8gbeodtW/feIpwrJanSEsqghUwGA7LSoxlH9ihjwdu5Itgct3RV0FyAaKxT0Ci5TXAa2rGjylKkqYhKyzwH9Np//8XlvS15qGZtFU+i9b2M0KuJh1ZzxZBXYbmsV5JXCFqq76c018tQ0tYvEsTU0HYYX7DnYs/MAmnxT0tgBViEVVfwWyBj3zr9HjywvQHaQKHHN027lwQfIinQKkH3zZ4yiVce3VuOKzJ8Yqz5kK7B3Fb+W7EHsdkBnYVhPC4xRWNlPlZI8O3w==</P><Q>3jEvMAh/PbBjVViUaWVi7uW7V74O+M43abkPUFLQ4qPUNm0Ht9Ln9yRowBf/FOmLehjP4z6NvB+0FuNfN0JsWBONZl/oqAq6YIbX8JyQbE5atVKsxne/nvj8A6l+kiKdZSrj3pobi5kt5V/gB4Ea5lWlbu+rvM5kVPej0bAtSLLlkKA1wB/7vFqyBnr028w0d0sJ8yaVf40gjDLmT5jlZjxfTNWzVVq65kfMW2BGbPxYAWwuJykvYx9+ocZyJtWvQ8FiDw1NqtgRK7JJ2njUGT1BL5sALHSZKSUm4N8zP13S5d0P/cGuNs9AQRROgtTRUlHW6qdcz1UbfvnO06De1w==</Q><DP>mzl5npv254OvLaY8AOWf5A0YHCI94FlqzMgyN9oBByEXrWRKb02LmIrZNsNi3hLaEt3F1aw21A6P7iILsiyZ3ffEuA8czOa8urTuPd+u7L9QorBgP1MFiWX04Vgg5j4Ro3SnCTbAv9wxOhor9olyRXTVemDTc5N8k4Fx6/NHesEZaGvytM+qVPxmFQLFYd7J/ml/AvXVbqW5vIL8IYWKzMVKGydayGTCzt9ZThSAqIsEkX0KJIo2DjV4bWW+ileHdU7P/4Ad73HQS/mrlyJXmhCNXwiI91m/DwQoY68n0zSf0ZS9rZbxuvcYcb6h+PoPpo/4yHp1feRhc1maUNRcpw==</DP><DQ>Jc46mDKp68YqcKgcwH4mwj1GjhxhkcYMualqSKF+t0Fm50fP62AwZeXFCQJMPbOrLsAwBxtqpFrEmqxfVBiyNJ5HDec9v/HrWmc3MNKC5FrEpluF2FMhwJvezYS2a4kGPON6WDLigvMyUvfoN36pJA3okyKmlv6LJcXpEtGmMtt1ohEQdDs86wSHjAC5zvhE6RNUgxHKmCRcKF60v1Bln+qf0IP/+oLwkCQhSl7sPLkHBF7yz2j7jiMauc35OFBdfOrXk3YRBAO1kDjKxiiO9ihtAfueoPHJ6a1Pmy1/G8YMD154zRV6XgsPhMM+xJYPc7FHHzAQ45o4jndU0L9riw==</DQ><InverseQ>1I3RqzJaRaEH6PGzg7ANptM3vBblZI1fquofiooMnldc/NNejn6AvNkQFhDUSI3WLgsDPsx78k0yAtAE2VizlOWAHsdlivwiLdb5qFQSUhqUqohzQTedPdje9WmkJqxSjEV8APrc1f4/90ymMAHnZeETpIuI6UNPTp4X7FxfRCTlZZQV3cInyCE75LKVPmpJzO4N1s9hB89UKgabxV5jGAcuJ+GUF+tjEeYGJCqSwl8RsIGncjzXNFgbWzd6cbhwBweM8XrI5Js/Og0B1ZR7JDxkbbVuJV+cIEJ1ECz75pZ7QzvQGGk80YbyVjy8Lnh9/rOoK9qQdZjTE5jh6Z7VUg==</InverseQ><D>JFUxZWhe19RMnxsLA2DDwv2vIx4Oo6UXksD0+6vZB6n202ET9Exx6pIr8Z35vwbXS1pgI5CL7l7+7+Z6hsvflwkrR02ADfxjJDWrcM41C5k0RJB/ImRdXQV7xVMoMyppqBHu0hD0JUl1pySw92fgqLFcvZs0RZKNlAna8HBhIbiAP8cp0OADdvR/UIO3XUynmI1bcMGjs9O335xg3TjLADWfw3HENcP1u763Tyqgr0Dsq3RoHPh3zizmr53D93u0gjwXEeoPqyu50NiM0MkZb55qwkpldVAy7iINCDoMRRiySyC5SxQgOmy6XWqz+bZRVGcbv8qxAaP8v0yER5w1UBYfkuZldGvSfVbFQUEzbCbLDxkfQNpc7oX46etrdClGQCyzj2l9d9xLHP5mgMhxI/bv9qqHZYvHtSXrzIes87R71uQSU2OjAYEB0D2SxCqtC1vW12ZRPNsLzOHL0uqEcJQHsQSzgfIi5VkmEHfVyaWSN1Vpt4/O226jNOd5GPhF+jVK0YA6klzdRzNYO6Kh25sKGiAx3CHat+HMevmSNzRLmc/g1ndD27P5mj2axjW1VDyDXfdehTyi2kmNpSj2s6Znz4Ue39Sj3s0GDhVVCQoBrr2JHgBNBZxpYXzeqBQfxOdGMpfhRRK8Dd+iywshLdruIG25xgFv6jD4nL48n7U=</D></RSAKeyValue>";
        public static string DecryptionRSA(string strText)
        {
            using (rsa = new RSACryptoServiceProvider(4096))
            {

                try
                {
                    rsa.FromXmlString(Private_Key);
                    var resultBytes = Convert.FromBase64String(strText);
                    var decryptstring = rsa.Decrypt(resultBytes, true);
                    string @string = Encoding.UTF8.GetString(decryptstring);
                    return @string.ToString();
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }

        public static string EncryptionRSA(string strText)
        {

            using (rsa = new RSACryptoServiceProvider(4096))
            {
                try
                {
                    rsa.FromXmlString(Public_Key.ToString());
                    var strData = Encoding.UTF8.GetBytes(strText);
                    var encryptedData = rsa.Encrypt(strData, true);
                    var base64Encrypted = Convert.ToBase64String(encryptedData);
                    return base64Encrypted.ToString();
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }

        }
    }
}
