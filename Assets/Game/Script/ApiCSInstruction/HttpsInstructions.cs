using System.Collections.Generic;
using Serialization;
using ZJYFrameWork.AttributeCustom;

namespace ZJYFrameWork
{

    public class Instructions
    {
        public List<HttpsInstructions> InstructionsList=new List<HttpsInstructions>();
    }
    
    [SuperClassAttribute(typeof(InstructionType))]
    public abstract  class HttpsInstructions
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
        [SerializableField("具体属性字段type")]
        public InstructionType type;
        [SerializableField("类型名，如果不是自定义的类型就不需要添")]
        public string InstructionString="";

        [SerializableField("属性名")]
        public string typeName="??";
        public override string HttpDescription()
        {
            throw new System.NotImplementedException();
        }
    }
    [SubClass(InstructionType.Api)]
    [Describe("TypeS/ApiBool")]
    public class ApiBoolInstruction : HttpsInstructions
    {
        [SerializableField("是否为具体")]
        public bool ApiBool;

        public string tips;
        public override string HttpDescription()
        {
            throw new System.NotImplementedException();
        }
    }
    [SubClass(InstructionType.Model)]
    [Describe("TypeS/Model/ModelBool")]
    public class ModelBoolInstruction : HttpsInstructions
    {
        [SerializableField("是否为具体")]
        public bool modelBool=false;
        [SerializableField("是否为具体")]
        public string tips;

        public override string HttpDescription()
        {
            throw new System.NotImplementedException();
        }
    }
    [SubClass(InstructionType.Request)]
    [Describe("TypeS/RequestBool")]
    public class RequestBoolInstruction : HttpsInstructions
    {
        [SerializableField("是否为具体")]
        public bool RequestBool;

        public override string HttpDescription()
        {
            throw new System.NotImplementedException();
        }
    }
    [SubClass(InstructionType.Response)]
    [Describe("TypeS/ResponseBool")]
    public class ResponseBoolInstruction : HttpsInstructions
    {
        [SerializableField("是否为具体")]
        public bool ResponseBool;

        public override string HttpDescription()
        {
            throw new System.NotImplementedException();
        }
    }
}