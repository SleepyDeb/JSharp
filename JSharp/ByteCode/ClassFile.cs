using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using JSharp.Helpers;
using JSharp.Package;

namespace JSharp.ByteCode
{
    public class ConstantsArray<T> : ClassItemBase, IEnumerable<T> where T : class
    {
        private ushort[] Indexes { get; }
        public T this[int i] => ClassFile.Constants[Indexes[i]] as T;

        public ConstantsArray(ClassFile classFile, ushort[] indexes) : base(classFile)
        {
            Indexes = indexes;
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var i in Indexes)
                yield return ClassFile.Constants[i] as T;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class ClassFile
    {
        public JavaPackage Parent;
        public string Name;

        public const UInt32 Magic = 0xCAFEBABE;
        public UInt16 MinorVersion;
        public UInt16 MajorVersion;
        public UInt16 ConstantPoolCount;
        public ConstantInfoBase[] Constants;
        public ClassAccessFlags ClassAccessFlag;
        public ushort ThisClassIndex;
        public ConstantClassInfo ThisClass => (ConstantClassInfo) Constants[ThisClassIndex];
        public ushort SuperClassIndex;
        public ConstantClassInfo SuperClass => Constants[ThisClassIndex] as ConstantClassInfo;

        public ushort InterfacesCount;
        public ushort[] InterfacesIndexes;
        public ConstantsArray<ConstantClassInfo> Interfaces { get; }

        public ushort FieldsInfoCount;
        public FieldInfo[] Fields;

        public ushort MethodsInfoCount;
        public MethodInfo[] Methods;

        public ushort AttributesCount;
        public AttributeInfo[] Attributes;

        public string Version => MajorVersion + "." + MinorVersion;

        // ReSharper disable once SuggestBaseTypeForParameter
        public ClassFile(JavaPackage parent, string name, BigEndianBinaryReader reader)
        {
            Parent = parent;
            Name = name;

            if(reader.ReadUInt32() != Magic)
                throw new FormatException();

            MinorVersion = reader.ReadUInt16();
            MajorVersion = reader.ReadUInt16();

            ConstantPoolCount = reader.ReadUInt16();
            Constants = new ConstantInfoBase[ConstantPoolCount];

            for (int i = 1; i < ConstantPoolCount; i++)
            {
                ConstantType tagByte = (ConstantType) reader.ReadByte();
                switch (tagByte)
                {
                    case ConstantType.Class:
                        Constants[i] = new ConstantClassInfo(this, reader);
                        break;
                    case ConstantType.Fieldref:
                        Constants[i] = new ConstantFieldrefInfo(this, reader);
                        break;
                    case ConstantType.Methodref:
                        Constants[i] = new ConstantMethodrefInfo(this, reader);
                        break;
                    case ConstantType.InterfaceMethodref:
                        Constants[i] = new ConstantInterfaceMethodrefInfo(this, reader);
                        break;
                    case ConstantType.String:
                        Constants[i] = new ConstantStringInfo(this, reader);
                        break;
                    case ConstantType.Integer:
                        Constants[i] = new ConstantIntegerInfo(this, reader);
                        break;
                    case ConstantType.Float:
                        Constants[i] = new ConstantFloatInfo(this, reader);
                        break;
                    case ConstantType.Long:
                        Constants[i++] = new ConstantLongInfo(this, reader);
                        break;
                    case ConstantType.Double:
                        Constants[i++] = new ConstantDoubleInfo(this, reader);
                        break;
                    case ConstantType.NameAndType:
                        Constants[i] = new ConstantNameAndTypeInfo(this, reader);
                        break;
                    case ConstantType.MethodHandle:
                        Constants[i] = new ConstantMethodHandleInfo(this, reader);
                        break;
                    case ConstantType.MethodType:
                        Constants[i] = new ConstantMethodType(this, reader);
                        break;
                    case ConstantType.InvokeDynamic:
                        Constants[i] = new ConstantInvokeDynamicInfo(this, reader);
                        break;
                    case ConstantType.Utf8:
                        Constants[i] = new ConstantUtf8Info(this, reader);
                        break;
                    default:
                        throw new InvalidOperationException("Unknown Constant Pool Type");
                }
            }

            ClassAccessFlag = (ClassAccessFlags) reader.ReadUInt16();

            ThisClassIndex = reader.ReadUInt16();
            SuperClassIndex = reader.ReadUInt16();

            InterfacesCount = reader.ReadUInt16();
            InterfacesIndexes = new ushort[InterfacesCount];
            for (int i = 0; i < InterfacesCount; i++)
                InterfacesIndexes[i] = reader.ReadUInt16();
            Interfaces = new ConstantsArray<ConstantClassInfo>(this, InterfacesIndexes);

            FieldsInfoCount = reader.ReadUInt16();
            Fields = new FieldInfo[FieldsInfoCount];
            for(int i = 0; i < FieldsInfoCount; i++)
                Fields[i] = new FieldInfo(this, reader);

            MethodsInfoCount = reader.ReadUInt16();
            Methods = new MethodInfo[MethodsInfoCount];
            for(int i=0;i<MethodsInfoCount; i++)
                Methods[i] = new MethodInfo(this, reader);

            AttributesCount = reader.ReadUInt16();
            Attributes = new AttributeInfo[AttributesCount];
            for(int i = 0; i < AttributesCount; i++)
                Attributes[i] = AttributeInfo.GetAttributeInfo(AttributeTarget.ClassFile, this, reader);
        }

        public ushort Find(string name)
        {
            for (ushort i = 1; i < ConstantPoolCount; i++)
            {
                var us = Constants[i] as ConstantUtf8Info;

                if(us == null) continue;

                if (name == us.Value)
                    return i;
            }

            throw new KeyNotFoundException();
        }

        public override string ToString() {
            if(Parent == null)
                return Name;

            return Parent + "." + Name + ".class";
        }
    }

    public abstract class ClassItemBase {
        protected ClassFile ClassFile { get; }
        
        protected ClassItemBase(ClassFile classFile) {
            ClassFile = classFile;
            if(ClassFile == null) {
                throw new ArgumentNullException(nameof(classFile));
            }
        }
    }

    [Flags]
    public enum ClassAccessFlags : ushort
    {
        Public = 0x0001, // Declared public; may be accessed from outside its package. 
        Final = 0x0010, // Declared final; no subclasses allowed. 
        Super = 0x0020, // Treat superclass methods specially when invoked by the invokespecial instruction. 
        Interface = 0x0200, // Is an interface, not a class.
        Abstract = 0x0400, // Declared abstract; must not be instantiated. 
        Synthetic = 0x1000, // Declared synthetic; not present in the source code. 
        Annotation = 0x2000, // Declared as an annotation type.
        Enum = 0x4000 // Declared as an enum type. 
    }
}
