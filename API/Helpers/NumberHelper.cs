using System;
using System.Globalization;

namespace API.Helpers
{

    public class NumberHelper
    {
        public static string NumberToWords(int number)
        {
            return NumberToWords(Convert.ToDouble(number));
        }
        public static string NumberToWords(decimal number)
        {
            return NumberToWords(Convert.ToDouble(number));
        }
        public static string NumberToWords(double number)
        {
            if (number == 0) return "nol";

            if (number < 0) return "minus " + NumberToWords(-number);

            long integerPart = (long)number;
            int decimalPart = (int)((number - integerPart) * 100);

            string words = ConvertIntegerToWords(integerPart);

            if (decimalPart > 0)
                words += " koma " + ConvertIntegerToWords(decimalPart);

            return words;
        }

        private static string ConvertIntegerToWords(long number)
        {
            string[] units = { "", "satu", "dua", "tiga", "empat", "lima", "enam", "tujuh", "delapan", "sembilan", "sepuluh", "sebelas" };

            if (number < 12)
                return units[number];
            else if (number < 20)
                return units[number - 10] + " belas";
            else if (number < 100)
                return units[number / 10] + " puluh" + (number % 10 > 0 ? " " + ConvertIntegerToWords(number % 10) : "");
            else if (number < 200)
                return "seratus" + (number % 100 > 0 ? " " + ConvertIntegerToWords(number % 100) : "");
            else if (number < 1000)
                return units[number / 100] + " ratus" + (number % 100 > 0 ? " " + ConvertIntegerToWords(number % 100) : "");
            else if (number < 2000)
                return "seribu" + (number % 1000 > 0 ? " " + ConvertIntegerToWords(number % 1000) : "");
            else if (number < 1000000)
                return ConvertIntegerToWords(number / 1000) + " ribu" + (number % 1000 > 0 ? " " + ConvertIntegerToWords(number % 1000) : "");
            else if (number < 1000000000)
                return ConvertIntegerToWords(number / 1000000) + " juta" + (number % 1000000 > 0 ? " " + ConvertIntegerToWords(number % 1000000) : "");
            else if (number < 1000000000000)
                return ConvertIntegerToWords(number / 1000000000) + " miliar" + (number % 1000000000 > 0 ? " " + ConvertIntegerToWords(number % 1000000000) : "");
            else if (number < 1000000000000000)
                return ConvertIntegerToWords(number / 1000000000000) + " triliun" + (number % 1000000000000 > 0 ? " " + ConvertIntegerToWords(number % 1000000000000) : "");
            else
                return ConvertIntegerToWords(number / 1000000000000000) + " kuadriliun" + (number % 1000000000000000 > 0 ? " " + ConvertIntegerToWords(number % 1000000000000000) : "");
        }

        public static string FormatRupiah(decimal number)
        {
            return "Rp " + number.ToString("N2", new CultureInfo("id-ID"));
        }
        public static string FormatRupiah(double number)
        {
            return FormatRupiah(Convert.ToDecimal(number));
        }
        public static string FormatRupiah(int number)
        {
            return FormatRupiah(Convert.ToDecimal(number));
        }



    }
}
