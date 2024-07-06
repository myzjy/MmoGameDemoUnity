namespace FrostEngine
{
    public class RootModules: Module
    {
        internal void Shutdown()
        {
            Destroy(gameObject);
        }

    }
}