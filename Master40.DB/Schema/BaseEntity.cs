using System.ComponentModel.DataAnnotations;

namespace Master40.DB
{
    public class BaseEntity : IBaseEntity
    {
        [Key]
        public int Id { get; set; }
        /*
        public BaseEntity(bool MemoryObject)
        {
            if (MemoryObject)
            {
                _count++;
                Id = _count;
            }
        }
        private static int _count;
        */

    }

    public interface IAggregateRoot {

    }
}
