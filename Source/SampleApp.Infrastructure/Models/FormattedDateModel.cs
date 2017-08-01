using System;

namespace SampleApp.Infrastructure.Models
{
    public class FormattedDateModel
    {
        public DateTime Date { get; set; }
        public string Language { get; set; }
        public string FormattedDate { get; set; }
    }
}