using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using JSharp.ByteCode;

namespace JSharp
{
    public class ClassFileCollection : ICollection<ClassFile> {
        List<ClassFile> classFiles;

        public int Count => classFiles.Count;

        public bool IsReadOnly => true;

        public ClassFileCollection() {
            classFiles = new List<ClassFile>();
        }

        public void Add(ClassFile item) {
            classFiles.Add(item);
        }

        public void Clear() {
            classFiles.Clear();
        }

        public bool Contains(ClassFile item) {
            return classFiles.Contains(item);
        }

        public void CopyTo(ClassFile[] array, int arrayIndex) {
            classFiles.CopyTo(array, arrayIndex);
        }

        public IEnumerator<ClassFile> GetEnumerator() {
            return classFiles.GetEnumerator();
        }

        public bool Remove(ClassFile item) {
            return classFiles.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return classFiles.GetEnumerator();
        }
    }
}
