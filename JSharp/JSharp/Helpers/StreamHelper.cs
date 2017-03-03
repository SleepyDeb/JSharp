﻿using System.IO;

namespace JSharp.Helpers {
    public static class StreamExtensions {
        public static byte[] ReadAllBytes(this Stream instream) {
            if(instream is MemoryStream)
                return ((MemoryStream)instream).ToArray();

            using(var memoryStream = new MemoryStream()) {
                instream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
