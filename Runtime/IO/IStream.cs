using System.IO;
using UnityEngine;

namespace ChainBehaviors.IO
{
    public interface IStream
    {
        public Stream CreateStream(long length = 0);
    }
}
