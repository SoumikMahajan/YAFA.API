using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Yafa.Application.Common.Interfaces;
using Yafa.Domain.Dtos;
using Yafa.Infrastructure.Data;

namespace Yafa.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;
        private readonly ILogService _logService;
        private IDbContextTransaction? _transaction;
        private readonly IConfiguration _configuration;
        public bool HasActiveTransaction => _transaction != null;

        #region :: Dapper
        public IDapperRepo DapperRepo { get; private set; }
        #endregion

        #region:: Email Service
        public IEmailRepo EmailRepo { get; private set; }

        #endregion

        public IInvoicePdfService InvoicePdfService { get; private set; }

		public UnitOfWork(ApplicationDbContext db, ILogService logService, IConfiguration configuration, IOptions<LocalMailSettings> localMailSettings, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _logService = logService;
            _configuration = configuration;

            #region :: Dapper
            DapperRepo = new DapperService(_db, _logService, _configuration);
            #endregion

            #region :: Email Service
            EmailRepo = new EmailService(_logService, localMailSettings, httpContextAccessor);
			#endregion

			InvoicePdfService = new InvoicePdfService();

		}


        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            if (_transaction == null)
            {
                _transaction = await _db.Database.BeginTransactionAsync();
            }
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }
}
