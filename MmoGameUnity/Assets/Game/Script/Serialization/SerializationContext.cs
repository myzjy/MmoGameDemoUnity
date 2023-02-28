using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Serialization
{
    public class SerializationContext
    {
        Stack<object> m_debugInfo = new Stack<object>();
        public bool isFull { get; private set; }
        public byte[] rndKeyBuffer = new byte[sizeof(int)];
        public Random random = new Random();

        public SerializationContext()
        {
        }

        public SerializationContext(bool isFull)
        {
            this.isFull = isFull;
        }

        public List<object> Capture()
        {
#if UNITY_EDITOR
            return m_debugInfo.ToList();
#else
			return null;
#endif
        }

        [Conditional("UNITY_EDITOR")]
        public void Push(object o)
        {
            m_debugInfo.Push(o);
        }

        [Conditional("UNITY_EDITOR")]
        public void Pop(object o)
        {
            while (m_debugInfo.Peek() != o)
            {
                m_debugInfo.Pop();
            }

            m_debugInfo.Pop();
        }
    }
}