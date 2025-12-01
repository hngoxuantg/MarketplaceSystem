namespace MarketplaceSystem.Web.UI.Extensions
{
    public static class ToDictionary
    {
        public static Dictionary<string, string> ToDictionaryString(this object obj)
        {
            return obj.GetType()
                      .GetProperties()
                      .Where(prop => prop.GetValue(obj, null) != null)
                      .ToDictionary(prop => prop.Name, prop => prop.GetValue(obj, null)?.ToString() ?? string.Empty);
        }
    }
}
