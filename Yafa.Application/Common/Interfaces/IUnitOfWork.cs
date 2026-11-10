namespace Yafa.Application.Common.Interfaces
{
    public interface IUnitOfWork
    {
        #region :: Dapper
        IDapperRepo DapperRepo { get; }
        #endregion

        #region:: Mail Service
        IEmailRepo EmailRepo { get; }
        #endregion

        IInvoicePdfService InvoicePdfService { get; }

		Task SaveAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        bool HasActiveTransaction { get; }
    }
}
