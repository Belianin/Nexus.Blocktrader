using System;
using Newtonsoft.Json;

namespace Nexus.Core
{
    public static class StringExtensions
    {
        public static Result<T> TryDeserialize<T>(this string value)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}