using System;

// ReSharper disable once CheckNamespace
namespace ZJYFrameWork.AttributeCustom
{
    public class DescribeAttribute : Attribute, IHasDescriptionAttribute
    {
        public string description { get; private set; }

        public DescribeAttribute(string description)
        {
            this.description = description;
        }
    }

    public class SubClassAttribute : Attribute
    {
        public object label { get; private set; }

        public SubClassAttribute(object label)
        {
            this.label = label;
        }
    }
}