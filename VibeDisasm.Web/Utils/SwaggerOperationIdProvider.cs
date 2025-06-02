using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace VibeDisasm.Web.Utils;

public static class SwaggerOperationIdProvider
{
    public static string? GenSwaggerOperationId(ApiDescription apiDescription)
    {
        return apiDescription.TryGetMethodInfo(out var methodInfo) ? methodInfo.Name : null;
    }
}
