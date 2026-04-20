using API.Data;
using API.Data.Entities;
using API.Data.Models;
using API.IServices;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{


    public class PostsService(AppDbContext DBContext) : IPostsService
    {

        private readonly AppDbContext _dbContext = DBContext;

        public async Task<IReadOnlyList<POSTS>> GetListPost(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Posts
                .Where(x => x.ISACTIVE.Equals(true))
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<POSTS?> GetListPostById(int PostId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Posts
                .Where(p => p.POST_ID == PostId && p.ISACTIVE)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<POSTS>> GetListPostByType(string PostType, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Posts
                .Where(p => p.POST_TYPE == PostType && p.ISACTIVE)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<POSTS>> GetListPostByHeader(string PostHeader, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Posts
                .Where(p => p.POST_HEADER == PostHeader && p.ISACTIVE)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<POSTS>> GetListPostByGroup(string PostGroup, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Posts
                .Where(p => p.POST_GROUP == PostGroup && p.ISACTIVE)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<PostValueModels>> GetListPostTypeHeaderNull(string PostType, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Posts
                .Where(p => p.ISACTIVE && p.POST_TYPE == PostType && string.IsNullOrEmpty(p.POST_HEADER))
                .Select(p => new PostValueModels
                {
                    POST_NAME = p.POST_NAME,
                    POST_VALUE = p.POST_VALUE
                })
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<PostValueModels>> GetListPostTypeHeader(string PostType, string PostHeader, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Posts
                .Where(p => p.ISACTIVE && p.POST_TYPE == PostType && p.POST_HEADER == PostHeader)
                .Select(p => new PostValueModels
                {
                    POST_NAME = p.POST_NAME,
                    POST_VALUE = p.POST_VALUE
                })
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<string>> GetListPostType(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Posts
                .Where(p => p.ISACTIVE && p.POST_TYPE != null)
                .Select(p => p.POST_TYPE!)
                .Distinct()
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<PostTypeModels>> GetListPostGroupType(CancellationToken cancellationToken = default)
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
                .ToListAsync(cancellationToken);
        }

    }
}
