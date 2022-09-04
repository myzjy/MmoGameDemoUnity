using System;

// ReSharper disable once CheckNamespace
namespace ZJYFrameWork.AttributeCustom
{
    public class DescribeAttribute : Attribute, IHasDescriptionAttribute
    {
        public string Description { get; private set; }

        public DescribeAttribute(string description)
        {
            this.Description = description;
        }
    }
    
}