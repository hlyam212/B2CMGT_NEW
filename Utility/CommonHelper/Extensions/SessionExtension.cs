using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using static CommonHelper.JsonExtensions;

namespace CommonHelper
{
    public static class SessionExtensions
    {
        public static void SetObject(this ISession session, string key, object value)
        {
            session.SetString(key, value.Serialize());
            //session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);

            return value == null ? default(T) : value.Deserialize<T>();
            //JsonSerializer.Deserialize<SessionObj>(_Session.GetString("SessionObj"))
        }
    }
}
