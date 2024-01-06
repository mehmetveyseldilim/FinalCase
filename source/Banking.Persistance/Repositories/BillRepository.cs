using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Banking.Persistance.Contracts;
using Banking.Persistance.Entities;
using Microsoft.Extensions.Logging;

namespace Banking.Persistance.Repositories
{
    public class BillRepository : IBillRepository
    {
        private readonly ILogger<BillRepository> _logger;

        private readonly BankingDbContext _context;

        private readonly string className = nameof(BillRepository);

        public BillRepository(BankingDbContext context, ILogger<BillRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void CreatBill(Bill bill)
        {

            _logger.LogDebug("In {@className} {@methoName} method", className, nameof(CreatBill));
            _context.Bills.Add(bill);
        }
    }
}