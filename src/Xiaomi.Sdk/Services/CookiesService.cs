using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Xiaomi.Sdk.Services;

internal class CookiesService
{
	private readonly Dictionary<string, string?> _cookies = new();

    public void AddCookie(string name, string value)
	{
        if (_cookies.ContainsKey(name))
        {
            if (value.ToLower() == "expired")
            {
                _cookies.Remove(name);
                return;
            }
            _cookies[name] = value;
            return;
        }

        if (value.ToLower() == "expired")
        {
            return;
        }
        
        _cookies.Add(name, value);
    }

    public string? GetCookie(string name)
    {
        return _cookies.ContainsKey(name) ? _cookies[name] : null;
    }

    public void AddCookiesFrom(HttpResponseMessage response)
    {
        response.Headers.TryGetValues("Set-Cookie", out var cookiesHeader);
        var cookies = ParseCookies(cookiesHeader);
        if (cookies is null)
        {
            return;
        }

        foreach (var cookie in cookies)
        {
            AddCookie(cookie.Key, cookie.Value);
        }
    }

    public void AddCookiesToRequest(HttpRequestMessage request)
    {
        if (!_cookies.Any())
        {
            return;
        }

        request.Headers.Add("Cookie", string.Join("; ", _cookies.Select(c => $"{c.Key}={c.Value}")));
    }
    

    private static IDictionary<string, string>? ParseCookies(IEnumerable<string>? headerCookies)
    {
        if (headerCookies is null)
        {
            return null;
        }

        var cookies = new Dictionary<string, string>();
        foreach (var cookie in headerCookies)
        {
            var firstItem = cookie.Split(';')[0];
            var dividerIndex = firstItem.IndexOf('=');
            if (dividerIndex < 0)
            {
                continue;
            }
            var name = firstItem[..dividerIndex];
            var value = firstItem[(dividerIndex + 1)..];
            if (cookies.ContainsKey(name))
            {
                cookies[name] = value;
            }
            else
            {
                cookies.Add(name, value);
            }
        }
        return cookies;
    }
}
