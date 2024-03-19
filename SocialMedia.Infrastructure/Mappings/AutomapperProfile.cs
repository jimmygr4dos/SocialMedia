using AutoMapper;
using SocialMedia.Core.DTOs;
using SocialMedia.Core.Entities;

namespace SocialMedia.Infrastructure.Mappings
{
    public class AutomapperProfile: Profile
    {
        public AutomapperProfile()
        {
            CreateMap<Post, PostDTO>();
            CreateMap<PostDTO, Post>();

            CreateMap<User, UserDTO>();
            CreateMap<UserDTO, User>();

            CreateMap<Comment, CommentDTO>();
            CreateMap<CommentDTO, Comment>();

            //ReverseMap permite registrar el mapping inverso en una sola línea de código.
            //CreateMap<Security, SecurityDTO>();
            //CreateMap<SecurityDTO, Security>();
            CreateMap<Security, SecurityDTO>().ReverseMap();
        }
    }
}
