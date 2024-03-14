namespace ZJYFrameWork
{
    public enum InstructionType
    {
        None,
        Int,
        DateTime,
        Float,
        Class,
        Enum,
        Differentiate,
        Api,
        ApiEnd,

        Request,
        RequestEnd,

        Response,
        ResponseEnd,

        Model,
    }

    public enum PostBack
    {
        None,
        Api,
        Request,
        Response,
    }

    public enum HttpMethods
    {
        Get,
        Post
    }
    
    public class HttpsInstructionsManager
    {
        
    }
}