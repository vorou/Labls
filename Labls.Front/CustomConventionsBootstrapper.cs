using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.TinyIoc;

namespace Labls.Front
{
    public class CustomConventionsBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            Conventions.ViewLocationConventions.Add((viewName, model, context) => string.Concat("Labls.React/public/", viewName));
            Conventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("/", "Labls.React/public/"));
        }
    }
}