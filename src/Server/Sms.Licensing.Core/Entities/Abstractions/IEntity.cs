namespace Sms.Licensing.Core.Entities.Abstractions
{
    /// <summary>
    /// Interface to define database entity classes
    /// </summary>
    public interface IEntity<TKey> : IEntityBase
    {
        /// <summary>
        /// Primary key of the entity. TKey is data type of primary key
        /// </summary>
        TKey Id { get; set; }
    }
}