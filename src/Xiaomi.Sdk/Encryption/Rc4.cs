using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Xiaomi.Sdk.Encryption;

public class Rc4
{
    private byte[] _sblock = new byte[256];
    private int _x = 0, _y = 0;

    public Rc4(string password)
        : this(Encoding.UTF8.GetBytes(password)) { }

    // Creates an RC4 instance using the given key of any length.
    public Rc4(byte[] key)
    {
        CreateSBlock();
        KeyScheduling(key);
    }


    public byte[] Cipher(string data)
    {
        return Cipher(Encoding.UTF8.GetBytes(data));
    }

    public byte[] Cipher(byte[] data)
    {
        return Cipher(data, 0, data.Length);
    }

    // Performs encryption or decryption of the data.
    public byte[] Cipher(byte[] data, int offset, int count)
    {
        var cipher = new byte[data.Length];
        for (int i = offset; i < count; i++)
        {
            cipher[i] = unchecked((byte)(data[i] ^ NextByte()));
        }
        return cipher;
    }


    private void CreateSBlock() // S-block initialization.
    {
        for (int i = 0; i < 256; i++)
        {
            _sblock[i] = (byte)i;
        }
    }

    private void KeyScheduling(byte[] key) // KSA
    {
        for (int i = 0, j = 0, l = key.Length; i < 256; i++)
        {
            j = (j + _sblock[i] + key[i % l]) % 256;
            Swap(_sblock, i, j);
        }
    }

    private void Swap(byte[] array, int index1, int index2)
    {
        byte b = array[index1];
        array[index1] = array[index2];
        array[index2] = b;
    }

    private byte NextByte() // PRGA
    {
        _x = (_x + 1) % 256;
        _y = (_y + _sblock[_x]) % 256;
        Swap(_sblock, _x, _y);
        return _sblock[(_sblock[_x] + _sblock[_y]) % 256];
    }
}
