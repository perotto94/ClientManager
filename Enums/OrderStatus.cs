using System.ComponentModel;

namespace ClientManager
{
    /// <summary>
    /// Order status
    /// </summary>
    public enum OrderStatus
    {
        [Description("Не выбран")]
        None,

        [Description("Заморожен")]
        Freeze,

        [Description("В процессе")]
        InProgress,

        [Description("Отменен")]
        Canceled,

        [Description("Ожидает оплаты")]
        AwaitForPayment,

        [Description("Завершен")]
        Completed
    }
}
