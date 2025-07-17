using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace SwashbucklerDiary.Rcl.Essentials
{
    public class RouteMatcher
    {
        private readonly List<string> routeTemplates = [];
        private readonly DefaultInlineConstraintResolver constraintResolver;

        public RouteMatcher(IEnumerable<Assembly> assemblies)
        {
            routeTemplates = GetRouteTemplates(assemblies);

            IServiceCollection services = new ServiceCollection();
            services.AddOptions<RouteOptions>();
            ServiceProvider sp = services.BuildServiceProvider();
            var routeOptions = sp.GetRequiredService<IOptions<RouteOptions>>();
            constraintResolver = new DefaultInlineConstraintResolver(routeOptions, sp);
        }

        public bool IsMatch(string path)
        {
            // https://q.cnblogs.com/q/146281

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

        private static List<string> GetRouteTemplates(IEnumerable<Assembly> assemblies)
        {
            var routes = new List<string>();

            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    var routeAttribute = type.GetCustomAttribute<RouteAttribute>();
                    if (routeAttribute is not null)
                    {
                        routes.Add(routeAttribute.Template);
                    }
                }
            }

            return routes;
        }
    }
}
