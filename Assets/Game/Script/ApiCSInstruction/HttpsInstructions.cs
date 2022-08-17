using System.Collections.Generic;

namespace ZJYFrameWork
{

    public class Instructions
    {
        public List<HttpsInstructions> InstructionsList;
    }
    
    
    public abstract class HttpsInstructions
    {
        public abstract string HttpDescription();
    }

   
    public class NoneInstruction : HttpsInstructions
    {
        public override string HttpDescription()
        {
            return "虚拟";
        }
    }
}