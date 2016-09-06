using Nancy;

namespace Labls.Front
{
    public class RootModule : NancyModule
    {
        public RootModule()
        {
            Get["/"] = parameters => View["index.html"];
        }
    }
}