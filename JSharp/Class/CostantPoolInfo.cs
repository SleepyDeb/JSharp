using JSharpPackage.Helpers;

namespace JSharpPackage.Class {
    public abstract class ConstantInfoBase : ClassItemBase
    {        
        public ConstantType Tag { get; }

        protected ConstantInfoBase(ClassFile classFile, ConstantType type) : base(classFile)
        {
            Tag = type;
        }
    }

    public class ConstantClassInfo : ConstantInfoBase
    {
        public ushort NameIndex { get; }
        public ConstantUtf8Info Name => (ConstantUtf8Info)ClassFile.Constants[NameIndex];

        public ConstantClassInfo(ClassFile classFile, BigEndianBinaryReader reader) : base(classFile, ConstantType.Class)
        {
            NameIndex = reader.ReadUInt16();
        }
    }

    public class ConstantMethodType : ConstantInfoBase
    {
        public ushort SignatureIndex { get; }
        public ConstantUtf8Info Signature => (ConstantUtf8Info)ClassFile.Constants[SignatureIndex];

        public ConstantMethodType(ClassFile classFile, BigEndianBinaryReader reader) : base(classFile, ConstantType.MethodType)
        {
            SignatureIndex = reader.ReadUInt16();
        }
    }

    public abstract class ConstantFMIInfo : ConstantInfoBase {
        public ConstantReferenceType Type { get; }
        public ushort ClassIndex { get; }
        public ushort NameAndTypeIndex { get; }

        public ConstantClassInfo Class => ClassFile.Constants[ClassIndex] as ConstantClassInfo;
        //The class_index item of a CONSTANT_Fieldref_info structure may be either a class type or an interface type. 
        public ConstantNameAndTypeInfo NameAndType => ClassFile.Constants[NameAndTypeIndex] as ConstantNameAndTypeInfo;

        protected ConstantFMIInfo(ConstantReferenceType constantType, ClassFile classFile, BigEndianBinaryReader reader, ConstantType type) : base(classFile, type) {
            Type = constantType;

            ClassIndex = reader.ReadUInt16();
            NameAndTypeIndex = reader.ReadUInt16();
        }
    }

    public enum ConstantReferenceType
    {
        FieldReference,
        MethodReference,
        InterfaceMethodReference
    }

    public class ConstantMethodHandleInfo : ConstantInfoBase
    {
        public MethodReferenceType ReferenceKind { get; }
        public ushort ReferenceIndex { get; }
        public ConstantFMIInfo Reference => (ConstantFMIInfo)ClassFile.Constants[ReferenceIndex];

        public ConstantFieldrefInfo AsFieldref => Reference as ConstantFieldrefInfo;
        public ConstantMethodrefInfo AsMethodref => Reference as ConstantMethodrefInfo;
        public ConstantInterfaceMethodrefInfo AsInterfaceMethodref => Reference as ConstantInterfaceMethodrefInfo;

        public ConstantMethodHandleInfo(ClassFile classFile, BigEndianBinaryReader reader) : base(classFile, ConstantType.MethodHandle)
        {
            ReferenceKind = (MethodReferenceType)reader.ReadByte();
            ReferenceIndex = reader.ReadUInt16();
        }
    }
    
    public class ConstantFieldrefInfo : ConstantFMIInfo
    {
        public ConstantFieldrefInfo(ClassFile classFile, BigEndianBinaryReader reader) : base(ConstantReferenceType.FieldReference, classFile, reader, ConstantType.Fieldref) { }
    }

    public class ConstantMethodrefInfo : ConstantFMIInfo
    {
        // public ConstantMethodrefInfo(ClassFile classFile, BigEndianBinaryReader reader) : base(ConstantReferenceType.MethodReference, classFile, reader, ConstantType.Methodref) { }
        public ConstantMethodrefInfo(ClassFile classFile, BigEndianBinaryReader reader) : base(ConstantReferenceType.MethodReference, classFile, reader, ConstantType.Methodref) { }
    }
    
    public class ConstantInterfaceMethodrefInfo : ConstantFMIInfo {
        public ConstantInterfaceMethodrefInfo(ClassFile classFile, BigEndianBinaryReader reader) : base(ConstantReferenceType.InterfaceMethodReference, classFile, reader, ConstantType.InterfaceMethodref) { }
    }

    public class ConstantNameAndTypeInfo : ConstantInfoBase
    {
        public ushort NameIndex { get; }
        public ushort DescriptorIndex { get; }

