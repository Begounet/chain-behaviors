using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Pool;

namespace ChainBehaviors.IO
{
    /// <summary>
    /// Provide a way to distribute access to multiple stream users.
    /// The idea is to be able to have only one reader and only one writer on a stream.
    /// </summary>
    /// <example>
    /// StreamAccess access = StreamAccess.Get(stream);
    /// var myReaderHandle = access.Reserve(AccessType.Read);
    /// var myWriterHandle = access.Reserve(AccessType.Write);
    /// var mySecondWriterHandle = access.Reserve(AccessType.Write); // => Throws exception because there is already one writer.
    /// access.Release(myReaderHandle); // Release reader
    /// access.Reset(); // Clean all accesses.
    /// </example>
    public class StreamAccess
    {
        public class Handle
        {
            public StreamAccess StreamAccess { get; private set; }
            public Stream Stream { get; private set; }
            public AccessType AccessType { get; private set; }

            public Handle() { }

            internal void Init(StreamAccess streamAccess, Stream stream, AccessType accessType)
            {
                StreamAccess = streamAccess;
                Stream = stream;
                AccessType = accessType;
            }
            
            public void Release() => StreamAccess.Release(this);
        }

        [Flags]
        public enum AccessType
        {
            None = 0,
            Read = 1 << 1,
            Write = 1 << 2,
            ReadWrite = Read | Write,
        }

        private Stream _stream;
        private List<Handle> _handles = new List<Handle>();

        private int _readerCount = 0;
        private int _writerCount = 0;

        public StreamAccess() { }

        public static StreamAccess Get(Stream stream)
        {
            var streamHandle = UnsafeGenericPool<StreamAccess>.Get();
            streamHandle.Reset();
            streamHandle._stream = stream;
            return streamHandle;
        }

        public Handle Reserve(AccessType accessType)
        {
            bool doesRead = accessType.HasFlag(AccessType.Read);
            bool doesWrite = accessType.HasFlag(AccessType.Write);
            bool doesAnyOp = doesRead || doesWrite;

            // Ensure there is no access conflict
            if (doesAnyOp && _writerCount > 0)
            {
                throw new Exception($"Cannot request access '{accessType}' because there is already a writer on the stream.");
            }
            if (doesWrite && _readerCount > 0)
            {
                throw new Exception($"Cannot request '{accessType}' access because some scripts are reading the stream.");
            }

            var handle = GenericPool<Handle>.Get();
            handle.Init(this, _stream, accessType);
            _handles.Add(handle);
            return handle;
        }

        private void Release(Handle handle)
        {
            _handles.Remove(handle);
            GenericPool<Handle>.Release(handle);

            // Differ the closing because someone else may reserve
            // the stream during the same frame but later
            TryCloseDeferred().Forget();
        }

        private void Reset()
        {
            _stream = null;
            _readerCount = 0;
            _writerCount = 0;
        }

        private async UniTaskVoid TryCloseDeferred()
        {
            await UniTask.Yield(PlayerLoopTiming.PreLateUpdate);
            if (_handles.Count == 0)
            {
                _stream.Close();
                UnsafeGenericPool<StreamAccess>.Release(this);
            }
        }
    }
}
