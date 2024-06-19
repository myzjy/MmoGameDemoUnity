using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using FrostEngine;
using ZJYFrameWork.Collection;
using ZJYFrameWork.Net.CsProtocol.Buffer;
using ZJYFrameWork.Net.CsProtocol;
using ZJYFrameWork.UISerializable.Manager;

namespace ZJYFrameWork.Net.Core
{
    /// <summary>
    /// 虚函数 客户端
    /// <exception cref="传承Tank项目"></exception>
    /// </summary>
    public abstract class AbstractClient
    {
        /////////////////////////////////////////////////////////
        // 公共代码
        //  .传入消息队列<connectionId, message>
        // (不是HashSet，因为一个连接可以有多个新消息)
        protected ConcurrentQueue<Message> receiveQueue = new ConcurrentQueue<Message>();

        // send queuec
        // => SafeQueue的速度是ConcurrentQueue的两倍，参见SafeQueue.cs!
        protected SafeQueue<byte[]> sendQueue = new SafeQueue<byte[]>();

        /// <summary>
        /// 链接id 线程id？
        /// </summary>
        private int connectionId = 0;

        /// <summary>
        /// 接受信息线程
        /// </summary>
        protected Thread receiveThread;

        /// <summary>
        /// 发送线程
        /// </summary>
        protected Thread sendThread;

        /// <summary>
        /// 唤醒发送线程的ManualResetEvent。超过Thread. Sleep
        /// -> call Set() 如果所有的东西都送去了
        /// -> call Reset()如果有什么东西要再发送一次
        /// -> call WaitOne() 阻塞，直到调用Reset
        /// </summary>
        protected ManualResetEvent sendPending = new ManualResetEvent(false);
        // queue count, useful for debugging / benchmarks

        public int ReceiveQueueCount => receiveQueue.Count;

        /// <summary>
        /// <c>如果消息队列太大，平均消息大约是20个字节，则警告:</c>
        ///  -   1k messages are   20KB
        ///  -  10k messages are  200KB
        ///  - 100k messages are 1.95MB
        ///2MB没有那么多，但是如果调用者进程不能比传入消息更快地调用GetNextMessage，这是一个不好的迹象。
        /// </summary>
        public static int messageQueueSizeWarning = 100000;

        /// <summary>
        /// TcpClient没有要检查的“连接”状态。我们需要手动跟踪它。
        /// <para> 检查的thread.IsAlive和 !Connected还不够，因为
        ///  线程是活的，连接后的一小会儿为假
        ///  断开连接，这会导致竞态条件。</para>
        /// <para>我们使用thread safe bool包装器，以保留ThreadFunction
        ///  静态的(它需要一个普通的锁)</para>
        /// <para>从这里的第一次Connect()调用开始，直到线程启动，直到TcpClient.Connect()返回，连接都为真。简单明了。</para>
        /// </summary>
        protected volatile bool connecting;

        /// <summary>
        ///<para>  从消息队列中删除并返回最早的消息。</para>
        /// <para> (可能想要调用它，直到它不再返回任何东西)</para>
        /// <para> -> 已连接、数据、未连接的事件都添加在这里</para>
        ///<para>  -> bool return使while (GetMessage(out Message))更容易!</para>
        ///<para>  -> 没有'客户端已连接'检查，因为我们仍然想在断开连接后读取断开连接的消息</para>
        /// </summary>
        public bool GetNextMessage(out Message message)
        {
            return receiveQueue.TryDequeue(out message);
        }

        /// <summary>
        /// NoDelay禁用nagle算法。降低CPU%和延迟，但增加带宽
        /// </summary>
        public bool NoDelay = true;

        /// <summary>
        /// 防止分配攻击。每个数据包都有一个长度前缀因此，
        /// 攻击者可以发送一个长度为2GB的假数据包，导致服务器分配2GB的内存，并很快耗尽。
        /// -> 如果你想发送更大的文件，只需增加最大包大小!
        /// -> 16KB 每条消息应该绰绰有余。
        /// </summary>
        public int MaxMessageSize = 16 * 1024;

        /// <summary>
        ///如果在发送过程中网络被切断，发送将永远停止，因此我们需要一个超时(以毫秒为单位)
        /// </summary>
        public int SendTimeout = 5000;

        // 避免header[4]分配，但不要对所有线程使用一个缓冲区
        [ThreadStatic] protected static byte[] header;

