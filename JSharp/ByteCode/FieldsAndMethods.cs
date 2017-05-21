using System;
using JSharp.Helpers;

namespace JSharp.ByteCode {

    public class AttributeInfo : ClassItemBase {
        public AttributeTarget Target { get; }

        public ushort NameIndex;

        public ConstantUtf8Info Name => (ConstantUtf8Info)ClassFile.Constants[NameIndex];

        public uint AttributeSize;
        public byte[] AttributePayload;
        
        protected AttributeInfo(AttributeTarget target, ClassFile classFile, ushort nameIndex, uint attributeSize, byte[] attributePayload = null) : base(classFile)
        {
            Target = target;
            NameIndex = nameIndex;
            AttributeSize = attributeSize;
            AttributePayload = attributePayload;
        }

        public static AttributeInfo GetAttributeInfo(AttributeTarget target, ClassFile classFile, BigEndianBinaryReader reader)
        {
            ushort indexName = reader.ReadUInt16();
            string name = ((ConstantUtf8Info)classFile.Constants[indexName]).Value;

            switch (name)
            {
                case "ConstantValue":
                    break;
                case "Code":
                    return new CodeAttributeInfo(target, classFile, indexName, reader);
                case "StackMapTable":
                    break;
                case "Exceptions":
                    break;
                case "InnerClasses":
                    break;
                case "EnclosingMethod":
                    break;
                case "Synthetic":
                    break;
                case "Signature":
                    break;
                case "SourceFile":
                    break;
                case "SourceDebugExtension":
                    break;
                case "LineNumberTable":
                    break;
                case "LocalVariableTable":
                    break;
                case "LocalVariableTypeTable":
                    break;
                case "Deprecated":
                    break;
                case "RuntimeVisibleAnnotations":
                    break;
                case "RuntimeInvisibleAnnotations":
                    break;
                case "RuntimeVisibleParameterAnnotations":
                    break;
                case "RuntimeInvisibleParameterAnnotations":
                    break;
                case "AnnotationDefault":
                    break;
                case "BootstrapMethods":
                    break;
            }

            uint attributeCount = reader.ReadUInt32();
            var bytes = reader.ReadBytes((int)attributeCount);

            return new AttributeInfo(target, classFile, indexName, attributeCount, bytes);
        }

    }

    public class CodeAttributeInfo : AttributeInfo
    {
        public ushort MaxStack;
        public ushort MaxLocals;

        public uint CodeLength;
        public byte[] Codes;

        public ushort ExceptionTableCount;
        public ExceptionInfo[] ExceptionTable;

        public ushort AttributesCount;
        public AttributeInfo[] Attributes;

        public CodeAttributeInfo(AttributeTarget target, ClassFile classFile, ushort nameIndex, BigEndianBinaryReader reader) : base(target, classFile, nameIndex, reader.ReadUInt32())
        {
            MaxStack = reader.ReadUInt16();
            MaxLocals = reader.ReadUInt16();

            CodeLength = reader.ReadUInt32();
            Codes = reader.ReadBytes((int)CodeLength);

            ExceptionTableCount = reader.ReadUInt16();
            ExceptionTable = new ExceptionInfo[ExceptionTableCount];

            for(var i=0; i< ExceptionTableCount; i++)
                ExceptionTable[i] = new ExceptionInfo(classFile, this, reader);

            AttributesCount = reader.ReadUInt16();
            Attributes = new AttributeInfo[AttributesCount];
            for (var i = 0; i < AttributesCount; i++)
                Attributes[i] = GetAttributeInfo(target | AttributeTarget.CodeAttribute, classFile, reader);
        }
    }

    public struct ExceptionInfo
    {
        public ClassFile ClassFile;
        public CodeAttributeInfo Code;

        public ushort StartPC;
        public ushort EndPC;

        public ushort HandlerPC;
        public ushort CatchTypeIndex;

        public ConstantClassInfo CatchType => ClassFile.Constants[CatchTypeIndex] as ConstantClassInfo;

        public ExceptionInfo(ClassFile classFile, CodeAttributeInfo info, BigEndianBinaryReader reader)
        {
            ClassFile = classFile;
            Code = info;

            StartPC = reader.ReadUInt16();
            EndPC = reader.ReadUInt16();

            HandlerPC = reader.ReadUInt16();
            CatchTypeIndex = reader.ReadUInt16();
        }
    }

    /// <summary>
    /// Imeplement Fields Info like described Here:
    ///     https://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.5
    /// </summary>
    public class FieldInfo : ClassItemBase {
        public FieldAccessFlags AccessFlags;

        public ushort NameIndex;

        public ConstantUtf8Info Name => (ConstantUtf8Info)ClassFile.Constants[NameIndex];

        public ushort DescriptionIndex;
        public ConstantUtf8Info Description => (ConstantUtf8Info)ClassFile.Constants[DescriptionIndex];

        public ushort AttributesCount;
        public AttributeInfo[] Attributes;

        public FieldInfo(ClassFile classFile, BigEndianBinaryReader reader) : base(classFile) {
            AccessFlags = (FieldAccessFlags)reader.ReadUInt16();
            NameIndex = reader.ReadUInt16();
            DescriptionIndex = reader.ReadUInt16();
            AttributesCount = reader.ReadUInt16();
            Attributes = new AttributeInfo[AttributesCount];

            for(int i = 0; i < AttributesCount; i++)
                Attributes[i] = AttributeInfo.GetAttributeInfo(AttributeTarget.Field, classFile, reader);
        }
    }

    public class MethodInfo : ClassItemBase
    {
        public MethodAccessFlags AccessFlags;
        public ushort NameIndex;
        public ConstantUtf8Info Name => (ConstantUtf8Info)ClassFile.Constants[NameIndex];
        public ushort DescriptionIndex;
        public ConstantUtf8Info Description => (ConstantUtf8Info)ClassFile.Constants[DescriptionIndex];

        public ushort AttributesCount;
        public AttributeInfo[] Attributes;

        public MethodInfo(ClassFile classFile, BigEndianBinaryReader reader) : base(classFile)
        {
            AccessFlags = (MethodAccessFlags)reader.ReadUInt16();
            NameIndex = reader.ReadUInt16();
            DescriptionIndex = reader.ReadUInt16();
            AttributesCount = reader.ReadUInt16();
            Attributes = new AttributeInfo[AttributesCount];

            for(int i = 0; i < AttributesCount; i++)
                Attributes[i] = AttributeInfo.GetAttributeInfo(AttributeTarget.Method, classFile, reader);
        }
    }
}
