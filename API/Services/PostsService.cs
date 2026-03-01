using API.Data;
using API.Data.Entities;
using API.Data.Models;
using API.IServices;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{


    public class PostsService(AppDbContext dbContext) : IPostsService
    {

        private readonly AppDbContext _dbContext = dbContext;

        public async Task<List<POSTS>> GetListPost()
        {
            return await _dbContext.Posts
                .Where(x => x.ISACTIVE.Equals(true))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<POSTS?> GetListPostById(int postId)
        {
            return await _dbContext.Posts
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.POST_ID == postId && p.ISACTIVE);
        }

        public async Task<List<POSTS>> GetListPostByType(string postType)
        {
            return await _dbContext.Posts
                .Where(p => p.POST_TYPE == postType && p.ISACTIVE)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<POSTS>> GetListPostByHeader(string postHeader)
        {
            return await _dbContext.Posts
                .Where(p => p.POST_HEADER == postHeader && p.ISACTIVE)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<POSTS>> GetListPostByGroup(string postGroup)
        {
            return await _dbContext.Posts
                .Where(p => p.POST_GROUP == postGroup && p.ISACTIVE)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<PostValueModels>> GetListPostTypeHeaderNull(string postType)
        {
            return await _dbContext.Posts
                .Where(p => p.ISACTIVE && p.POST_TYPE == postType && string.IsNullOrEmpty(p.POST_HEADER))
                .Select(p => new PostValueModels
                {
                    POST_NAME = p.POST_NAME,
                    POST_VALUE = p.POST_VALUE
                })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<PostValueModels>> GetListPostTypeHeader(string postType, string postHeader)
        {
            return await _dbContext.Posts
                .Where(p => p.ISACTIVE && p.POST_TYPE == postType && p.POST_HEADER == postHeader)
                .Select(p => new PostValueModels
                {
                    POST_NAME = p.POST_NAME,
                    POST_VALUE = p.POST_VALUE
                })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<string>> GetListPostType()
        {
            return await _dbContext.Posts
                .Where(p => p.ISACTIVE && p.POST_TYPE != null)
                .Select(p => p.POST_TYPE!)
                .Distinct()
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<PostTypeModels>> GetListPostGroupType()
        {
            return await _dbContext.Posts
                .Where(p => p.ISACTIVE)
                .GroupBy(p => p.POST_TYPE)
                .Select(g => new PostTypeModels
                {
                    GroupPostType = g.Key,
                    PostHelps = g.ToList()
                })
                .AsNoTracking()
                .ToListAsync();
        }

    }
}
