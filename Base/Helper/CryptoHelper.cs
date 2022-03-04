using System.Security.Cryptography;
using System.Text;

namespace Base.Helper;

public static class CryptoHelper
{
    private const string DefaultKey = "!@#ASD12";

    /// <summary>
    /// 
    /// </summary>
    public static string Key => DefaultKey;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="urlParam"></param>
    /// <returns></returns>
    public static string UrlParamUrlEncodeRun(string urlParam)
    {
        urlParam = urlParam.Replace("+", "＋");
        urlParam = urlParam.Replace("=", "＝");
        urlParam = urlParam.Replace("&", "＆");
        urlParam = urlParam.Replace("?", "？");
        return urlParam;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="urlParam"></param>
    /// <returns></returns>
    public static string UrlParamUrlDecodeRun(string urlParam)
    {
        urlParam = urlParam.Replace("＋", "+");
        urlParam = urlParam.Replace("＝", "=");
        urlParam = urlParam.Replace("＆", "&");
        urlParam = urlParam.Replace("？", "?");
        return urlParam;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static string MD5_Encrypt(string source, string encoding)
    {
        return MD5_Encrypt(source, string.Empty, Encoding.GetEncoding(encoding));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string MD5_Encrypt(string source)
    {
        return MD5_Encrypt(source, string.Empty, Encoding.Default);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static string MD5_Encrypt(string source, Encoding encoding)
    {
        return MD5_Encrypt(source, string.Empty, encoding);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="addKey"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static string MD5_Encrypt(string source, string addKey, Encoding encoding)
    {
        if (addKey.Length > 0)
        {
            source += addKey;
        }

        byte[] bytes = encoding.GetBytes(source);
        return MD5_Encrypt(bytes);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static string MD5_Encrypt(byte[] bytes)
    {
        MD5 mD = MD5.Create();
        byte[] array = mD.ComputeHash(bytes);
        string text = "";
        for (int i = 0; i < array.Length; i++)
        {
            string text2 = array[i].ToString("x");
            if (text2.Length == 1)
            {
                text2 = "0" + text2;
            }

            text += text2;
        }

        return text;
    }

    /// <summary>
    /// 获取文件的MD5 Hash值
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static string? FileToMd5Hash(string fileName)
    {
        string? hashMd5 = null;
        if (File.Exists(fileName))
        {
            FileInfo fi = new(fileName);
            string? str;
            using (var sr = fi.OpenText())
            {
                str = sr.ReadToEnd();
            }

            hashMd5 = ToMd5Hash(str);
        }

        return hashMd5;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string ToMd5Hash(string str)
    {
        return ToMd5Hash(Encoding.UTF8.GetBytes(str));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static string ToMd5Hash(byte[] bytes)
    {
        //计算文件的MD5值
        MD5 calculator = MD5.Create();
        Byte[] buffer = calculator.ComputeHash(bytes);
        calculator.Clear();
        //将字节数组转换成十六进制的字符串形式
        StringBuilder stringBuilder = new();
        for (int i = 0; i < buffer.Length; i++)
        {
            stringBuilder.Append(buffer[i].ToString("x2"));
        }

        var hashMd5 = stringBuilder.ToString();
        return hashMd5;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="strpwd"></param>
    /// <returns></returns>
    public static string? RegUser_MD5_Pwd(string strpwd)
    {
        string s = "fdjf,jkgfkl";
        MD5 mD = MD5.Create();
        byte[] bytes = Encoding.Default.GetBytes(s);
        byte[] bytes2 = Encoding.Default.GetBytes(strpwd);
        byte[] array = new byte[bytes.Length + 4 + bytes2.Length];
        int i;
        for (i = 0; i < bytes2.Length; i++)
        {
            array[i] = bytes2[i];
        }

        array[i++] = 163;
        array[i++] = 172;
        array[i++] = 161;
        array[i++] = 163;
        for (int j = 0; j < bytes.Length; j++)
        {
            array[i] = bytes[j];
            i++;
        }

        byte[] array2 = mD.ComputeHash(array);
        string? text = null;
        for (i = 0; i < array2.Length; i++)
        {
            string text2 = array2[i].ToString("x");
            if (text2.Length == 1)
            {
                text2 = "0" + text2;
            }

            text += text2;
        }

        return text;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string? DES_Encrypt(string source, string key = DefaultKey)
    {
        if (string.IsNullOrEmpty(source))
        {
            return null;
        }

        var des = DES.Create();
        byte[] bytes = Encoding.Default.GetBytes(source);
        des.Key = Encoding.Default.GetBytes(key);
        des.IV = Encoding.Default.GetBytes(key);
        MemoryStream memoryStream = new();
        CryptoStream cryptoStream = new(memoryStream, des.CreateEncryptor(),
            CryptoStreamMode.Write);
        cryptoStream.Write(bytes, 0, bytes.Length);
        cryptoStream.FlushFinalBlock();
        StringBuilder stringBuilder = new();
        byte[] array = memoryStream.ToArray();
        for (int i = 0; i < array.Length; i++)
        {
            byte b = array[i];
            stringBuilder.AppendFormat("{0:X2}", b);
        }

        return stringBuilder.ToString();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="key">8 length</param>
    /// <param name="iv"></param>
    /// <returns></returns>
    public static string? DES_Decrypt(string source, string key = DefaultKey, string iv = "")
    {
        if (string.IsNullOrEmpty(key))
        {
            key = DefaultKey;
        }
#if ! NET35
        if (string.IsNullOrWhiteSpace(iv))
        {
            iv = key;
        }
#endif
        if (string.IsNullOrEmpty(source))
        {
            return null;
        }

        var des = DES.Create();
        byte[] array = new byte[source.Length / 2];
        for (int i = 0; i < source.Length / 2; i++)
        {
            int num = Convert.ToInt32(source.Substring(i * 2, 2), 16);
            array[i] = (byte) num;
        }

        des.Key = Encoding.UTF8.GetBytes(key);
        des.IV = Encoding.UTF8.GetBytes(iv);
        MemoryStream memoryStream = new();
        CryptoStream cryptoStream = new(memoryStream, des.CreateDecryptor(),
            CryptoStreamMode.Write);
        cryptoStream.Write(array, 0, array.Length);
        cryptoStream.FlushFinalBlock();
        return Encoding.Default.GetString(memoryStream.ToArray());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string SHA1_Encrypt(string source)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(source);
        var sha1 = SHA1.Create();
        byte[] value = sha1.ComputeHash(bytes);
        sha1.Clear();
        string text = BitConverter.ToString(value);
        text = text.Replace("-", "");
        return text.ToLower();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string HttpBase64Encode(string? source)
    {
        if (string.IsNullOrEmpty(source))
        {
            return "";
        }

        string text = Convert.ToBase64String(Encoding.UTF8.GetBytes(source));
        text = text.Replace("+", "~");
        text = text.Replace("/", "@");
        return text.Replace("=", "$");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string HttpBase64Decode(string? source)
    {
        if (string.IsNullOrEmpty(source))
        {
            return "";
        }

        string text = source.Replace("~", "+");
        text = text.Replace("@", "/");
        text = text.Replace("$", "=");
        return Encoding.UTF8.GetString(Convert.FromBase64String(text));
    }
}