namespace Repeat.Domain.SeedWork
{
    public abstract class Entity : IEntity
    {
        public virtual int Id { get; set; }
    }
}