using AutoMapper;
using PCM.Application.DTOs.Auth;
using PCM.Application.DTOs.Bookings;
using PCM.Application.DTOs.Courts;
using PCM.Application.DTOs.Members;
using PCM.Application.DTOs.News;
using PCM.Application.DTOs.Tournaments;
using PCM.Application.DTOs.Transactions;
using PCM.Application.DTOs.Wallet;
using PCM.Domain.Entities;

namespace PCM.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Member mappings
        CreateMap<Member, MemberDto>();
        CreateMap<MemberCreateDto, Member>();
        CreateMap<MemberUpdateDto, Member>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Court mappings
        CreateMap<Court, CourtDto>();
        CreateMap<CourtCreateDto, Court>();
        CreateMap<CourtUpdateDto, Court>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Booking mappings
        CreateMap<Booking, BookingDto>()
            .ForMember(dest => dest.CourtName, opt => opt.MapFrom(src => src.Court != null ? src.Court.Name : string.Empty))
            .ForMember(dest => dest.MemberName, opt => opt.MapFrom(src => src.Member != null ? src.Member.FullName : string.Empty));
        CreateMap<BookingCreateDto, Booking>();

        // News mappings
        CreateMap<News, NewsDto>();
        CreateMap<NewsCreateDto, News>();
        CreateMap<NewsUpdateDto, News>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Tournament mappings
        CreateMap<Tournament, TournamentDto>()
            .ForMember(dest => dest.CreatorName, opt => opt.MapFrom(src => src.Creator.FullName))
            .ForMember(dest => dest.ParticipantCount, opt => opt.MapFrom(src => src.Participants.Count))
            .ForMember(dest => dest.ConfigTargetWins, opt => opt.MapFrom(src => src.Config_TargetWins))
            .ForMember(dest => dest.CurrentScoreTeamA, opt => opt.MapFrom(src => src.CurrentScore_TeamA))
            .ForMember(dest => dest.CurrentScoreTeamB, opt => opt.MapFrom(src => src.CurrentScore_TeamB));
        CreateMap<TournamentCreateDto, Tournament>();
        CreateMap<TournamentUpdateDto, Tournament>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Participant mappings
        CreateMap<Participant, ParticipantDto>()
            .ForMember(dest => dest.MemberName, opt => opt.MapFrom(src => src.Member.FullName));

        // Transaction mappings
        CreateMap<Transaction, TransactionDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.CategoryType, opt => opt.MapFrom(src => src.Category.Type))
            .ForMember(dest => dest.CreatorName, opt => opt.MapFrom(src => src.Creator != null ? src.Creator.FullName : null));
        CreateMap<TransactionCreateDto, Transaction>();

        CreateMap<TransactionCategory, TransactionCategoryDto>();
        CreateMap<TransactionCategoryCreateDto, TransactionCategory>();

        // Wallet Transaction mappings
        CreateMap<WalletTransaction, WalletTransactionDto>()
            .ForMember(dest => dest.MemberName, opt => opt.MapFrom(src => src.Member.FullName))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
    }
}
