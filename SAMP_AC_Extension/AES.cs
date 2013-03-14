using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
 

public class Cryptology
{
private const string Salt = "password salt removed for public src release";
private const int SizeOfBuffer = 1024*8;
 
internal static byte[] EncryptStringToBytes(string plainText, byte[] key, byte[] iv)
{
// Check arguments.
if (plainText == null || plainText.Length <= 0)
{
throw new ArgumentNullException("plainText");
}
if (key == null || key.Length <= 0)
{
throw new ArgumentNullException("key");
}
if (iv == null || iv.Length <= 0)
{
throw new ArgumentNullException("key");
}
 
byte[] encrypted;
// Create an RijndaelManaged object
// with the specified key and IV.
using (var rijAlg = new RijndaelManaged())
{
rijAlg.Key = key;
rijAlg.IV = iv;
 
// Create a decrytor to perform the stream transform.
ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);
 
// Create the streams used for encryption.
using (var msEncrypt = new MemoryStream())
{
using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
{
using (var swEncrypt = new StreamWriter(csEncrypt))
{
//Write all data to the stream.
swEncrypt.Write(plainText);
}
encrypted = msEncrypt.ToArray();
}
}
}
 
 
// Return the encrypted bytes from the memory stream.
return encrypted;
 
}
 
internal static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
{
// Check arguments.
if (cipherText == null || cipherText.Length <= 0)
throw new ArgumentNullException("cipherText");
if (key == null || key.Length <= 0)
throw new ArgumentNullException("key");
if (iv == null || iv.Length <= 0)
throw new ArgumentNullException("key");
 
// Declare the string used to hold
// the decrypted text.
string plaintext;
 
// Create an RijndaelManaged object
// with the specified key and IV.
using (var rijAlg = new RijndaelManaged())
{
rijAlg.Key = key;
rijAlg.IV = iv;
 
// Create a decrytor to perform the stream transform.
ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);
 
// Create the streams used for decryption.
using (var msDecrypt = new MemoryStream(cipherText))
{
using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
{
using (var srDecrypt = new StreamReader(csDecrypt))
{
// Read the decrypted bytes from the decrypting stream
// and place them in a string.
plaintext = srDecrypt.ReadToEnd();
}
}
}
 
}
return plaintext;
}
 
internal static bool EncryptFile(string inputPath, string outputPath, string password)
{
var input = new FileStream(inputPath, FileMode.Open, FileAccess.Read);
var output = new FileStream(outputPath, FileMode.OpenOrCreate, FileAccess.Write);
 
// Essentially, if you want to use RijndaelManaged as AES you need to make sure that:
// 1.The block size is set to 128 bits
// 2.You are not using CFB mode, or if you are the feedback size is also 128 bits
 
RijndaelManaged algorithm = new RijndaelManaged {KeySize = 256, BlockSize = 128};

algorithm.Mode = CipherMode.CBC;

var key = new Rfc2898DeriveBytes(password, Encoding.ASCII.GetBytes(Salt));
 
algorithm.Key = key.GetBytes(algorithm.KeySize/8);
algorithm.IV = key.GetBytes(algorithm.BlockSize/8);
 
using (var encryptedStream = new CryptoStream(output, algorithm.CreateEncryptor(), CryptoStreamMode.Write))
{
CopyStream(input, encryptedStream);
}

return true;
}
 
internal static bool DecryptFile(string inputPath, string outputPath, string password)
{
var input = new FileStream(inputPath, FileMode.Open, FileAccess.Read);
var output = new FileStream(outputPath, FileMode.OpenOrCreate, FileAccess.Write);
 
// Essentially, if you want to use RijndaelManaged as AES you need to make sure that:
// 1.The block size is set to 128 bits
// 2.You are not using CFB mode, or if you are the feedback size is also 128 bits
RijndaelManaged algorithm = new RijndaelManaged {KeySize = 256, BlockSize = 128};

algorithm.Mode = CipherMode.CBC;

var key = new Rfc2898DeriveBytes(password, Encoding.ASCII.GetBytes(Salt));
 
algorithm.Key = key.GetBytes(algorithm.KeySize/8);
algorithm.IV = key.GetBytes(algorithm.BlockSize/8);
 
try
{
using (var decryptedStream = new CryptoStream(output, algorithm.CreateDecryptor(), CryptoStreamMode.Write))
{
CopyStream(input, decryptedStream);
return true;
}
}
catch (CryptographicException)
{
throw new InvalidDataException("Please supply a correct password");
}
catch (Exception ex)
{
throw new Exception(ex.Message);
}
}
 
private static void CopyStream(Stream input, Stream output)
{
using (output)
using (input)
{
byte[] buffer = new byte[SizeOfBuffer];
int read;
while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
{
output.Write(buffer, 0, read);
}
}
}
}