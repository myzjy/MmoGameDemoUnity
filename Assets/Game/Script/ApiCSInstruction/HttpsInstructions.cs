using System.Collections.Generic;
using ZJYFrameWork.AttributeCustom;

namespace ZJYFrameWork
{

    public class Instructions
    {
        public List<HttpsInstructions> InstructionsList=new List<HttpsInstructions>();
    }
    
    [SubClass(typeof(InstructionType))]
    public abstract class HttpsInstructions
    {
        public abstract string HttpDescription();
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