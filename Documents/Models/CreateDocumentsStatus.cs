﻿using System.Net;

namespace Documents.Models
{

    public class CreateDocumentStatus
    {
        public string FileName { get; set; }
        public HttpStatusCode Status { get; set; }

        public CreateDocumentStatus(string fileName, HttpStatusCode status)
        {
            FileName = fileName;
            Status = status;
        }
    }
}