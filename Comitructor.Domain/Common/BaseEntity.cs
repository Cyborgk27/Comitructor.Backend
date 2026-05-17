namespace Comitructor.Domain.Common
{
    public abstract class BaseEntity<TKey> : IAuditableEntity
    {
        public TKey Id { get; set; } = default!;
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
