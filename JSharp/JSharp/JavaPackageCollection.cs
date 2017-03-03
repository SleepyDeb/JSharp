using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace JSharp.Package
{
    public class JavaPackageCollection : ICollection<JavaPackage> {
        List<JavaPackage> Packages;

        public JavaPackageCollection() {
            Packages = new List<JavaPackage>();
        }

        public int Count => Packages.Count;

        public bool IsReadOnly => false;

        public void Add(JavaPackage item) {
            Packages.Add(item);            
        }

        public void Clear() {
            Packages.Clear();
        }

        public bool Contains(JavaPackage item) {
            return Packages.Contains(item);
        }

        public void CopyTo(JavaPackage[] array, int arrayIndex) {
            Packages.CopyTo(array, arrayIndex);
        }

        public IEnumerator<JavaPackage> GetEnumerator() {
            return Packages.GetEnumerator();
        }

        public bool Remove(JavaPackage item) {
            return Packages.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return Packages.GetEnumerator();
        }

        public JavaPackage GetByName(string name) {
            return Packages.Find(p => p.Name == name);
        }
    }
}
