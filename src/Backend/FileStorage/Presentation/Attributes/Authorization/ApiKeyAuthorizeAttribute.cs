using Microsoft.AspNetCore.Mvc;

namespace Presentation.Attributes.Authorization;

public sealed class ApiKeyAuthorizeAttribute : TypeFilterAttribute<ApiKeyFilter>;