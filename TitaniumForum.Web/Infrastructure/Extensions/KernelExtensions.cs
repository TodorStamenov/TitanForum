namespace TitaniumForum.Web.Infrastructure.Extensions
{
    using Ninject;
    using Services;
    using System.Linq;
    using System.Reflection;

    public static class KernelExtensions
    {
        public static IKernel AddDomainServices(this IKernel services)
        {
            Assembly
                .GetAssembly(typeof(IService))
                .GetTypes()
                .Where(t => t.IsClass && t.GetInterfaces().Any(i => i.Name == $"I{t.Name}"))
                .Select(t => new
                {
                    Interface = t.GetInterface($"I{t.Name}"),
                    Implementation = t
                })
                .ToList()
                .ForEach(s => services.Bind(s.Interface).To(s.Implementation));

            return services;
        }
    }
}