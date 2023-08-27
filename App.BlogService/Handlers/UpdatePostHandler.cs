﻿using App.BlogService.Commands;
using App.BlogService.Repositories;
using App.Entity;
using MediatR;

namespace App.BlogService.Handlers
{
    public class UpdatePostHandler : IRequestHandler<UpdatePostCommand, Entity.ApiResponse<string>>
    {
        private readonly IPostRepository _postRepository;
        public UpdatePostHandler(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }
        public async Task<Entity.ApiResponse<string>> Handle(UpdatePostCommand command, CancellationToken cancellationToken)
        {
            var result = await _postRepository.UpdatePostAsync(command.Id, command.Post);
            if (result == null)
            {
                return new ApiResponse<string> { Success = true, Message = "Post updated successfully" };
            }
            return new ApiResponse<string> { Success = false, Message = "Post not found" };          
        }
    }
}
