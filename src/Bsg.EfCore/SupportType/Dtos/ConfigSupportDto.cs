namespace Bsg.EfCore.SupportType.Dtos
{
    using System;
    using System.Reflection;

    public class ConfigSupportDto
    {
        public Type EntityConfigurationType { get; set; }

        public MethodInfo GenerifiedModelApplyMethod { get; set; }
    }
}
