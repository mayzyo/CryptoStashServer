using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoStashServer.Utilities
{
    public class AuthorizeOperationFilter : IOperationFilter
    {
        private static readonly Dictionary<string, string> scopes = new()
        {
            { "read_access", "finance.read mining.read" },
            { "write_access", "finance.write mining.write" },
            { "manage_access", "manage" }
        };

        private static readonly Dictionary<string, string> audiences = new()
        {
            { "finance_audience", "finance.read mining.read manage" },
            { "mining_audience", "finance.write mining.write manage" }
        };

        public static Dictionary<string, string> Scopes
        {
            get
            {
                return scopes
                .SelectMany(e => e.Value
                    .Split(" ")
                    .ToDictionary(x => x, _ => e.Key)
                )
                .ToDictionary(e => e.Key, e => e.Value);
            }
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (HasAuthPolicy(context, out string audience, out string scope))
            {
                operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
                operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

                operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    [
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "oauth2"
                            }
                        }
                    ] = scope != null ? new [] { scopes[scope] } : audiences[audience].Split(" ")
                }
            };

            }
        }

        private bool HasAuthPolicy(OperationFilterContext context, out string audience, out string scope)
        {
            audience = context.MethodInfo
                .DeclaringType
                .GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>()
                .FirstOrDefault()?
                .Policy;

            scope = context.MethodInfo
                .GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>()
                .FirstOrDefault()?
                .Policy;

            return audience != null || scope != null;
        }
    }
}
