using App.Entity.Database;
using AutoMapper;
using BlogAPI;

namespace App.BlogService.MApper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<BlogPostModel, BlogPost>().ReverseMap();
            
        }
    }
}
