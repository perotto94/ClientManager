using System.ComponentModel;

namespace ClientManager
{
    /// <summary>
    /// Exercise status
    /// </summary>
    public enum ExerciseStatus
    {
        [Description("Не выбран")]
        None,

        [Description("Запланировано")]
        Planned,

        [Description("Отменено")]
        Canceled,

        [Description("Ожидает оплаты")]
        AwaitForPayment,

        [Description("Завершено")]
        Completed
    }
}
