namespace Timesheet.Domain
{
    public abstract class Entity : IEquatable<Entity>
    {
        //public Entity()
        //{
        //    Id = GenerateId();
        //}

        public Entity(string id)
        {
            Id = id;
        }

        public string Id { get; protected set; }

        public static string GenerateId() => Guid.NewGuid().ToString();

        public DateTime CreatedDate { get; protected set; } = DateTime.Now;

        public DateTime ModifiedDate { get; protected set; } = DateTime.Now;

        public string UpdatedBy { get; protected set; } = "-1";

        public override bool Equals(object? obj)
        {
            if(obj is null)
            {
                return false;
            }

            if(obj.GetType() != GetType())
            {
                return false;
            }

            if(obj is not Entity entity)
            {
                return false;
            }

            return entity.Id == Id;
        }

        public bool Equals(Entity? other)
        {
            if (other is null)
            {
                return false;
            }

            if (other.GetType() != GetType())
            {
                return false;
            }

            return other.Id == Id;
        }

        public override int GetHashCode() => Id.GetHashCode();

        public void UpdateMetadataOnModification(string by)
        {
            ModifiedDate = DateTime.Now;
            UpdatedBy = by ;
        }

        public void UpdateMetadata(DateTime createdDate, DateTime modifiedDate)
        {
            CreatedDate = createdDate;
            ModifiedDate = modifiedDate;
        }

        public void UpdateMetadata()
        {
            this.UpdateMetadata(this.CreatedDate, DateTime.Now);
        }

    }
}
