using System;
using System.Collections.Generic;
using Fin24.LiveData.Candlestick.Services.API.DTOs;
using Fin24.LiveData.Common.DTOs;
using Fin24.LiveData.Common.MarketModel.MarketMessages;
using Fin24.LiveData.Common.Protocol.DataFeed;

namespace FundXchange.Data.McGregorBFA
{
    public class ProtocolTypeMapper
    {
        //TODO This should go into spring or dynamicall loaded via reflection
        private static readonly IDictionary<int, Type> s_codeToType = new Dictionary<int, Type>();
        private static readonly IDictionary<Type, int> s_typeToCode = new Dictionary<Type, int>();


        //---------------------------------------------------------------------------------*---------/
        static ProtocolTypeMapper()
        {
            Add<AckMessage>(ProtoId.ACK_MESSAGE);
            Add<SocketHandshakeRequest>(ProtoId.SOCKET_HANDSHAKE_REQUEST);
            Add<StringMessage>(ProtoId.STRING_MESSAGE);
            Add<MessageWrapper>(ProtoId.MESSAGE_WRAPPER);
            Add<SocketHandshakeResponse>(ProtoId.SOCKET_HANDSHAKE_RESPONSE);

            //market messages
            Add<Announcement>(ProtoId.ANNOUNCEMENT);
            Add<ClosingPrice>(ProtoId.CLOSING_PRICE);
            Add<CumulativeNumberAndVolumeOfTrades>(ProtoId.CUMULATIVE_NUMBER_AND_VOLUME_OF_TRADES);
            Add<EnhancedBestPrice>(ProtoId.ENHANCED_BEST_PRICE);
            Add<EquityBackgroundData>(ProtoId.EQUITY_BACKGROUND_DATA);
            Add<ExMarkerStatus>(ProtoId.EX_MARKER_STATUS);
            Add<FrameworkMessage>(ProtoId.FRAMEWORK_MESSAGE);
            Add<IdlePoll>(ProtoId.IDLE_POLL);
            Add<IndexStatus>(ProtoId.INDEX_STATUS);
            Add<IndexValue>(ProtoId.INDEX_VALUE);
            Add<InstrumentTradingData>(ProtoId.INSTRUMENT_TRADING_DATA);
            Add<MarketDescription>(ProtoId.MARKET_DESCRIPTION);
            Add<MarketMessages>(ProtoId.MARKET_MESSAGES);
            Add<MarketStatusInformation>(ProtoId.MARKET_STATUS_INFORMATION);
            Add<MemberDetails>(ProtoId.MEMBER_DETAILS);
            Add<MemberInSegment>(ProtoId.MEMBER_IN_SEGMENT);
            Add<MemberRoleInInstrument>(ProtoId.MEMBER_ROLE_IN_INSTRUMENT);
            Add<NewsControl>(ProtoId.NEWS_CONTROL);
            Add<NewsServiceHeader>(ProtoId.NEWS_SERVICE_HEADER);
            Add<NewsServiceTrailer>(ProtoId.NEWS_SERVICE_TRAILER);
            Add<NewsText>(ProtoId.NEWS_TEXT);
            Add<OrderDeletion>(ProtoId.ORDER_DELETION);
            Add<OrderDetails>(ProtoId.ORDER_DETAILS);
            Add<PeriodExtension>(ProtoId.PERIOD_EXTENSION);
            Add<PeriodForMarketSector>(ProtoId.PERIOD_FORMARKET_SECTOR);
            Add<PeriodHandlingForMarketMechanismType>(ProtoId.PERIOD_HANDLING_FOR_MARKET_MECHANISM_TYPE);
            Add<PeriodHandlingForValidityType>(ProtoId.PERIOD_HANDLING_FOR_VALIDITY_TYPE);
            Add<PeriodRules>(ProtoId.PERIOD_RULES);
            Add<PeriodRulesForMarketMechanismAndValidityType>(ProtoId.PERIOD_RULES_FOR_MARKET_MECHANISM_AND_VALIDITY_TYPE);
            Add<RelatedAnnouncement>(ProtoId.RELATED_ANNOUNCEMENT);
            Add<SectorDescription>(ProtoId.SECTOR_DESCRIPTION);
            Add<SectorPeriodChange>(ProtoId.SECTOR_PERIOD_CHANGE);
            Add<SegmentDescription>(ProtoId.SEGMENTDESCRIPTION);
            Add<SettlementVenueForTradableInstrument>(ProtoId.SETTLEMENT_VENUE_FOR_TRADABLE_INSTRUMENT);
            Add<SettlementVenues>(ProtoId.SETTLEMENT_VENUES);
            Add<TemporaryPeriodForATradableInstrumentOrCurrency>(ProtoId.TEMPORARY_PERIOD_FOR_A_TRADABLE_INSTRUMENT_OR_CURRENCY);
            Add<TickSizeMatrixForSegmentOrCurrency>(ProtoId.TICK_SIZE_MATRIX_FOR_SEGMENT_OR_CURRENCY);
            Add<TradableInstrumentControl>(ProtoId.TRADABLE_INSTRUMENT_CONTROL);
            Add<TradableInstrumentOrCurrencyPeriodChange>(ProtoId.TRADABLE_INSTRUMENT_OR_CURRENCY_PERIOD_CHANGE);
            Add<TradeReport>(ProtoId.TRADE_REPORT);
            Add<TradeTypePerSegment>(ProtoId.TRADE_TYPE_PER_SEGMENT);
            Add<UncrossingCompleted>(ProtoId.UNCROSSING_COMPLETED);
            Add<UncrossingPriceAndVolume>(ProtoId.UNCROSSING_PRICE_AND_VOLUME);
            Add<VWAPFromTrades>(ProtoId.VWAP_FROM_TRADES);
            Add<AddSubscriptionCommand>(ProtoId.ADD_SUBSCRIPTION_COMMAND);
            Add<DepthItemDto>(ProtoId.DEPTH_ITEM_DTO);
            Add<OrderbookDto>(ProtoId.ORDER_BOOK_DTO);
            Add<OrderDto>(ProtoId.ORDER_DTO);
            Add<ExceptionMessage>(ProtoId.EXCEPTION_MESSAGE);


            Add<EquityDto>(ProtoId.EQUITY_DTO);
            Add<EquityDtoCollection>(ProtoId.EQUITY_DTO_COLLECTION);
            Add<IndexDto>(ProtoId.INDEX_DTO);
            Add<IndexDtoCollection>(ProtoId.INDEX_DTO_COLLECTION);
            Add<EquityCandlestickCollection>(ProtoId.EQUITY_CANDLESTICK_COLLECTION);
            Add<EquityDetailsDto>(ProtoId.EQUITY_DETAILS_DTO);
            Add<EquityDetailsDtoCollection>(ProtoId.EQUITY_DETAILS_DTO_COLLECTION);
            Add<IndexCandlestickCollection>(ProtoId.INDEX_CANDLESTICK_COLLECTION);
            Add<IndexValueDto>(ProtoId.INDEX_VALUE_DTO);
            Add<IndexValueDtoCollection>(ProtoId.INDEX_VALUE_DTO_COLLECTION);
            Add<QuoteDto>(ProtoId.QUOTE_DTO);
            Add<TradeDto>(ProtoId.TRADE_DTO);
            Add<TradeDtoCollection>(ProtoId.TRADE_DTO_COLLECTION);
            Add<EquityCandlestick>(ProtoId.EQUITY_CANDLESTICK);
            Add<IndexCandlestick>(ProtoId.INDEX_CANDLESTICK);

            Add<RemoveSubscriptionCommand>(ProtoId.REMOVE_SUBSCRIPTION_COMMAND);
            Add<HeartbeatMessage>(ProtoId.HEARTBEAT_MESSAGE);
            Add<JSERawMessage>(ProtoId.JSE_RAW_MESSAGE);

            Add<CandlestickDto>(ProtoId.CANDLESTICK_DTO);
            Add<CandlestickCollectionDto>(ProtoId.CANDLESTICK_DTO_COLLECTION);
        }

        //---------------------------------------------------------------------------------*---------/
        private static void Add<T>(int id)
        {
            s_codeToType.Add(id, typeof(T));
            s_typeToCode.Add(typeof(T), id);
        }

        //---------------------------------------------------------------------------------*---------/
        public static Type GetTypeForCode(int code)
        {
            Type theType;
            if (!s_codeToType.TryGetValue(code, out theType))
            {
                return null;
            }

            return theType;
        }

        //---------------------------------------------------------------------------------*---------/
        public static int GetCodeForType(Type theType)
        {
            return s_typeToCode[theType];
        }



    }
}