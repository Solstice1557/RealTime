using AutoMapper;
using RealTime.BL.Brokers;
using RealTime.BL.Tdameritrade.Models;
using System;
using System.Linq;

namespace RealTime.BL.Tdameritrade.Mappers
{
    public class TdAmeritradeMappingProfile : Profile
    {
        public TdAmeritradeMappingProfile()
        {
            CreateMap<TdAmeritradeOrdersStrategy, BrokerOrder>()
                .ForMember(x => x.Id, y => y.MapFrom(u => u.OrderId))
                .ForMember(x => x.Type, y => y.MapFrom(u => GetOrderTypeFromInstruction(u)))
                .ForMember(x => x.Symbol, y => y.MapFrom(u => GetOrderSymbol(u)))
                .ForMember(x => x.Amount, y => y.MapFrom(u => GetOrderQuantity(u)))
                .ForMember(x => x.Status, y => y.MapFrom(u => GetOrderStatus(u)))
                .ForMember(x => x.Price, y => y.MapFrom(u => GetOrderPrice(u)))
                .ForMember(x => x.ExecutionTime, y => y.MapFrom(u => GetExecutionTime(u)))
                ;

            CreateMap<TdAmeritradeAccount, BrokerAccount>()
                .ForMember(x => x.AccountName, y => y.MapFrom(u => $"{u.AccountId}:{u.Type}"))
                .ForMember(x => x.Fund, y => y.MapFrom(u => u.CurrentBalances.BuyingPower))
                ;

            CreateMap<TdAmeritradePosition, BrokerPosition>()
                .ForMember(x => x.Symbol, y => y.MapFrom(u => u.Instrument.Symbol))
                .ForMember(x => x.Amount, y => y.MapFrom(u => u.LongQuantity))
                .ForMember(x => x.CostBasis, y => y.MapFrom(u => u.AveragePrice))
                ;

            CreateMap<AccessTokenResponse, BrokerAuthResponse>()
                .ForMember(x => x.ErrorMessage, y => y.MapFrom(u => u.Error))
                .ForMember(x => x.ExpiresInSeconds, y => y.MapFrom(u => u.ExpiresIn))
                .ForMember(x => x.RefreshTokenExpiresInSeconds, y => y.MapFrom(u => u.RefreshTokenExpiresIn))
                .ForMember(x => x.Status, y => y.MapFrom(u => string.IsNullOrWhiteSpace(u.Error) ? BrokerAuthStatus.Succeeded : BrokerAuthStatus.Failed))
                ;
        }

        private static DateTime? GetExecutionTime(TdAmeritradeOrdersStrategy u)
            => u.CloseTime ?? u.ReleaseTime ?? u.EnteredTime;

        private static decimal? GetOrderPrice(TdAmeritradeOrdersStrategy order)
            => order.OrderActivityCollection?.FirstOrDefault()?.ExecutionLegs?.FirstOrDefault()?.Price;

        private static BrokerOrderType GetOrderTypeFromInstruction(TdAmeritradeOrdersStrategy order) 
        {
            var instruction = order.OrderLegCollection?.FirstOrDefault()?.Instruction;

            if (!instruction.HasValue) 
            {
                return BrokerOrderType.Unknown;
            }

            return instruction.Value switch
            {
                TdAmeritradeInstructionEnum.BUY => BrokerOrderType.Buy,
                TdAmeritradeInstructionEnum.SELL => BrokerOrderType.Sell,
                _ => BrokerOrderType.Unknown,
            };
        }

        private static string GetOrderSymbol(TdAmeritradeOrdersStrategy order)
        {
            var instrument = order.OrderLegCollection?.FirstOrDefault()?.Instrument;

            if (instrument == null)
            {
                return "";
            }

            return instrument.Symbol;
        }

        private static decimal GetOrderQuantity(TdAmeritradeOrdersStrategy order)
        {
            var orderLeg = order.OrderLegCollection?.FirstOrDefault();

            if (orderLeg == null)
            {
                return 0m;
            }

            return orderLeg.Quantity ?? 0;
        }

        private static BrokerOrderStatus GetOrderStatus(TdAmeritradeOrdersStrategy order)
        {
            return order.Status switch
            {
                TdAmeritradeOrderStatusEnum.CANCELED => BrokerOrderStatus.Cancelled,
                TdAmeritradeOrderStatusEnum.FILLED => BrokerOrderStatus.Success,
                TdAmeritradeOrderStatusEnum.EXPIRED => BrokerOrderStatus.Failed,
                TdAmeritradeOrderStatusEnum.REJECTED => BrokerOrderStatus.Failed,
                _ => BrokerOrderStatus.InProgress
            };
        }
    }
}