        public ConstantUtf8Info Name => (ConstantUtf8Info)ClassFile.Constants[NameIndex];
        public ConstantUtf8Info Description => (ConstantUtf8Info)ClassFile.Constants[DescriptorIndex];

        public ConstantNameAndTypeInfo(ClassFile classFile, BigEndianBinaryReader reader) : base(classFile, ConstantType.NameAndType)
        {
            NameIndex = reader.ReadUInt16();
            DescriptorIndex = reader.ReadUInt16();
        }
    }

    public class ConstantStringInfo : ConstantInfoBase
    {
        public ushort StringIndex { get; }

        public ConstantUtf8Info StringInfo => (ConstantUtf8Info)ClassFile.Constants[StringIndex];

        public ConstantStringInfo(ClassFile classFile, BigEndianBinaryReader reader) : base(classFile, ConstantType.String)
        {
            StringIndex = reader.ReadUInt16();
        }
    }

    public class ConstantInvokeDynamicInfo : ConstantInfoBase
    {
        public int BootstrapMethodIndex { get; } //TODO: BootstrapMethods
        public AttributeInfo BoostrapMethod => ClassFile.Attributes[BootstrapMethodIndex];
        public int NameAndTypeIndex { get; }

        public ConstantNameAndTypeInfo NameAndType => ClassFile.Constants[NameAndTypeIndex] as ConstantNameAndTypeInfo;

        public ConstantInvokeDynamicInfo(ClassFile classFile, BigEndianBinaryReader reader) : base(classFile, ConstantType.InvokeDynamic)
        {
            BootstrapMethodIndex = reader.ReadUInt16();
            NameAndTypeIndex = reader.ReadUInt16();
        }
    }

    public class ConstantFloatInfo : ConstantInfoBase
    {
        public float Value { get; }

        public ConstantFloatInfo(ClassFile classFile, BigEndianBinaryReader reader) : base(classFile, ConstantType.Float)
        {
            Value = reader.ReadSingle();
        }
    }

    public class ConstantDoubleInfo : ConstantInfoBase
    {
        public double Value { get; }

        public ConstantDoubleInfo(ClassFile classFile, BigEndianBinaryReader reader) : base(classFile, ConstantType.Float)
        {
            Value = reader.ReadDouble();
        }
    }

    public class ConstantIntegerInfo : ConstantInfoBase
    {
        public int Value { get; }

        public ConstantIntegerInfo(ClassFile classFile, BigEndianBinaryReader reader) : base(classFile, ConstantType.Integer)
        {
            Value = reader.ReadInt32();
        }
    }

    public class ConstantLongInfo : ConstantInfoBase
    {
        public long Value { get; }

        public ConstantLongInfo(ClassFile classFile, BigEndianBinaryReader reader) : base(classFile, ConstantType.Long)
        {
            Value = reader.ReadInt64();
        }
    }

    public class ConstantUtf8Info : ConstantInfoBase
    {
        public ushort Length { get; }
        public byte[] Bytes { get; }

        public string Value => BigEndianBinaryReader.UTF8ToString(Bytes);

        public ConstantUtf8Info(ClassFile classFile, BigEndianBinaryReader reader) : base(classFile, ConstantType.Utf8)
        {
            Length = reader.ReadUInt16();
            Bytes = reader.ReadBytes(Length);
        }

        public override string ToString()
        {
            return Value;
        }
    }

    public enum ConstantType : byte
    {
        Utf8 = 1,
        Class = 7,
        Fieldref = 9,
        Methodref = 10,
        InterfaceMethodref = 11,
        String = 8,
        Integer = 3,
        Float = 4,
        Long = 5,
        Double = 6,
        NameAndType = 12,
        MethodHandle = 15,
        MethodType = 16,
        InvokeDynamic = 18
    }

    public enum MethodReferenceType : byte {
        GetField = 1, // C.f:T
        GetStatic = 2, // C.f:T
        PutField = 3, // C.f:T
        PutStatic = 4, // C.f:T
        InvokeVirtual = 5, // C.m:(A*)T
        InvokeStatic = 6, // C.m:(A*)T
        InvokeSpecial = 7, // C.m:(A*)T
        NewInvokeSpecial = 8, // C.<init>:(A*)void
        InvokeInterface = 9  // C.m:(A*)T
    }
}
