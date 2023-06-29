﻿namespace AccountingAPI.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string objectName) : base($"{objectName} not found")
        {
        }
    }

}
