namespace Bsg.EfCore.Settings.Dtos
{
    public class ContextSettingDto
    {
        public string ConnectionString { get; set; }

        public string ProviderName { get; set; }

        public bool EnableDbContextConsoleLogging { get; set; }

        public int ContextTimeout { get; set; }

        public int BulkUpdateTimeout { get; set; }

        public int BulkInsertTimeout { get; set; }
    }
}
