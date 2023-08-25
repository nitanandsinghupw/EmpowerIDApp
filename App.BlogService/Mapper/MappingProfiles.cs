using App.Entity.Service;
using AutoMapper;

namespace App.BlogService.MApper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<BlogPost, Entity.Database.BlogPost>().ReverseMap();           
        }
    }
}
