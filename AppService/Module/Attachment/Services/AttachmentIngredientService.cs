using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using WebCore.Entities;

namespace WebCore.Services
{
    public interface IAttachmentIngredientService : IEntityService<AttachmentIngredient> { }
    public class AttachmentIngredientService : EntityService<AttachmentIngredient>, IAttachmentIngredientService
    {
        public AttachmentIngredientService() : base() { }
        public AttachmentIngredientService(System.Data.IDbConnection db) : base(db) { }
    }
}
