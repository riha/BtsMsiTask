namespace BtsMsiTask.Model
{
    public class AssemblyResource : BaseResource
    {
        public AssemblyResource(string assemblyFilePath) : base(assemblyFilePath, "System.BizTalk:Assembly") { }
    }
}
