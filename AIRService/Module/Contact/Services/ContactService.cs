using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using WebCore.Entities;

namespace WebCore.Services
{
    public interface IContactService : IEntityService<Banner> { }
    public partial class ContactService : EntityService<Banner>, IContactService
    {
        public ContactService() : base() { }
        public ContactService(System.Data.IDbConnection db) : base(db) { }

    }
}