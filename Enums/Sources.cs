using System.ComponentModel;

namespace ClientManager
{
    /// <summary>
    /// Sources
    /// </summary>
    public enum Sources
    {
        [Description("Другой")]
        Other,

        [Description("Знакомый")]
        Friend,

        [Description("Профи.ру")]
        Profi,

        [Description("Авито")]
        Avito
    }
}