        // avoid payload[packetSize] allocations but don't use one buffer for
        // all threads
        [ThreadStatic] protected static byte[] payload;

        public abstract void Start();

        public abstract bool Connected();

        public abstract void Close();

        public abstract string ToConnectUrl();

        protected abstract void SendMessagesBlocking(byte[] messages, int offset, int size);

        protected abstract bool ReadMessageBlocking(out byte[] content);

        // .Read returns '0' if remote closed the connection but throws an
        // IOException if we voluntarily closed our own connection.
        //
        // let's add a ReadSafely method that returns '0' in both cases so we don't
        // have to worry about exceptions, since a disconnect is a disconnect...
        public int ReadSafely(Stream stream, byte[] buffer, int offset, int size)
        {
            try
            {
                return stream.Read(buffer, offset, size);
            }
            catch (IOException)
            {
                return 0;
            }
        }


        /////////////////////////////////////////////
        // static helper functions
        // send message (via stream) with the <size,content> message structure
        // this function is blocking sometimes!
        // (e.g. if someone has high latency or wire was cut off)
        private bool SendMessagesBlocking(byte[][] messages)
        {
            // stream.Write throws exceptions if client sends with high
            // frequency and the server stops
            try
            {
                // we might have multiple pending messages. merge into one
                // packet to avoid TCP overheads and improve performance.
                int packetSize = messages.Sum(t => t.Length);

                // create payload buffer if not created yet or previous one is
                // too small
                // IMPORTANT: payload.Length might be > packetSize! don't use it!
                if (payload == null || payload.Length < packetSize)
                {
                    payload = new byte[packetSize];
                }

                // create the packet
                int position = 0;
                foreach (var t in messages)
                {
                    Array.Copy(t, 0, payload, position, t.Length);
                    position += t.Length;
                }

                // write the whole thing
                SendMessagesBlocking(payload, 0, packetSize);

                return true;
            }
            catch (Exception exception)
            {
                // log as regular message because servers do shut down sometimes
                Debug.Log("Send: stream.Write exception: " + exception);
                return false;
            }
        }

        public virtual bool Send(byte[] data)
        {
            if (Connected())
            {
                // respect max message size to avoid allocation attacks.
                if (data.Length <= MaxMessageSize)
                {
                    // add to send queue and return immediately.
                    // calling Send here would be blocking (sometimes for long times if other side lags or wire was disconnected)
                    sendQueue.Enqueue(data);
                    // interrupt SendThread WaitOne()
                    sendPending.Set();
                    return true;
                }

                Debug.LogError($"Client.Send: message too big: {data.Length}. Limit: {MaxMessageSize}");
                return false;
            }

            Debug.Log("Client.Send: not connected!");
            return false;
        }

