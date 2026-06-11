using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace alposim.Helper{

    public class AuthorizationHeaderParameterOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var filterPipeline = context.ApiDescription.ActionDescriptor.FilterDescriptors;
            var isAuthorized = filterPipeline
                .Select(filterInfo => filterInfo.Filter)
                .Any(filter => filter is AuthorizeFilter);
            var allowAnonymous = filterPipeline
                .Select(filterInfo => filterInfo.Filter)
                .Any(filter => filter is AllowAnonymousFilter);

            if (!isAuthorized && !allowAnonymous)
            {
                operation.Parameters = new List<IOpenApiParameter>();
                operation.Parameters.Add(new OpenApiParameter
                    {
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Description = "Bearer token",
                        Required = true,
                    }
                );
            }
                
        }
    }
}
