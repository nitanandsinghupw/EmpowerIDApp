using App.Entity.Database;
using AutoMapper;

namespace App.BlogService.MApper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Comment, Entity.Database.Comment>().ReverseMap();           
        }
    }
}
