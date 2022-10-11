﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using System.IO;
using System;

namespace PxWeb
{
    public class CacheMiddleware
    {
        private readonly RequestDelegate _next;
        private string _cacheLock = "lock";

        public CacheMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        private async Task<HttpResponse> readResponse(HttpContext httpContext)
        {
            using (var ms = new MemoryStream())
            {
                Stream originalStream = httpContext.Response.Body;
                httpContext.Response.Body = ms;
                await _next(httpContext);

                ms.Seek(0, SeekOrigin.Begin);
                string body = await new StreamReader(ms).ReadToEndAsync();
                ms.Seek(0, SeekOrigin.Begin);

                httpContext.Response.Body = originalStream;

                string contentType = httpContext.Response.ContentType;
                HttpResponse response = new HttpResponse(body, contentType);
                return response;
            }
        }

        private string generateKey(HttpRequest request, string body)
        {
            // Get url
            string url = $"{request.Method}:{request.Scheme}://{request.Host.Value}{request.Path}{request.QueryString}";
            string key = $"{url}";
            if (request.Method == "POST" && body != "")
            {
                key += $":{body}";
            }
            return key;
        }

        public async Task Invoke(HttpContext httpContext, IPxCache cache)
        {
            HttpRequest request = httpContext.Request;

            string body = await new StreamReader(request.Body).ReadToEndAsync();
            string key = generateKey(request, body);

            HttpResponse response;
            HttpResponse? cached = cache.Get<HttpResponse>(key);
            if (cached is null)
            {
                response = readResponse(httpContext).Result;
 
                lock (_cacheLock)
                {
                    HttpResponse? freshCached = cache.Get<HttpResponse>(key);
                    if (freshCached is null)
                    {
                        cache.Set(key, response);
                    }
                    else
                    {
                        response = freshCached;
                    }
                }
            }
            else
            {
                response = cached;
            }

            httpContext.Response.ContentType = response.contentType;
            await httpContext.Response.WriteAsync(response.content);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseCacheMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CacheMiddleware>();
        }
    }
}
