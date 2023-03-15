namespace BestHTTP
{
    /// <summary>
    /// Some supported methods described in the rfc: http://www.w3.org/Protocols/rfc2616/rfc2616-sec9.html#sec9
    /// </summary>
    public enum HttpMethods : byte
    {
        /// <summary>
        /// <p>GET方法意味着检索由Request-URI标识的任何信息(以实体形式)。</p>
        /// <p>如果Request-URI指的是一个产生数据的过程，则产生的数据将作为</p>
        /// <p>响应中的实体，而不是流程的源文本，除非该文本恰好是流程的输出。</p>
        /// </summary>
        Get,

        /// <summary>
        /// <p>HEAD方法与GET方法相同，只是服务器不能在响应中返回消息体。</p>
        /// <p>响应HEAD请求的HTTP报头中包含的元信息应该与响应GET请求时发送的信息相同。</p>
        /// <p>该方法可用于获取请求中隐含的实体的元信息，而无需传输实体本体本身。</p>
        /// <p>该方法通常用于测试超文本链接的有效性、可访问性和最近的修改。</p>
        /// </summary>
        Head,

        /// <summary>
        /// <p>POST方法用于请求源服务器接受请求中包含的实体，作为请求行中由request - uri标识的资源的新从属。</p>
        /// POST被设计为允许一个统一的方法覆盖以下函数:
        /// <list type="bullet">
        ///     <item><description>现有资源的注释;</description></item>
        ///     <item><description>向公告栏、新闻组、邮件列表或类似的文章组发布消息;</description></item>
        ///     <item><description>向数据处理过程提供数据块，例如提交表单的结果;</description></item>
        ///     <item><description>通过追加操作扩展数据库。</description></item>
        /// </list>
        /// <p>POST方法执行的实际功能由服务器决定，通常依赖于Request-URI。</p>
        /// <p>发布的实体从属于该URI，就像文件从属于包含它的目录一样。</p>
        /// <p>新闻文章从属于其发布的新闻组，或者一条记录从属于数据库。</p>
        /// <p>POST方法执行的操作可能不会产生可以由URI标识的资源。在这种情况下,</p>
        /// <p>200 (OK)或204 (No Content)是适当的响应状态，取决于响应是否包含描述结果的实体。</p>
        /// </summary>
        Post,

        /// <summary>
        /// <p>PUT方法请求将包含的实体存储在提供的Request-URI下。</p>
        /// <p>如果Request-URI引用的是一个已经存在的资源，所包含的实体应该被认为是原始服务器上的实体的修改版本。</p>
        /// <p>如果请求URI不指向现有资源，并且该URI能够被请求用户代理定义为新资源，</p>
        /// <p>源服务器可以使用该URI创建资源。如果创建了新资源，源服务器必须通过201(已创建)响应通知用户代理。</p>
        /// <p>如果现有的资源被修改，应该发送200 (OK)或204 (No Content)响应码来表示请求成功完成。</p>
        /// <p>如果资源不能用Request-URI创建或修改，则应该给出反映问题性质的适当错误响应。</p>
        /// <p>实体的接收者绝对不能忽略任何它不理解或不实现的Content-*(例如Content- range)报头，在这种情况下必须返回501(未实现)响应。</p>
        /// </summary>
        Put,

        /// <summary>
        /// <p>DELETE方法请求源服务器删除Request-URI标识的资源。源服务器上的人工干预(或其他方式)可以覆盖此方法。</p>
        /// <p>客户端不能保证操作已经被执行，即使从源服务器返回的状态码表明操作已经成功完成。</p>
        /// <p>但是，服务器不应该表示成功，除非在给出响应时，它打算删除资源或将其移动到一个不可访问的位置。</p>
        /// <p>一个成功的响应应该是200 (OK)如果响应包括一个描述状态的实体，202(已接受)如果动作尚未生效，或204(无内容)</p>
        /// <p>如果动作已经生效，但响应不包括实体。</p>
        /// </summary>
        Delete,

        /// <summary>
        /// http://tools.ietf.org/html/rfc5789
        /// <p>PATCH方法请求将请求实体中描述的一组更改应用于request - uri标识的资源。</p>
        /// <p>更改集以一种被称为“patchDocument”的格式表示，由媒体类型标识。如果Request-URI不指向现有资源，</p>
        /// <p>服务器可以创建一个新的资源，这取决于补丁文件类型(是否可以在逻辑上修改一个空资源)和权限等。</p>
        /// </summary>
        Patch,

        /// <summary>
        /// HTTP方法PATCH可用于更新部分资源。例如，当您只需要更新资源的一个字段时，放置一个完整的资源表示可能会很麻烦，并且会占用更多的带宽。
        /// <seealso href="http://restcookbook.com/HTTP%20Methods/patch/"/>
        /// </summary>
        Merge,

        Options,

        /// <summary>
        /// https://tools.ietf.org/html/rfc8441
        /// </summary>
        Connect
    }
}