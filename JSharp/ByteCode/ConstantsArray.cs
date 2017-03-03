using System.Collections;
using System.Collections.Generic;

namespace JSharp.ByteCode {
    public class ConstantsArray<T> : ClassItemBase, IEnumerable<T> where T : class {
        private ushort[] Indexes { get; }
        public T this[int i] => ClassFile.Constants[Indexes[i]] as T;

        public ConstantsArray(ClassFile classFile, ushort[] indexes) : base(classFile) {
            Indexes = indexes;
        }

        public IEnumerator<T> GetEnumerator() {
            foreach(var i in Indexes)
                yield return ClassFile.Constants[i] as T;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }

}
