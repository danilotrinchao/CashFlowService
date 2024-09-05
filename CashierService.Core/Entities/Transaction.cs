using CashierService.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashierService.Core.Entities
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public ETransactionType eTransactionType { get; set; }
        public DateTime Date { get; set; }
    }
}
