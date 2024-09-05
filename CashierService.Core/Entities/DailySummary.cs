using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashierService.Core.Entities
{
    public class DailySummary
    {
        public DateTime Date { get; set; }
        public decimal TotalCredits { get; set; }
        public decimal TotalDebits { get; set; }
        public decimal NetAmount => TotalCredits - TotalDebits;
    }
}
