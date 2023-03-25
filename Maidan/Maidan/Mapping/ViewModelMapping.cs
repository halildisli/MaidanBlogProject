using AutoMapper;
using Maidan.Models;
using Maidan.Models.ViewModels;

namespace Maidan.Mapping
{
    public class ViewModelMapping:Profile
    {
        public ViewModelMapping()
        {
            CreateMap<MyProfileViewModel, Author>().ReverseMap();
        }
    }
}
