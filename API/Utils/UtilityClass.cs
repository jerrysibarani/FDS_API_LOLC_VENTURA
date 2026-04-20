using Microsoft.AspNetCore.StaticFiles;
using Newtonsoft.Json;

namespace API.Utils
{
    public class UtilityClass
    {

        public static string GetContentType(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fileName, out string? contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }

        public static bool CompareString(string param1, string paramCheckEqual)
        {
            bool bIsEqual = false;
            param1 = GetStringToUpper(param1);
            paramCheckEqual = GetStringToUpper(paramCheckEqual);

            if (param1.Equals(paramCheckEqual))
                bIsEqual = true;

            return bIsEqual;
        }
        public static String GetStringToUpper(String oParam)
        {
            return string.IsNullOrWhiteSpace(oParam) ? "" : oParam.ToUpper().Trim();
        }
        public static DateTime? GetDateFromSQL(object oParam)
        {
            try
            {
                if (oParam == null || oParam == DBNull.Value || CompareString(oParam.ToString()!, "NULL") || string.IsNullOrWhiteSpace(oParam?.ToString()))
                    return null;

                return Convert.ToDateTime(oParam);
            }
            catch (Exception ex)
            {
                throw new Exception("Cannot convert date value : " + oParam.ToString() + " Message :" + ex.Message.ToString());
            }
        }
        public static int GetIntValue(object oParam)
        {
            try
            {
                if (oParam == null || UtilityClass.CompareString(oParam.ToString()!, "-"))
                    return 0;



                return oParam == DBNull.Value ? 0 : Convert.ToInt32(oParam);
            }
            catch (Exception ex)
            {
                throw new Exception("Cannot convert GetIntFromSQL value : " + JsonConvert.SerializeObject(oParam) + " Message :" + ex.Message.ToString());
            }
        }

        internal static void LogToFile(string ErrorMessage)
        {

            using (StreamWriter w = File.AppendText(string.Format("logFile-{0}.txt", DateTime.Now.ToString("yyyy-MM-dd"))))
            {
                w.WriteLine(string.Format("error on {0}: {1} {2}~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~", DateTime.Now.ToString(), ErrorMessage, Environment.NewLine + Environment.NewLine));
            }
        }

        internal static bool ContainsString(string value1, string valueText)
        {
            value1 = GetStringToUpper(value1);
            valueText = GetStringToUpper(valueText);

            return value1.Contains(valueText);
        }
    }
}
