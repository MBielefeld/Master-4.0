using Master40.DB.Schema.Enums;
using Master40.DB.Models;
using System.Collections.Generic;
using Master40.DB.Data.Repository;

namespace Master40.DB.Schema.Repository
{
    public interface IDemandToProvider
    {
        int Id { get; set; }
        int ArticleId { get; set; }
        Article Article { get; set; }
        decimal Quantity { get; set; }
        int? DemandRequesterId { get; set; }
        DemandToProvider DemandRequester { get; set; }
        List<DemandToProvider> DemandProvider { get; set; }
        State State { get; set; }
    }

    public interface IDemandProviderRepo : IRepository<DemandToProvider>
    {

    }
}
