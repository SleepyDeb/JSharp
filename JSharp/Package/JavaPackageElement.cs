using System;
using System.Collections.Generic;
using System.Text;
using JSharp.Package;

namespace JSharp.Package
{
    public abstract class JavaPackageElement {
        public JavaPackage Parent { get; }
        public string Name { get; }

        public JavaPackageElementTypes Type { get; }
        public virtual string Extension { get; }
        
        protected JavaPackageElement(JavaPackage parent, string name, JavaPackageElementTypes type) {
            Parent = parent;
            Name = name;

            Type = type;

            Extension = Type.GetExtension();
        }
        
        public override string ToString() {
            var sb = new StringBuilder();

            if(Parent != null) {
                sb.Append(Parent);

                if(Parent is JavaArchive) sb.Append('/');
                else sb.Append('.');
            }

            sb.Append(Name);
            
            if(Type!=JavaPackageElementTypes.Package)
                if(!string.IsNullOrWhiteSpace(Extension))
                    sb.Append(Extension);
            
            return sb.ToString();
        }
    }
}
