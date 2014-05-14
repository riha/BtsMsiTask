namespace BtsMsiTask.Model
{
    public class BizTalkAssemblyResource : BaseResource
    {
        public BizTalkAssemblyResource(string assemblyFilePath) : base(assemblyFilePath, "System.BizTalk:BizTalkAssembly") { }
    }
}
