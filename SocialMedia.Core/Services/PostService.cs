using SocialMedia.Core.Entities;
using SocialMedia.Core.Exceptions;
using SocialMedia.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialMedia.Core.Services
{
    public class PostService : IPostService
    {
        private readonly IUnitOfWork _unit;
        //private readonly IRepository<Post> _postRepository;
        //private readonly IRepository<User> _userRepository;
        //private readonly IPostRepository _postRepository;
        //private readonly IUserRepository _userRepository;

        public PostService(IUnitOfWork unit)
        //public PostService(IRepository<Post> postRepository, IRepository<User> userRepository)
        //public PostService(IPostRepository postRepository, IUserRepository userRepository)
        {
            _unit = unit;
            //_postRepository = postRepository;
            //_userRepository = userRepository;
        }

        public async Task<Post> GetPost(int id)
        {
            return await _unit.PostRepository.GetById(id);
        }

        public IEnumerable<Post> GetPosts()
        {
            return _unit.PostRepository.GetAll();
        }

        public async Task InsertPost(Post post)
        {
            //Validar que el usuario esté registrado
            var user = await _unit.UserRepository.GetById(post.UserId);
            if(user == null)
            {
                throw new BusinessException("User doesn't exist");
            }

            //Si un usuario tiene menos de 10 posts, sólo podrá publica un post en la semana
            var userPosts = await _unit.PostRepository.GetPostsByUserId(post.UserId);
            if(userPosts.Count() < 10)
            {
                var lastPost = userPosts.OrderByDescending(x => x.Date).FirstOrDefault();
                if((DateTime.Now - lastPost.Date).TotalDays < 7)
                {
                    throw new BusinessException("You are not able to publish the post");
                }
            }

            //Validar que el contenido del post no contenga la palabra sexo
            if (post.Description.Contains("sexo"))
            {
                throw new BusinessException("Content not allowed");
            }

            await _unit.PostRepository.Add(post);
            await _unit.SaveChangesAsync();
        }

        public async Task<bool> UpdatePost(Post post)
        {
            _unit.PostRepository.Update(post);
            await _unit.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeletePost(int id)
        {
            await _unit.PostRepository.Delete(id);
            return true;
        }

    }
}
