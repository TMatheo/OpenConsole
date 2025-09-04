using MelonLoader;

namespace OpenCommunicator
{
    public class Entry : MelonPlugin
    {
        public override void OnPreInitialization()
        {
            LogHandler.Setup();
        }
    }
}
