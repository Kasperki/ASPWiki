using ASPWiki.Model;
using ASPWiki.ViewModels;
using AutoMapper;
using System.Linq;

namespace ASPWiki.Mapping
{
    public class AutoMapperConfiguration
    {
        public static void Configure()
        {
            ConfigureMappings();
            Mapper.Configuration.AssertConfigurationIsValid();
        }

        private static void ConfigureMappings()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<WikiPage, WikipageSummary>()
                     .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                     .ForMember(dest => dest.Path, opt => opt.MapFrom(src => src.Path))
                     .ForMember(dest => dest.Label, opt => opt.MapFrom(src => src.Label))
                     .ForMember(dest => dest.ContentSummary, opt => opt.MapFrom(src => src.GetContentSummary()))
                     .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author))
                     .ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.GetSizeKiloBytes()))
                     .ForMember(dest => dest.AttachmentCount, opt => opt.MapFrom(src => src.Attachments.Count()))
                     .ForMember(dest => dest.LastModified, opt => opt.MapFrom(src => src.LastModified))
                     .ForMember(dest => dest.Public, opt => opt.MapFrom(src => src.Public))
                     .ReverseMap();

                cfg.CreateMap<WikiPage, WikipageView>()
                     .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                     .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                     .ForMember(dest => dest.Path, opt => opt.MapFrom(src => src.Path))
                     .ForMember(dest => dest.Label, opt => opt.MapFrom(src => src.Label))
                     .ForMember(dest => dest.Attachments, opt => opt.MapFrom(src => src.Attachments))
                     .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author))
                     .ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.GetSizeKiloBytes()))
                     .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                     .ForMember(dest => dest.ContentHistory, opt => opt.MapFrom(src => src.ContentHistory))
                     .ForMember(dest => dest.LastModified, opt => opt.MapFrom(src => src.LastModified))
                     .ReverseMap();

                cfg.CreateMap<WikiPage, WikipageEdit>()
                     .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                     .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                     .ForMember(dest => dest.Path, opt => opt.MapFrom(src => src.Path))
                     .ForMember(dest => dest.Label, opt => opt.MapFrom(src => src.Label))
                     .ForMember(dest => dest.PathArray, opt => opt.MapFrom(src => src.PathArray))
                     .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                     .ForMember(dest => dest.Attachments, opt => opt.MapFrom(src => src.Attachments))
                     .ForMember(dest => dest.Public, opt => opt.MapFrom(src => src.Public))
                     .ForMember(dest => dest.PathToParent, opt => opt.MapFrom(src => src.GetPathToParent()));

                cfg.CreateMap<WikiPage, WikiPageMetadata>()
                     .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                     .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                     .ForMember(dest => dest.Path, opt => opt.MapFrom(src => src.Path))
                     .ReverseMap();

                cfg.CreateMap<WikipageEdit, WikiPage>()
                     .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                     .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                     .ForMember(dest => dest.Path, opt => opt.MapFrom(src => src.Path))
                     .ForMember(dest => dest.Label, opt => opt.MapFrom(src => src.Label))
                     .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                     .ForMember(dest => dest.Attachments, opt => opt.MapFrom(src => src.Attachments))
                     .ForMember(dest => dest.Public, opt => opt.MapFrom(src => src.Public))
                     .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                     .ForMember(dest => dest.DueDate, opt => opt.Ignore())
                     .ForMember(dest => dest.Author, opt => opt.Ignore())
                     .ForMember(dest => dest.Visits, opt => opt.Ignore())
                     .ForMember(dest => dest.ContentHistory, opt => opt.Ignore())
                     .ForMember(dest => dest.LastModified, opt => opt.Ignore())
                     .ForMember(dest => dest.PathArray, opt => opt.Ignore());

                cfg.CreateMap<Node, NodeJsonModel>();
                cfg.CreateMap<Node, NodeListModel>();
            });
        }
    }
}
