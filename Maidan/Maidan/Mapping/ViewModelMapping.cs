using AutoMapper;
using Maidan.Areas.Admin.Models.ViewModels;
using Maidan.Models;
using Maidan.Models.ViewModels;

namespace Maidan.Mapping
{
    public class ViewModelMapping:Profile
    {
        public ViewModelMapping()
        {
            CreateMap<MyProfileViewModel, Author>().ReverseMap();
            CreateMap<TagViewModel, Tag>().ReverseMap();
            CreateMap<UserViewModel, Author>().ReverseMap();
        }
    }
}
