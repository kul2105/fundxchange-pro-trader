using System;
using System.Collections.Generic;
using FundXchange.Domain.ValueObjects;
using FundXchange.Domain.Entities;

namespace FundXchange.Morningstar
{
    public class Repository
    {
        private Dictionary<string, Quote> _Quotes;
        private Dictionary<string, Instrument> _Instruments;

        public Repository()
        {
            _Quotes = new Dictionary<string, Quote>();
            _Instruments = new Dictionary<string, Instrument>();
        }

        public Quote GetQuote(string symbol)
        {
            if (_Quotes.ContainsKey(symbol))
            {
                return _Quotes[symbol];
            }
            return null;
        }

        public void UpdateQuote(Quote quote)
        {
            if (_Quotes.ContainsKey(quote.Symbol))
            {
                _Quotes[quote.Symbol] = quote;
            }
            else
            {
                _Quotes.Add(quote.Symbol, quote);
            }
        }

        public Instrument GetInstrument(string symbol)
        {
            if (_Instruments.ContainsKey(symbol))
            {
                return _Instruments[symbol];
            }
            return null;
        }

        public void UpdateInstrument(Instrument instrument)
        {
            if (_Instruments.ContainsKey(instrument.Symbol))
            {
                _Instruments[instrument.Symbol] = instrument;
            }
            else
            {
                _Instruments.Add(instrument.Symbol, instrument);
            }
        }
    }
}