        protected void ReceiveLoop()
        {
            // keep track of last message queue warning
            var messageQueueLastWarningTimestamp = DateTimeUtil.Now();

            // absolutely must wrap with try/catch, otherwise thread exceptions
            // are silent
            try
            {
                // add connected event to queue with ip address as data in case
                // it's needed
                receiveQueue.Enqueue(new Message(MessageType.Connected, null));

                // let's talk about reading data.
                // -> normally we would read as much as possible and then
                //    extract as many <size,content>,<size,content> messages
                //    as we received this time. this is really complicated
                //    and expensive to do though
                // -> instead we use a trick:
                //      Read(2) -> size
                //        Read(size) -> content
                //      repeat
                //    Read is blocking, but it doesn't matter since the
                //    best thing to do until the full message arrives,
                //    is to wait.
                // => this is the most elegant AND fast solution.
                //    + no resizing
                //    + no extra allocations, just one for the content
                //    + no crazy extraction logic
                while (true)
                {
                    // read the next message (blocking) or stop if stream closed
                    byte[] content;
                    if (!ReadMessageBlocking(out content))
                    {
                        // break instead of return so stream close still happens!
                        break;
                    }

                    ByteBuffer byteBuffer = null;
                    try
                    {
                        byteBuffer = ByteBuffer.ValueOf();
                        byteBuffer.WriteBytes(content);
                        var packet = ProtocolManager.Read(byteBuffer);
                        // queue it
                        receiveQueue.Enqueue(new Message(MessageType.Data, packet));
                    }
                    finally
                    {
                        if (byteBuffer != null)
                        {
                            byteBuffer.Clear();
                        }
                    }

                    // and show a warning if the queue gets too big
                    // -> we don't want to show a warning every single time,
                    //    because then a lot of processing power gets wasted on
                    //    logging, which will make the queue pile up even more.
                    // -> instead we show it every 10s, so that the system can
                    //    use most it's processing power to hopefully process it.
                    if (receiveQueue.Count > messageQueueSizeWarning)
                    {
                        var elapsed = DateTimeUtil.Now() - messageQueueLastWarningTimestamp;
                        if (elapsed > 10 * DateTimeUtil.MILLIS_PER_SECOND)
                        {
                            Debug.Log("ReceiveLoop: messageQueue is getting big(" + receiveQueue.Count +
                                      "), try calling GetNextMessage more often. You can call it more than once per frame!");
                            messageQueueLastWarningTimestamp = DateTimeUtil.Now();
                        }
                    }
                }
            }
            catch (SocketException exception)
            {
                // this happens if (for example) the ip address is correct
                // but there is no server running on that ip/port
                Debug.LogError("Client Recv: failed to connect to {} reason={}", ToConnectUrl(), exception.ToString());

                // add 'Disconnected' event to message queue so that the caller
                // knows that the Connect failed. otherwise they will never know
                receiveQueue.Enqueue(new Message(MessageType.Disconnected, null));
            }
            catch (ThreadInterruptedException)
            {
                // expected if Disconnect() aborts it
            }
            catch (ThreadAbortException)
            {
                // expected if Disconnect() aborts it
            }
            catch (Exception exception)
            {
                // something went wrong. the thread was interrupted or the
                // connection closed or we closed our own connection or ...
                // -> either way we should stop gracefully
                Debug.Log("ReceiveLoop: finished receive function for connectionId=" + connectionId + " reason: " +
                          exception);
                // something went wrong. probably important.
                Debug.LogError($"Client Recv Exception: {exception}");
            }
            finally
            {
                // Connect might have failed. thread might have been closed.
                // let's reset connecting state no matter what.
                connecting = false;

                // clean up no matter what
                // if we got here then we are done. ReceiveLoop cleans up already,
                // but we may never get there if connect fails. so let's clean up here too.
                Close();

                // add 'Disconnected' message after disconnecting properly.
                // -> always AFTER closing the streams to avoid a race condition
                //    where Disconnected -> Reconnect wouldn't work because
                //    Connected is still true for a short moment before the stream
                //    would be closed.
                receiveQueue.Enqueue(new Message(MessageType.Disconnected, null));
            }
        }

        // thread send function
        // note: we really do need one per connection, so that if one connection
        //       blocks, the rest will still continue to get sends
        protected void SendLoop()
        {
            try
            {
                // try this. client will get closed eventually.
                while (Connected())
                {
                    // reset ManualResetEvent before we do anything else. this
                    // way there is no race condition. if Send() is called again
                    // while in here then it will be properly detected next time
                    // -> otherwise Send might be called right after dequeue but
                    //    before .Reset, which would completely ignore it until
                    //    the next Send call.
                    // WaitOne() blocks until .Set() again
                    sendPending.Reset();

                    // dequeue all
                    // SafeQueue.TryDequeueAll is twice as fast as
                    // ConcurrentQueue, see SafeQueue.cs!
                    byte[][] messages;
                    if (sendQueue.TryDequeueAll(out messages))
                    {
                        // send message (blocking) or stop if stream is closed
                        if (!SendMessagesBlocking(messages))
                        {
                            // break instead of return so stream close still happens!
                            break;
                        }
                    }

                    // don't choke up the CPU: wait until queue not empty anymore
                    sendPending.WaitOne();
                }
            }
            catch (ThreadAbortException)
            {
                // happens on stop. don't log anything.
            }
            catch (ThreadInterruptedException)
            {
                // happens if receive thread interrupts send thread.
            }
            catch (Exception exception)
            {
                // something went wrong. the thread was interrupted or the
                // connection closed or we closed our own connection or ...
                // -> either way we should stop gracefully
                Debug.Log("SendLoop Exception: connectionId=" + connectionId + " reason: " + exception);
            }
            finally
            {
                // clean up no matter what
                // we might get SocketExceptions when sending if the 'host has
                // failed to respond' - in which case we should close the connection
                // which causes the ReceiveLoop to end and fire the Disconnected
                // message. otherwise the connection would stay alive forever even
                // though we can't send anymore.
                Close();
            }
        }
    }
}