using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MarketplaceSystem.Common.Extensions
{
    public static class EnumExtension
    {
        public static string GetDisplayName(this Enum value)
        {
            return value
                .GetType()
                .GetMember(value.ToString())
                .FirstOrDefault()?
                .GetCustomAttribute<DisplayAttribute>()?
                .Name
                ?? value.ToString();
        }
    }
}
