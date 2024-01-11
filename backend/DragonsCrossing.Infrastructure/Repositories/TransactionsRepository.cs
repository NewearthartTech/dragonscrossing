using DragonsCrossing.Core.Contracts.Infrastructure;
using DragonsCrossing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Infrastructure.Repositories
{
    public class TransactionsRepository : ITransactionsRepository
    {
        private readonly ApplicationDbContext dbContext;
        private IDbContextTransaction? dbTransaction;

        public TransactionsRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task BeginTransaction()
        {
            dbTransaction = await dbContext.Database.BeginTransactionAsync();
        }

        public async Task CommitTransaction()
        {
            if (dbTransaction == null)
                throw new InvalidOperationException("Cannot commit a null transaction. Call BeginTransaction() first.");

            try
            {
                await dbContext.Database.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await RollbackTransaction();
                throw new Exception("An error occurred committing the transaction. Transaction was rolled back.", ex);
            }
            finally
            {
                await dbTransaction.DisposeAsync();
            }
        }

        public async Task RollbackTransaction()
        {
            if (dbTransaction == null)
                throw new InvalidOperationException("Cannot rollback a null transaction. Call BeginTransaction() first.");

            await dbContext.Database.RollbackTransactionAsync();
            await dbTransaction.DisposeAsync();
            dbTransaction = null;
        }
    }
}
