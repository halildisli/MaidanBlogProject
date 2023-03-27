using AutoMapper;
using Maidan.Areas.Admin.ViewModels;
using Maidan.Models;
using Maidan.ViewModels;

namespace Maidan.Mapping
{
    public class ViewModelMapping:Profile
    {
        public ViewModelMapping()
        {
            CreateMap<MyProfileViewModel, Author>().ReverseMap();
            CreateMap<TagViewModel, Tag>().ReverseMap();
            CreateMap<UserViewModel, Author>().ReverseMap();
            CreateMap<ArticleViewModel, Article>().ReverseMap();
        }
    }
}
