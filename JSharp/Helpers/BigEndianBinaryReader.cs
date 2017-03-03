using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSharp.Helpers
{

    public class BigEndianBinaryReader : BinaryReader
    {
        public BigEndianBinaryReader(Stream input) : base(input, Encoding.UTF8)
        {
            
        }

        public override int Read(byte[] buffer, int index, int count)
        {
            return base.Read(buffer, index, count);
        }

        public override decimal ReadDecimal()
        {
            throw new NotImplementedException();
        }

        public override double ReadDouble()
        {
            var arr = base.ReadBytes(8);
            Array.Reverse(arr);
            return BitConverter.ToDouble(arr, 0);
        }

        public override short ReadInt16()
        {
            var arr = base.ReadBytes(2);
            Array.Reverse(arr);
            return BitConverter.ToInt16(arr, 0);
        }

        public override int ReadInt32()
        {
            var arr = base.ReadBytes(4);
            Array.Reverse(arr);
            return BitConverter.ToInt32(arr, 0);
        }

        public override long ReadInt64()
        {
            var arr = base.ReadBytes(8);
            Array.Reverse(arr);
            return BitConverter.ToInt64(arr, 0);
        }

        public override float ReadSingle()
        {
            return base.ReadSingle();
        }

        public override string ReadString()
        {
            var lenght = ReadUInt16();
            return UTF8ToString(ReadBytes(lenght));
        }

        public override ushort ReadUInt16()
        {
            var arr = base.ReadBytes(2);
            Array.Reverse(arr);
            return BitConverter.ToUInt16(arr, 0);
        }

        public override uint ReadUInt32()
        {
            var arr = base.ReadBytes(4);
            Array.Reverse(arr);
            return BitConverter.ToUInt32(arr, 0);
        }

        public override ulong ReadUInt64()
        {
            return base.ReadUInt64();
        }

        public static string UTF8ToString(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }
    }

}
