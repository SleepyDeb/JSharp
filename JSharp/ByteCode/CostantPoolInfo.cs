using JSharp.Helpers;

namespace JSharp.ByteCode {
    /// <summary>
    /// Base Constant Pool structure as described here:
    ///     https://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.4
    /// </summary>
    public abstract class ConstantInfoBase : ClassItemBase
    {
        /// <summary>
        /// Each item in the constant_pool table must begin with a 1-byte tag indicating
        /// the kind of cp_info entry.
        /// </summary>
        public ConstantType Tag { get; }

        protected ConstantInfoBase(ClassFile classFile, ConstantType type) 
            : base(classFile)
        {
            Tag = type;
        }
    }

    /// <summary>
    /// Constant Class Info structure as described here:
    ///     https://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.4.1
    /// </summary>
    public class ConstantClassInfo : ConstantInfoBase
    {
        /// <summary>
        /// The value of the name_index item must be a valid index into the constant_pool table. 
        /// The constant_pool entry at that index must be a CONSTANT_Utf8_info structure representing 
        /// a valid binary class or interface name encoded in internal form.
        /// </summary>
        public ushort NameIndex { get; }

        /// <summary>
        /// Name pointed by NameIndex
        /// TODO: Name parsing
        /// </summary>
        public ConstantUtf8Info Name => (ConstantUtf8Info)ClassFile.Constants[NameIndex];

        public ConstantClassInfo(ClassFile classFile, BigEndianBinaryReader reader) 
            : base(classFile, ConstantType.Class)
        {
            NameIndex = reader.ReadUInt16();
        }
    }

    /// <summary>
    /// Shared property of Constant Fieldref\Methodref\InterfaceMethodref Info, as described:
    ///     https://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.4.2
    /// </summary>
    public abstract class ConstantFMIInfo : ConstantInfoBase {
        /// <summary>
        /// Rappresent the type of this CI instance (Fieldref\Methodref\InterfaceMethodref)
        /// </summary>
        public ConstantReferenceType Type { get; }

        /// <summary>
        /// The value of the class_index item must be a valid index into the constant_pool table.
        ///  The constant_pool entry at that index must be a CONSTANT_Class_info structure 
        ///  representing a class or interface type that has the field or method as a member.
        ///  
        ///     The classindex item of a CONSTANT_Methodref_info structure must be a class type, not an interface type.
        ///     The classindex item of a CONSTANT_InterfaceMethodref_info structure must be an interface type.
        ///     The classindex item of a CONSTANT_Fieldref_info structure may be either a class type or an interface type.
        /// </summary>
        public ushort ClassIndex { get; }

        /// <summary>
        /// This the class info pointed by ClassIndex
        /// </summary>
        public ConstantClassInfo Class => ClassFile.Constants[ClassIndex] as ConstantClassInfo;

        /// <summary>
        /// This constant_pool entry indicates the name and descriptor of the field or method.
        /// 
        /// In a CONSTANT_Fieldref_info, the indicated descriptor must be a field descriptor. 
        ///     https://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.3.2
        ///     
        /// Otherwise, the indicated descriptor must be a method descriptor.
        ///     https://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.3.3
        ///     
        /// If the name of the method of a CONSTANT_Methodref_info structure begins with a '<'
        /// then the name must be the special name<init>, representing an instance initialization method. 
        ///     https://docs.oracle.com/javase/specs/jvms/se8/html/jvms-2.html#jvms-2.9
        ///     
        /// The return type of such a method must be void.
        /// </summary>
        public ushort NameAndTypeIndex { get; }

        /// <summary>
        /// This is the ConstantNameAndTypeInfo pointed by NameAndTypeIndex
        /// </summary>
        public ConstantNameAndTypeInfo NameAndType => ClassFile.Constants[NameAndTypeIndex] as ConstantNameAndTypeInfo;

        protected ConstantFMIInfo(ConstantReferenceType constantType, ClassFile classFile, BigEndianBinaryReader reader, ConstantType type) 
            : base(classFile, type) {
            Type = constantType;

            ClassIndex = reader.ReadUInt16();
            NameAndTypeIndex = reader.ReadUInt16();
        }
    }

    /// <summary>
    /// Constant Fieldref Info as described here:
    ///     https://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.4.2
    /// </summary>
    public class ConstantFieldrefInfo : ConstantFMIInfo
    {
        public ConstantFieldrefInfo(ClassFile classFile, BigEndianBinaryReader reader) 
            : base(ConstantReferenceType.FieldReference, classFile, reader, ConstantType.Fieldref) { }
    }

    /// <summary>
    /// Constant Fieldref Info as described here:
    ///     https://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.4.2
    /// </summary>
    public class ConstantMethodrefInfo : ConstantFMIInfo
    {
        public ConstantMethodrefInfo(ClassFile classFile, BigEndianBinaryReader reader) 
            : base(ConstantReferenceType.MethodReference, classFile, reader, ConstantType.Methodref) { }
    }

    /// <summary>
    /// Constant Fieldref Info as described here:
    ///     https://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.4.2
    /// </summary>
    public class ConstantInterfaceMethodrefInfo : ConstantFMIInfo
    {

        public ConstantInterfaceMethodrefInfo(ClassFile classFile, BigEndianBinaryReader reader) 
            : base(ConstantReferenceType.InterfaceMethodReference, classFile, reader, ConstantType.InterfaceMethodref) { }
    }

