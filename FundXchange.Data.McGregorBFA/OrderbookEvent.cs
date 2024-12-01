namespace FundXchange.Data.McGregorBFA
{
    internal class OrderbookEvent
    {
        internal enum OrderbookType
        {
            OrderInitialize,
            OrderAdd,
            OrderDelete
        }

        internal OrderbookEvent(OrderbookType type, object message)
        {
            Type = type;
            Message = message;
        }

        internal OrderbookType Type { get; set; }
        internal object Message { get; set; }
    }
}