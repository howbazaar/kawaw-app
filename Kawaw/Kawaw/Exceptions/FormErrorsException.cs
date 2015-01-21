using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Kawaw.Exceptions
{
    public class FormErrorsException : Exception
    {
        private readonly string _message;

        public FormErrorsException(JObject parsed, bool fieldPrefix = false)
        {
            var errors = new List<string>();

            foreach (var field in parsed)
            {
                var prefix = "";
                if (field.Key != "__all__" && fieldPrefix)
                {
                    prefix = field.Key + ": ";
                }
                var values = field.Value.Select(m => m.ToObject<string>()).ToList();
                var msg = string.Join(", ", values);
                errors.Add(prefix + msg);
            }
            _message = string.Join("\n", errors);
        }

        public override string Message { get { return _message; }}
    }
}