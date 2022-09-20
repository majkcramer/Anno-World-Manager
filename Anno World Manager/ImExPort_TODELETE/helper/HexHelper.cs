using FluentResults;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Anno_World_Manager.ImExPort.helper
{

    //  Helpfull Tool: https://www.scadacore.com/tools/programming-calculators/online-hex-converter/


    internal static class HexHelper
    {
        //  Length (in Bytes) of elementary types in hexadecimal notation
        private const int lenghtInt32Hex = 8;
        private const int lenghtInt16Hex = 4;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hexString"></param>
        /// <param name="convertFromBigEndianToLittleEndian"></param>
        /// <returns></returns>
        /// <remarks>
        /// From programming in rust I am actually used to return explicit result types as return value. I'm not sure yet if tuple is the right way here.
        /// </remarks>
        /// <example>
        /// For example, the size of a map in a7tinfo is noted in hexadecimal as two int32. For example: "0006000000060000" (as hex) results in 0 (int32) and 0 (int32)
        /// </example>
        internal static Result<(Int32,Int32)> GetTwoInt32FromHex(String hexString, bool convertFromBigEndianToLittleEndian)
        {
            int requiredLenght = lenghtInt32Hex * 2;
            if (hexString.Length == requiredLenght)
            {
                Result<int> part01 = GetInt32BigEndianFromHex(hexString.Substring(0, lenghtInt32Hex));
                Result<int> part02 = GetInt32BigEndianFromHex(hexString.Substring(lenghtInt32Hex, lenghtInt32Hex));

                if (part01.IsSuccess && part02.IsSuccess)
                {
                    switch (convertFromBigEndianToLittleEndian)
                    {
                        case true:
                            return Result.Ok((ConvertInt32BigEndianToLittleEndian(part01.Value), ConvertInt32BigEndianToLittleEndian(part02.Value)));
                            break;
                        case false:
                            return Result.Ok((part01.Value, part02.Value));
                            break;
                    }
                }
                else 
                {
                    return Result.Fail(String.Empty);
                }
            }
            else 
            {
                Log.Logger.Debug("The string passed as parameter should have had a length of {0} bytes. Instead, the string '{1}' had a length of {2} bytes.", requiredLenght, hexString, hexString.Length);
                return Result.Fail(String.Empty);
            }
        }

        internal static Result<(Int32,Int32,Int32,Int32)> GetFourInt32FromHex(String hexString, bool convertFromBigEndianToLittleEndian)
        {
            int requiredLenght = lenghtInt32Hex * 4;
            if (hexString.Length == requiredLenght)
            {
                Result<int> part01 = GetInt32BigEndianFromHex(hexString.Substring(0, lenghtInt32Hex));
                Result<int> part02 = GetInt32BigEndianFromHex(hexString.Substring(lenghtInt32Hex, lenghtInt32Hex));
                Result<int> part03 = GetInt32BigEndianFromHex(hexString.Substring(lenghtInt32Hex*2, lenghtInt32Hex));
                Result<int> part04 = GetInt32BigEndianFromHex(hexString.Substring(lenghtInt32Hex*3, lenghtInt32Hex));

                if (part01.IsSuccess && part02.IsSuccess && part03.IsSuccess && part04.IsSuccess)
                {
                    switch (convertFromBigEndianToLittleEndian)
                    {
                        case true:
                            return Result.Ok((ConvertInt32BigEndianToLittleEndian(part01.Value), ConvertInt32BigEndianToLittleEndian(part02.Value), ConvertInt32BigEndianToLittleEndian(part03.Value), ConvertInt32BigEndianToLittleEndian(part04.Value)));
                            break;
                        case false:
                            return Result.Ok((part01.Value, part02.Value, part03.Value, part04.Value));
                            break;
                    }
                }
                else
                {
                    return Result.Fail(String.Empty);
                }
            }
            else
            {
                Log.Logger.Debug("The string passed as parameter should have had a length of {0} bytes. Instead, the string '{1}' had a length of {2} bytes.", requiredLenght, hexString, hexString.Length);
                return Result.Fail(String.Empty);
            }
        }

        internal static Result<Int16> GetOneInt16FromHex(String hexString, bool convertFromBigEndianToLittleEndian)
        {
            int requiredLenght = lenghtInt16Hex;
            if (hexString.Length == requiredLenght)
            {
                Result<Int16> result = GetInt16BigEndianFromHex(hexString);

                if (result.IsSuccess)
                {
                    switch(convertFromBigEndianToLittleEndian)
                    {
                        case true:
                            return Result.Ok((ConvertInt16BigEndianToLittleEndian(result.Value)));
                            break;
                        case false:
                            return Result.Ok(result.Value);
                            break;
                    }
                }
                else
                {
                    return Result.Fail(String.Empty);
                }
            }
            else
            {
                Log.Logger.Debug("The string passed as parameter should have had a length of {0} bytes. Instead, the string '{1}' had a length of {2} bytes.", requiredLenght, hexString, hexString.Length);
                return Result.Fail(String.Empty);
            }
        }

        internal static Result<List<Int16>> GetListOfInt16FromHex(String hexString, bool convertFromBigEndianToLittleEndian)
        {
            //  TODO: Prio 1 - Check functionality as soon new FileDBReader Version is integrated
            throw new NotImplementedException();
        }


        internal static Result<Int32> GetInt32BigEndianFromHex(String hexString)
        {
            if (hexString.Length == lenghtInt32Hex)
            {
                //  var t = int.Parse("48 3D E3 F4".Replace(" ", string.Empty), System.Globalization.NumberStyles.HexNumber);
                Int32 v = int.Parse(hexString, System.Globalization.NumberStyles.HexNumber);
                return Result.Ok(v);
            }

            Log.Logger.Debug("The string passed as parameter should have had a length of {0} bytes. Instead, the string '{1}' had a length of {2} bytes.", lenghtInt32Hex, hexString, hexString.Length);
            return Result.Fail(String.Empty);
        }

        internal static Result<Int16> GetInt16BigEndianFromHex(String hexString)
        {
            if (hexString.Length == lenghtInt16Hex)
            {
                //  var t = int.Parse("48 3D E3 F4".Replace(" ", string.Empty), System.Globalization.NumberStyles.HexNumber);
                Int16 v = short.Parse(hexString, System.Globalization.NumberStyles.HexNumber);
                return Result.Ok(v);
            }

            Log.Logger.Debug("The string passed as parameter should have had a length of {0} bytes. Instead, the string '{1}' had a length of {2} bytes.", lenghtInt16Hex, hexString, hexString.Length);
            return Result.Fail(String.Empty);
        }

        internal static Int32 ConvertInt32BigEndianToLittleEndian(Int32 bigEndian)
        {
            //  https://docs.microsoft.com/en-us/dotnet/api/system.buffers.binary.binaryprimitives?view=net-6.0

            return BinaryPrimitives.ReverseEndianness(bigEndian);
        }

        internal static Int16 ConvertInt16BigEndianToLittleEndian(Int16 bigEndian)
        {
            //  https://docs.microsoft.com/en-us/dotnet/api/system.buffers.binary.binaryprimitives?view=net-6.0

            return BinaryPrimitives.ReverseEndianness(bigEndian);
        }
    }
}
