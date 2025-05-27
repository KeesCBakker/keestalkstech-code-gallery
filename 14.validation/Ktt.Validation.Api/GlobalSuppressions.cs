// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Minor Code Smell", "S2325:Methods and properties that don't access instance data should be static", Justification = "This is the way we want to do the startup. A static would not work.", Scope = "member", Target = "~M:Ktt.Validation.Api.Startup.ConfigureServices(Microsoft.Extensions.DependencyInjection.IServiceCollection)")]
[assembly: SuppressMessage("Minor Code Smell", "S2325:Methods and properties that don't access instance data should be static", Justification = "This is the way we want to do the startup. A static would not work.", Scope = "member", Target = "~M:Ktt.Validation.Api.Startup.Configure(Microsoft.AspNetCore.Builder.IApplicationBuilder)")]
[assembly: SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Used as an example.", Scope = "member", Target = "~M:Ktt.Validation.Api.Controllers.ProvisioningController.GetComplexApplications(System.String,System.String)~Ktt.Validation.Api.Models.ComplexApplication[]")]
[assembly: SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Used as an example.", Scope = "member", Target = "~M:Ktt.Validation.Api.Controllers.ProvisioningController.GetComplexApplications(System.String,System.String,System.String)~Ktt.Validation.Api.Models.ComplexApplication[]")]
