using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace JSharp.Package
{
    /// <summary>
    /// Describe all Package Common Files Type
    /// </summary>
    public enum JavaPackageElementTypes
    {
        /// <summary>
        /// .Class files
        /// </summary>
        Class,

        /// <summary>
        /// .MD files
        /// </summary>
        Manifest,

        /// <summary>
        /// A directory that contains .class or resource
        /// </summary>
        Package,

        /// <summary>
        /// All other files types
        /// </summary>
        Other
    }

    internal static class JavaPackageElementEx {
        private static Dictionary<string, JavaPackageElementTypes> _typesByEx = new Dictionary<string, JavaPackageElementTypes>(){
            { ".class", JavaPackageElementTypes.Class },
            { ".MF", JavaPackageElementTypes.Manifest },
            { "/", JavaPackageElementTypes.Package },
            { string.Empty, JavaPackageElementTypes.Other }
        };

        public static Dictionary<string, JavaPackageElementTypes> TypesByEx => _typesByEx;

        public static string GetExtension(this JavaPackageElementTypes type) {
            return _typesByEx.FirstOrDefault(t => t.Value == type).Key ?? string.Empty;
        }

        public static JavaPackageElementTypes GetTypeFromString(string name) {
            foreach(var key in _typesByEx.Keys)
                if(name.EndsWith(key))
                    return _typesByEx[key];

            return JavaPackageElementTypes.Other;
        }
    }
}
