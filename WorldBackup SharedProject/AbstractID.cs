using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;


namespace com.Lavaver.WorldBackup.Core
{
    internal class AbstractID
    {
        private static readonly Random random = new Random();


        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("由于考虑到有悖法律法规的要求，此处代码将弃用。有关此代码及其关联的 DiagnosticId，请参阅随开源代码附带的 README 或软件内发行注记。", DiagnosticId = "LAW00EF/8")]
        public static string GenerateRandomID()
        {
            string scanCode = GenerateScanCode();
            string dateCode = GenerateDateCode();
            string hashCode = GenerateHashCode(scanCode + dateCode);
            string checkDigit = CalculateCheckDigit(scanCode + dateCode + hashCode);

            return scanCode + dateCode + hashCode + checkDigit;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("由于考虑到有悖法律法规的要求，此处代码将弃用。有关此代码及其关联的 DiagnosticId，请参阅随开源代码附带的 README 或软件内发行注记。", DiagnosticId = "LAW00EF/8")]
        private static string GenerateScanCode()
        {
            string systemVersion = GenerateRandomNumberString(6);
            string kernelVersion = GenerateRandomNumberString(6);
            string randomArray = GenerateRandomNumberString(6);

            return systemVersion + kernelVersion + randomArray;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("由于考虑到有悖法律法规的要求，此处代码将弃用。有关此代码及其关联的 DiagnosticId，请参阅随开源代码附带的 README 或软件内发行注记。", DiagnosticId = "LAW00EF/8")]
        private static string GenerateDateCode()
        {
            DateTime now = DateTime.Now;
            string year = now.ToString("yyyy");
            string month = now.ToString("MM");
            string day = now.ToString("dd");

            return year + month + day;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("由于考虑到有悖法律法规的要求，此处代码将弃用。有关此代码及其关联的 DiagnosticId，请参阅随开源代码附带的 README 或软件内发行注记。", DiagnosticId = "LAW00EF/8")]
        private static string GenerateHashCode(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                int hashValue = BitConverter.ToInt32(bytes, 0);
                return hashValue.ToString("D3");
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("由于考虑到有悖法律法规的要求，此处代码将弃用。有关此代码及其关联的 DiagnosticId，请参阅随开源代码附带的 README 或软件内发行注记。", DiagnosticId = "LAW00EF/8")]
        private static string CalculateCheckDigit(string input)
        {
            int sum = 0;
            for (int i = 0; i < input.Length; i++)
            {
                sum += int.Parse(input[i].ToString());
            }
            int remainder = sum % 7;
            if (remainder == 10)
            {
                return "X";
            }
            else
            {
                return remainder.ToString();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("由于考虑到有悖法律法规的要求，此处代码将弃用。有关此代码及其关联的 DiagnosticId，请参阅随开源代码附带的 README 或软件内发行注记。", DiagnosticId = "LAW00EF/8")]
        private static string GenerateRandomNumberString(int length)
        {
            StringBuilder result = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                result.Append(random.Next(10));
            }
            return result.ToString();
        }
    }
}
