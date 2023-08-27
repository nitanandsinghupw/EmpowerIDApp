using App.Entity.Database;
using AutoMapper;

namespace App.BlogService.MApper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Comment, Comment>().ReverseMap();           
        }
    }
}
