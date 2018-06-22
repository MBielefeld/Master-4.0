using Master40.DB.Data.Context;
using Master40.DB.Data.Repository;
using Master40.DB.Models;

namespace Master40.DB.Schema.Repository
{
    public interface IArticleRepo : IRepository<Article>
    {
        //Article GetArticleTree(int id);
    }

    public class ArticleRepository : Repository<Article>, IArticleRepo
    {
        public ArticleRepository(MasterDBContext context)
            : base(context)
        {
        }
    }
}
