using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace SwashbucklerDiary.Rcl.Essentials
{
    // ToDo:写的不好，以后可能要改
    public class RouteHelper
    {
        private readonly List<string> routeTemplates = [];

        public RouteHelper(Assembly[] assemblies)
        {
            routeTemplates = GetRouteTemplates(assemblies);
        }

        public bool IsMatch(string path)
        {
            // https://q.cnblogs.com/q/146281
            IServiceCollection services = new ServiceCollection();
            services.AddOptions<RouteOptions>();
            using ServiceProvider sp = services.BuildServiceProvider();
            var routeOptions = sp.GetRequiredService<IOptions<RouteOptions>>();
            var constraintResolver = new DefaultInlineConstraintResolver(routeOptions, sp);
            foreach (string routeTemplate in routeTemplates)
            {
                var parsedTemplate = TemplateParser.Parse(routeTemplate);

                var values = new RouteValueDictionary();
                var matcher = new TemplateMatcher(parsedTemplate, values);
                if (matcher.TryMatch(path, values))
                {
                    if (parsedTemplate.Parameters.Count == 0)
                    {
                        return true;
                    }

                    foreach (var parameter in parsedTemplate.Parameters)
                    {
                        if (!parameter.InlineConstraints.Any())
                        {
                            return true;
                        }

                        foreach (var inlineConstraint in parameter.InlineConstraints)
                        {
                            var routeConstraint = constraintResolver.ResolveConstraint(inlineConstraint.Constraint);
                            var routeDirection = RouteDirection.IncomingRequest;
                            bool matched = routeConstraint!.Match(httpContext: null, route: null, parameter.Name!, values, routeDirection);
                            if (matched)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public static List<string> GetRouteTemplates(Assembly[] assemblies)
        {
            var routes = new List<string>();

            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    var routeAttribute = type.GetCustomAttribute<RouteAttribute>();
                    if (routeAttribute != null)
                    {
                        routes.Add(routeAttribute.Template);
                    }
                }
            }

            return routes;
        }
    }
}
