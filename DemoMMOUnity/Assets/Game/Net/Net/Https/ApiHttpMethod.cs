using System;
using BestHTTP;
using Unity.VisualScripting;

namespace Game.Net.Net.Https
{
    public class ApiHttpMethodAttribute:Attribute
    {
        [Value]
        public HttpMethods Methods;
    }
}