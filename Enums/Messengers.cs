using System.ComponentModel;

namespace ClientManager
{
    /// <summary>
    /// Messengers
    /// </summary>
    public enum Messengers
    {
        [Description("Другой")]
        Other,

        [Description("Профи.ру")]
        Profi,

        [Description("Авито")]
        Avito,

        [Description("WhatsApp")]
        WhatsApp,

        [Description("Telegram")]
        Telegram
    }
}
