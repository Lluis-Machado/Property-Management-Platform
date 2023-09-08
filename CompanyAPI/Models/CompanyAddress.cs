using MassTransit.Internals.GraphValidation;
using MongoDB.Driver;
using System.Buffers.Text;
using System.Numerics;
using System.Runtime.Intrinsics.X86;

namespace CompanyAPI.Models
{
    public class CompanyAddress
    {
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public int? State { get; set; }
        public string? PostalCode { get; set; }
        public int? Country { get; set; }
        public int? AddressType { get; set; }
        public string? ShortComment { get; set; }
               
    }
       
}
