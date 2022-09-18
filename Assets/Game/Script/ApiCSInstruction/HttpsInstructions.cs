using System.Collections.Generic;
using GameData.Net;
using Serialization;
using ZJYFrameWork.AttributeCustom;

namespace ZJYFrameWork
{
    public class Instructions
    {
        public List<HttpsInstructions> InstructionsList = new List<HttpsInstructions>();
    }

    [SuperClassAttribute(typeof(InstructionType))]
    public abstract class HttpsInstructions
    {
        public virtual string HttpDescription()
        {
            return "";
        }
    }

    [SubClass(InstructionType.None)]
    [Describe("虚拟")]
    public class NoneInstruction : HttpsInstructions
    {
        public override string HttpDescription()
        {
            return "虚拟";
        }
    }

    [SubClass(InstructionType.Int)]
    [Describe("TypeS/ValueString")]
    public class IntInstruction : HttpsInstructions
    {
        [SerializableField("具体属性字段type")] public InstructionType type;
        [SerializableField("http所属")] public PostBack httpType;
        [SerializableField("http请求方式")]   public HttpMethods _methods;

        [SerializableField("http请求路径")] public string path;

        [SerializableField("是否为Response")] public bool ResponseBool;
        [SerializableField("是否为Request")] public bool RequestBool;
        [SerializableField("是否为Api")] public bool ApiBool;
        [SerializableField("是否为modeldata")] public bool modelBool = false;
        
        [SerializableField("类型名，如果不是自定义的类型就不需要添")]
        public string InstructionString = "";

        [SerializableField("属性名")] public string typeName = "??";

        public override string HttpDescription()
        {
            throw new System.NotImplementedException();
        }
    }

    [SubClass(InstructionType.Api)]
    [Describe("TypeS/ApiBool")]
    public class ApiBoolInstruction : HttpsInstructions
    {
        [SerializableField("是否为Api")] public bool ApiBool;

        [SerializableField("当前api名字")] public string tips;
        [SerializableField("请求的http路径")] public string HttpsPath;

        public override string HttpDescription()
        {
            throw new System.NotImplementedException();
        }
    }

    [SubClass(InstructionType.Model)]
    [Describe("TypeS/Model/ModelBool")]
    public class ModelBoolInstruction : HttpsInstructions
    {
        [SerializableField("是否为具体")] public bool modelBool = false;
        [SerializableField("当前具体Model的名字")] public string tips;

        public override string HttpDescription()
        {
            throw new System.NotImplementedException();
        }
    }

    [SubClass(InstructionType.Request)]
    [Describe("TypeS/RequestBool")]
    public class RequestBoolInstruction : HttpsInstructions
    {
        [SerializableField("是否为具体")] public bool RequestBool;

        public override string HttpDescription()
        {
            throw new System.NotImplementedException();
        }
    }

    [SubClass(InstructionType.Response)]
    [Describe("TypeS/ResponseBool")]
    public class ResponseBoolInstruction : HttpsInstructions
    {
        [SerializableField("是否为具体")] public bool ResponseBool;

        public override string HttpDescription()
        {
            throw new System.NotImplementedException();
        }
    }
}