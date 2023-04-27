using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KorYmeLibrary
{
    public static class EncryptionUtilities
    {
        static string _encryptionString;
        public static string EncryptionString
        {
            get
            {
                return _encryptionString;
            }
            set
            {
                _encryptionString = value;
            }
        }

        //A mettre dans la class Utility ou dans une nouvelle classe
        public enum EncryptionType
        {
            None,
            XOR,
        }

        public static string Encrypt(string data, EncryptionType encryptionType, bool isEncrypting)
        {
            switch (encryptionType)
            {
                case EncryptionType.None:
                    return data;
                case EncryptionType.XOR:
                    return XOREncrypting(data);
                default:
                    return "";
            }
        }

        public static string XOREncrypting(string data)
        {
            string modifiedData = "";
            for (int i = 0; i < data.Length; i++)
            {
                modifiedData += (char)(data[i] ^ EncryptionString[i % EncryptionString.Length]);
            }
            return modifiedData;
        }
    }
}