#region --- License & Copyright Notice ---
/*
Copyright (c) 2005-2012 Jeevan James
All rights reserved.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
#endregion

using System;
using System.Text;

namespace Id3.Internal
{
    //Replacement for the System.Text.ASCIIEncoding class, since it is not available in portable code.
    internal static class AsciiEncoding
    {
        internal static string GetString(byte[] bytes, int index, int count)
        {
            //return Encoding.ASCII.GetString(bytes, index, count);
            if (bytes == null)
                throw new ArgumentNullException("bytes");
            if (bytes.Length == 0)
                return string.Empty;

            if (index > bytes.Length - 1)
                throw new ArgumentOutOfRangeException("index", "index value is greater than the size of the byte array");

            if (index < 0)
                index = 0;
            if (index + count > bytes.Length)
                count = bytes.Length - index;

            var sb = new StringBuilder(count);
            for (int i = index; i < index + count; i++)
                sb.Append(bytes[i] <= 0x7f ? (char)bytes[i] : '?');
            return sb.ToString();
        }

        internal static byte[] GetBytes(string str)
        {
            //return Encoding.ASCII.GetBytes(str);
            if (str == null)
                return new byte[0];
            var retval = new byte[str.Length];
            for (int i = 0; i < str.Length; ++i)
            {
                char ch = str[i];
                if (ch <= 0x7f)
                    retval[i] = (byte)ch;
                else
                    retval[i] = (byte)'?';
            }
            return retval;
        }
    }
}