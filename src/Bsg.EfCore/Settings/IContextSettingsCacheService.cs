namespace Bsg.EfCore.Settings
{
    using Context;
    using Dtos;

    public interface IContextSettingsCacheService
    {
        int ContextTimeout<TContext>()
            where TContext : IDbContext;

        int BulkInsertTimeout<TContext>()
            where TContext : IDbContext;

        int BulkUpdateTimeout<TContext>()
            where TContext : IDbContext;

        string ConnectionString<TContext>()
            where TContext : IDbContext;

        string Provider<TContext>()
            where TContext : IDbContext;

        bool EnableDbContextConsoleLogging<TContext>()
            where TContext : IDbContext;

        void StoreSettings<TContext>(ContextSettingDto settings)
            where TContext : IDbContext;
    }
}