using System;

namespace PgPartner.SampleApp.Models
{
    public class Sample
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int? ItemSum { get; set; }

        public decimal? ItemAmount { get; set;  }
    }
}
