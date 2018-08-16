namespace Bsg.EfCore.SupportType.Dtos
{
    using System;
    using System.Collections.Generic;

    public class ContextSupportTypeDto
    {
        public ContextSupportTypeDto()
        {
            this.WrapperEntityTypes = new List<Type>();
            this.ConfigTypes = new List<ConfigSupportDto>();
        }

        public IList<ConfigSupportDto> ConfigTypes { get; set; }

        public IList<Type> WrapperEntityTypes { get; set; }
    }
}
