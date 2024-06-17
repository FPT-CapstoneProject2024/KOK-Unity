using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KOK.ApiHandler.Model
{
    [Serializable]
    public class ResponseObject<T>
    {
        public string Message { get; set; }

        public T Value { get; set; }
        public bool Result { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
