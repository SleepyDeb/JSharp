using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace JSharp
{
    public class JavaResourceCollection : ICollection<JavaPackageResource> {
        List<JavaPackageResource> resources;

        public JavaResourceCollection() {
            resources = new List<JavaPackageResource>();
        }

        public int Count => resources.Count;

        public bool IsReadOnly => false;

        public void Add(JavaPackageResource item) {
            resources.Add(item);
        }

        public void Clear() {
            resources.Clear();
        }

        public bool Contains(JavaPackageResource item) {
            return resources.Contains(item);
        }

        public void CopyTo(JavaPackageResource[] array, int arrayIndex) {
            resources.CopyTo(array, arrayIndex);
        }

        public IEnumerator<JavaPackageResource> GetEnumerator() {
            return resources.GetEnumerator();
        }

        public bool Remove(JavaPackageResource item) {
            return resources.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return resources.GetEnumerator();
        }
    }
}
