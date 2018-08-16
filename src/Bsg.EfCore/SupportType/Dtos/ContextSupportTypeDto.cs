namespace Bsg.EfCore.SupportType.Dtos
{
    using System;
    using System.Collections.Generic;

    public class ContextSupportTypeDto
    {
        public ContextSupportTypeDto()
        {
            this.ConfigTypes = new List<ConfigSupportDto>();
        }

        public IList<ConfigSupportDto> ConfigTypes { get; set; }
    }
}
