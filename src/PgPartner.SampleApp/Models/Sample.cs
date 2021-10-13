using NpgsqlTypes;
using System;
using System.Net;

namespace PgPartner.SampleApp.Models
{
    public class Sample
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int? ItemSum { get; set; }

        public decimal? ItemAmount { get; set; }

        public DateTime? Created { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

        public byte[] Test1 { get; set; }

        public bool Test2 { get; set; }

        public IPAddress? Test3 { get; set; }
    }
}
