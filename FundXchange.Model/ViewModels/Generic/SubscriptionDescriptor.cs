using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FundXchange.Model.ViewModels.Charts;

namespace FundXchange.Model.ViewModels.Generic
{
    public class SubscriptionDescriptor : IEquatable<SubscriptionDescriptor>
    {
        public SubscriptionDescriptor(string symbol, string exchange, int interval, Periodicity periodicity)
        {
            Symbol = symbol;
            Exchange = exchange;
            Interval = interval;
            Periodicity = periodicity;
        }

        public string Symbol { get; private set; }
        public string Exchange { get; private set; }
        public int Interval { get; private set; }
        public Periodicity Periodicity { get; private set; }

        public static SubscriptionDescriptor From(string descriptor)
        {
            if (IsValidDescriptor(descriptor))
            {
                string[] splitDescriptor = descriptor.Split('.', ':', '_');
                string symbol = splitDescriptor[0];
                string exchange = splitDescriptor[1];
                int interval = int.Parse(splitDescriptor[2]);
                Periodicity periodicity = (Periodicity)Enum.Parse(typeof(Periodicity), splitDescriptor[3]);

                return new SubscriptionDescriptor(symbol, exchange, interval, periodicity);
            }
            return null;
        }

        public static bool IsValidDescriptor(string descriptor)
        {
            return descriptor.Contains(":") && descriptor.Contains(".") && descriptor.Contains("_");
        }

        public static implicit operator string(SubscriptionDescriptor descriptor)
        {
            return descriptor.ToString();
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}:{2}_{3}", Symbol, Exchange, Interval, Periodicity);
        }

        public bool Equals(SubscriptionDescriptor other)
        {
            return this.ToString() == other.ToString();
        }
    }
}
