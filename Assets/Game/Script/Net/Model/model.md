# 这里存放类似于配置表的结构类型

格式

#### 一般都会创建两个类文件
#### 分别是
类名 | 类名.Generated
>文件名只有类名
>里面格式
>
例如
````
    public partial class Error
    {
      
    }
````


>文件名有类名.Generated
>里面格式
>
例如
````
    public partial class 类名:SeverModel
    {
        //各种属性
        protected override string ToJson()
        {
            return 类名Serializer.ToJson(this);
        }
    }
    public class 类名Serializer:Serializer
    {
        public static string ToJson(Error model)
        {
            StringBuilder builder = new StringBuilder();
            ToJson(model, builder, true, 0);
            return builder.ToString();
        }
        public static void ToJson(Error model,StringBuilder builder, bool isPretty = false,int index=0)
        {
            builder.Append("{");
            NewLine(builder,isPretty);
            Indent(builder,index+1,isPretty);
            //各种属性
            builder.Append($"\"commandId\":{model.commandId},");
            NewLine(builder,isPretty);
            Indent(builder,index+1,isPretty);
            builder.Append($"\"message\":{model.message}");
            NewLine(builder,isPretty);
            Indent(builder,index+1,isPretty);
            builder.Append("}");

        }
    }
````