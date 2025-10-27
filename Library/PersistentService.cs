using System.Text;

namespace DecodeMessagePack.Library;

public class PersistentService
{
    public static byte[] DecryptOptimizeBytes(byte[] inputbytes, String key)
    {
        if(inputbytes == null || String.IsNullOrEmpty(key))
            return inputbytes;
        return EncryptOptimizeBytes(inputbytes, key);
    }
    
    public static byte[] EncryptOptimizeBytes(byte[] inputbytes, String key)
    {
        if(inputbytes == null || String.IsNullOrEmpty(key))
            return null;
        //key和之前一样
        return EncryptOptimize(inputbytes, Encoding.UTF8.GetBytes(key));
    }
    
    public static byte[] EncryptOptimize(byte[] plaintext, byte[] key)
    {
        byte[] S = new byte[256]; // S box
        byte[] keySchedul = new byte[plaintext.Length];

        ksa(S, key);
        rpga(S, keySchedul, plaintext.Length);

        byte[] result = new byte[plaintext.Length];

        for (int i = 0; i < plaintext.Length; ++i)
        {
            result[i] = (byte)(plaintext[i] ^ keySchedul[i]);
        }
        return result;
    }
    private static void rpga(byte[] s, byte[] keySchedul, int plaintextLength)
    {
        int i = 0, j = 0;
        for (int k = 0; k < plaintextLength; ++k)
        {
            i = (i + 1) % 256;
            j = (j + s[i]) % 256;
            Swap(s, i, j);
            keySchedul[k] = (byte)(s[(s[i] + s[j]) % 256]);
        }
    }
    
    private static void ksa(byte[] s, byte[] key)
    {
        for (int i = 0; i < 256; ++i)
        {
            s[i] = (byte)i;
        }

        int j = 0;
        for (int i = 0; i < 256; ++i)
        {
            j = (j + s[i] + key.ElementAt(i % key.Length)) % 256;
            Swap(s, i, j);
        }
    }
    
    private static void Swap(byte[] s, int i, int j)
    {
        (s[i], s[j]) = (s[j], s[i]);
    }
}