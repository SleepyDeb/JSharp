using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace JSharp.Package
{
    public class JavaPackageElementCollection : ICollection<JavaPackageElement> {
        List<JavaPackageElement> resources;

        public JavaPackageElementCollection() {
            resources = new List<JavaPackageElement>();
        }

        public int Count => resources.Count;

        public bool IsReadOnly => false;

        public void Add(JavaPackageElement item) {
            resources.Add(item);
        }

        public void Clear() {
            resources.Clear();
        }

        public bool Contains(JavaPackageElement item) {
            return resources.Contains(item);
        }

        public void CopyTo(JavaPackageElement[] array, int arrayIndex) {
            resources.CopyTo(array, arrayIndex);
        }

        public IEnumerator<JavaPackageElement> GetEnumerator() {
            return resources.GetEnumerator();
        }

        public bool Remove(JavaPackageElement item) {
            return resources.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return resources.GetEnumerator();
        }
    }
}
