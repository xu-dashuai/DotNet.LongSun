﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Pharos.Framework.MVC
{
    [DataContract]
    public class OpResult
    {
        private bool _successed;
        [JsonProperty("successed")]
        [DataMember]
        public bool Successed
        {
            get { return _successed; }
            set { _successed = value; }
        }

        private string _code;

        [JsonProperty("code")]
        [DataMember]
        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }

        private string _message;

        [JsonProperty("message")]
        [DataMember]
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        private Exception _error;
        [Newtonsoft.Json.JsonIgnore]
        public Exception Error
        {
            get { return _error; }
            set { _error = value; }
        }

        private object _data;

        [JsonProperty("data")]
        [DataMember]
        public object Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public static OpResult Success(string message = "", string code = "", object data = null)
        {
            return new OpResult()
            {
                Successed = true,
                Message = message,
                Code = code,
                Data = data
            };
        }
        public static OpResult Fail(string message = "", string code = "", object data = null)
        {
            return new OpResult()
            {
                Successed = false,
                Message = message,
                Code = code,
                Data = data
            };
        }
    }
}
