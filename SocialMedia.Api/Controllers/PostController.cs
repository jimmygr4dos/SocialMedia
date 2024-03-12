using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Api.Responses;
using SocialMedia.Core.DTOs;
using SocialMedia.Core.Entities;
using SocialMedia.Core.Interfaces;
using SocialMedia.Core.QueryFilters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace SocialMedia.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        //private readonly IPostRepository _postService;
        private readonly IPostService _postService;
        private readonly IMapper _mapper;
        public PostController(IPostService postService, IMapper mapper)
        {
            _postService = postService;
            _mapper = mapper;
        }

        //[HttpGet]
        //public IActionResult GetPost()
        //{
        //    var posts = new PostRepository().GetPosts();
        //    return Ok(posts);
        //}

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        //public IActionResult GetPosts(int? userId, DateTime? date, string description)
        public IActionResult GetPosts([FromQuery]PostQueryFilter filters)
        {
            var posts = _postService.GetPosts(filters);
            //var postsDTO = posts.Select(x => new PostDTO
            //{
            //    PostId = x.PostId,
            //    Description = x.Description,
            //    Date = x.Date,
            //    Image = x.Image,
            //    UserId = x.UserId
            //});
            var postsDTOs = _mapper.Map<IEnumerable<PostDTO>>(posts);
            var response = new ApiResponse<IEnumerable<PostDTO>>(postsDTOs);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPost(int id)
        {
            var post = await _postService.GetPost(id);
            //var postDTO = new PostDTO
            //{
            //    PostId = post.PostId,
            //    Description = post.Description,
            //    Date = post.Date,
            //    Image = post.Image,
            //    UserId = post.UserId
            //};
            var postDTO = _mapper.Map<PostDTO>(post);
            var response = new ApiResponse<PostDTO>(postDTO);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Post(PostDTO postDTO)
        {
            //var post = new Post
            //{
            //    Description = postDTO.Description,
            //    Date = postDTO.Date,
            //    Image = postDTO.Image,
            //    UserId = postDTO.UserId
            //};
            var post = _mapper.Map<Post>(postDTO);

            await _postService.InsertPost(post);
            
            postDTO = _mapper.Map<PostDTO>(post);
            var response = new ApiResponse<PostDTO>(postDTO);
            return Ok(response);
        }

        [HttpPut]
        public async Task<IActionResult> Put(int id, PostDTO postDTO)
        {
            var post = _mapper.Map<Post>(postDTO);
            //post.PostId = id;
            post.Id = id;

            var result = await _postService.UpdatePost(post);
            var response = new ApiResponse<bool>(result);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _postService.DeletePost(id);
            var response = new ApiResponse<bool>(result);
            return Ok(response);
        }

    }
}
