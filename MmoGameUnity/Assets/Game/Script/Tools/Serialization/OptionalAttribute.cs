using System;

namespace Serialization
{
    //这是一种属性，允许不存在与原始数据相对应的字段。
    public class OptionalAttribute:Attribute
    {
        // 在等待服务器实现等需要暂时Optional的时候
        public bool temporary;
        // 不允许用JSON明确写为null的情况
        public bool notNull;
        // 如果没有取到值，则调用默认构造符
        public bool generateDefault;
    }
}