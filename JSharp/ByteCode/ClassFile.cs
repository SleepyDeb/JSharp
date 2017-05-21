using System;
using System.Collections.Generic;
using JSharp.Helpers;
using JSharp.Package;

namespace JSharp.ByteCode {
    /// <summary>
    /// This class is a parser for JAVA 8 .class compliant to: 
    ///     https://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html
    ///     
    /// This class doesn't ensure package consistency.
    /// </summary>
    public class ClassFile : JavaPackageElement
    {
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

        /// <summary>
        /// Parse .class files using BigEndianBinaryReader.
        /// </summary>
        /// <param name="name">Class name</param>
        /// <param name="parent">The package that contains this class</param>
        /// <param name="reader">The reader to get binary data</param>
        public ClassFile(string name, JavaPackage parent, BigEndianBinaryReader reader) : base(parent, name, JavaPackageElementTypes.Class)
        {
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
    }

    /// <summary>
    /// All item that lives in a class file inehrit this class
    /// </summary>
    public abstract class ClassItemBase {

        /// <summary>
        /// Parent class of the Item
        /// </summary>
        protected ClassFile ClassFile { get; }
        
        protected ClassItemBase(ClassFile classFile) {
            ClassFile = classFile;

            if(ClassFile == null)
                throw new ArgumentNullException(nameof(classFile));
        }
    }
}
