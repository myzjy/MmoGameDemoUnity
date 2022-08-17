using System;

namespace ZJYFrameWork.AttributeCustom
{
    public class SerializableFieldAttribute: Attribute, IHasDescriptionAttribute
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string fieldNanme { get; private set; }
        public string description { get; private set; }

        /// <summary>
        /// 需要重写，默认fileName属性
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public virtual string GetFieldName(Type type)
        {
            return fieldNanme;
        }
        protected SerializableFieldAttribute() {}
        public SerializableFieldAttribute(string fieldName)
        {
            this.fieldNanme = fieldName;
            this.description = fieldName;
        }
        public SerializableFieldAttribute(string fieldName, string description) : this(fieldName)
        {
            this.description = description;
        }
    }
}