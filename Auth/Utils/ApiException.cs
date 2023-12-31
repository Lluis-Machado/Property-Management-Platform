﻿using System.Net;

namespace Authentication.Utils
{
    public class ApiException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public string ErrorMessage { get; }

        public ApiException(HttpStatusCode statusCode, string errorMessage) : base(errorMessage)
        {
            StatusCode = statusCode;
            ErrorMessage = errorMessage;
        }
    }
}
