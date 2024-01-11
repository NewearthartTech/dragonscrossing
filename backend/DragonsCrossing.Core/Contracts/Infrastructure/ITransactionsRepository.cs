using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Infrastructure
{
    public interface ITransactionsRepository
    {
        public Task BeginTransaction();
        public Task CommitTransaction();
        public Task RollbackTransaction();
    }
}
