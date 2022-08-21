using System.Collections.Generic;
using Serialization;
using ZJYFrameWork.AttributeCustom;

namespace ZJYFrameWork
{

    public class Instructions
    {
        public List<HttpsInstructions> InstructionsList=new List<HttpsInstructions>();
    }
    
    [ObjSerializer.SuperClassAttribute(typeof(InstructionType))]
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
    [Describe("TypeS/Int")]
    public class IntInstruction : HttpsInstructions
    {
        [SerializableField("具体属性字段type")]
        public InstructionType type;

        [SerializableField("属性名")]
        public string typeName="??";
        public override string HttpDescription()
        {
            throw new System.NotImplementedException();
        }
    }
}