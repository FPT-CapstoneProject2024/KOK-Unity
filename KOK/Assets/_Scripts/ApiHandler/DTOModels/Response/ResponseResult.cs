using System;

namespace KOK.ApiHandler.DTOModels
{
    [Serializable]
    public class ResponseResult<T>
    {
        public string? Message { get; set; }
        public T? Value { get; set; }
        public bool? Result { get; set; }
    }
}
