using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace JOSAM.Utility
{
    public class BarcodeDecoder
    {
        private static readonly Dictionary<string, char> Code128B = new Dictionary<string, char>();
        
        static BarcodeDecoder()
        {
            // 初始化 Code 128B 字符集映射
            InitializeCode128B();
        }

        private static void InitializeCode128B()
        {
            // Code 128B 編碼表（部分示例）
            Code128B.Add("11011001100", ' ');  // Space
            Code128B.Add("11001101100", '!');
            Code128B.Add("11001100110", '"');
            // ... 可以根據需要添加更多字符映射
        }

        public static string DecodeCode128(string binaryPattern)
        {
            if (string.IsNullOrEmpty(binaryPattern))
                return string.Empty;

            StringBuilder result = new StringBuilder();
            int startIndex = 0;

            try
            {
                // 查找起始字符
                while (startIndex < binaryPattern.Length - 11)
                {
                    string currentPattern = binaryPattern.Substring(startIndex, 11);
                    
                    if (Code128B.TryGetValue(currentPattern, out char decodedChar))
                    {
                        result.Append(decodedChar);
                        startIndex += 11;
                    }
                    else
                    {
                        startIndex++;
                    }
                }

                return result.ToString();
            }
            catch (Exception ex)
            {
                Debug.LogError($"條碼解碼錯誤: {ex.Message}");
                return string.Empty;
            }
        }

        public static bool ValidateChecksum(string binaryPattern)
        {
            // TODO: 實現校驗和驗證邏輯
            return true;
        }
    }
} 