    /// <summary>
    /// Constant Method structure as described here:
    ///     https://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.6
    /// </summary>
    public class ConstantMethodType : ConstantInfoBase
    {
        public ushort SignatureIndex { get; }

        public ConstantUtf8Info Signature => (ConstantUtf8Info)ClassFile.Constants[SignatureIndex];

        public ConstantMethodType(ClassFile classFile, BigEndianBinaryReader reader) : base(classFile, ConstantType.MethodType)
        {
            SignatureIndex = reader.ReadUInt16();
        }
    }

    /// <summary>
    /// Constant Method Handle info structue: 
    ///     https://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.4.8
    /// </summary>
    public class ConstantMethodHandleInfo : ConstantInfoBase
    {
        /// <summary>
        /// The value of the reference_kind item must be in the range 1 to 9. 
        /// The value denotes the kind of this method handle, which characterizes its bytecode behavior.
        /// </summary>
        public MethodReferenceType ReferenceKind { get; }

        /// <summary>
        /// The value of the reference_index item must be a valid index into the constant_pool table. 
        /// The constant_pool entry at that index must be as follows:
        /// </summary>
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

    /// <summary>
    /// Constant Name And Type 
    ///     https://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.4.6    
    /// </summary>
    public class ConstantNameAndTypeInfo : ConstantInfoBase
    {
        /// <summary>
        /// The constant_pool entry at that index must be a CONSTANT_Utf8_info structure
        /// representing either the special method name <init> 
        /// or a valid unqualified name denoting a field or method.
        /// </summary>
        public ushort NameIndex { get; }

        /// <summary>
        /// Decoded NameIndex
        /// </summary>
        public ConstantUtf8Info Name => (ConstantUtf8Info)ClassFile.Constants[NameIndex];

        /// <summary>
        /// The constant_pool entry at that index must be a CONSTANT_Utf8_info structure
        /// representing a valid field descriptor or method descriptor.
        /// </summary>
        public ushort DescriptorIndex { get; }

        /// <summary>
        /// Decoded DescriptorIndex
        /// </summary>
        public ConstantUtf8Info Description => (ConstantUtf8Info)ClassFile.Constants[DescriptorIndex];

        public ConstantNameAndTypeInfo(ClassFile classFile, BigEndianBinaryReader reader) : base(classFile, ConstantType.NameAndType)
        {
            NameIndex = reader.ReadUInt16();
            DescriptorIndex = reader.ReadUInt16();
        }
    }

    /// <summary>
    /// Constant String Info 
    ///     https://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.4.3
    /// </summary>
    public class ConstantStringInfo : ConstantInfoBase
    {
        /// <summary>
        /// he constant_pool entry at that index must be a CONSTANT_Utf8_info structure 
        /// representing the sequence of Unicode code points to which the String object is to be initialized.
        /// </summary>
        public ushort StringIndex { get; }

        public ConstantUtf8Info StringInfo => (ConstantUtf8Info)ClassFile.Constants[StringIndex];

        public ConstantStringInfo(ClassFile classFile, BigEndianBinaryReader reader) : base(classFile, ConstantType.String)
        {
            StringIndex = reader.ReadUInt16();
        }

        public override string ToString()
        {
            return StringInfo.Value;
        }
    }

    /// <summary>
    /// Constant Float Info
    ///     https://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.4.10
    /// </summary>
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

    /// <summary>
    /// Cosntant Float Info
    ///     https://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.4.4
    /// </summary>
    public class ConstantFloatInfo : ConstantInfoBase
    {
        public float Value { get; }

        public ConstantFloatInfo(ClassFile classFile, BigEndianBinaryReader reader) : base(classFile, ConstantType.Float)
        {
            Value = reader.ReadSingle();
        }
    }

    /// <summary>
    /// Constant Double Info
    ///     https://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.4.5
    /// </summary>
    public class ConstantDoubleInfo : ConstantInfoBase
    {
        public double Value { get; }

        public ConstantDoubleInfo(ClassFile classFile, BigEndianBinaryReader reader) : base(classFile, ConstantType.Float)
        {
            Value = reader.ReadDouble();
        }
    }

    /// <summary>
    /// Constant Integer info
    ///     https://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.4.4
    /// </summary>
    public class ConstantIntegerInfo : ConstantInfoBase
    {
        public int Value { get; }

        public ConstantIntegerInfo(ClassFile classFile, BigEndianBinaryReader reader) : base(classFile, ConstantType.Integer)
        {
            Value = reader.ReadInt32();
        }
    }

    /// <summary>
    /// Constant Long info
    ///     https://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.4.5
    /// </summary>
    public class ConstantLongInfo : ConstantInfoBase
    {
        public long Value { get; }

        public ConstantLongInfo(ClassFile classFile, BigEndianBinaryReader reader) : base(classFile, ConstantType.Long)
        {
            Value = reader.ReadInt64();
        }
    }

    /// <summary>
    /// Constant UTF8 Info
    ///     https://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.4.7
    /// </summary>
    public class ConstantUtf8Info : ConstantInfoBase
    {
        /// <summary>
        /// Lenght of the string
        /// </summary>
        public ushort Length { get; }

        /// <summary>
        /// Payload of the string
        /// The bytes array contains the bytes of the string.
        /// No byte may have the value(byte)0.
        /// No byte may lie in the range(byte)0xf0 to(byte)0xff.
        /// </summary>
        public byte[] Bytes { get; }

        /// <summary>
        /// Decoded payload
        /// </summary>
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
}
