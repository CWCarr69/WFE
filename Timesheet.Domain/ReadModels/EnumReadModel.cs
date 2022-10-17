
namespace Timesheet.Domain.ReadModels
{
    public class EnumReadModel<T> where T : Enum
    {
        public EnumReadModel(T type)
        {
            Type = type;
            Name = Type.ToString();
        }

        public T Type { get; set; }
        public string Name { get; set; }

        public static explicit operator EnumReadModel<T>(T domainType)
            => new EnumReadModel<T>(domainType);
    }
}
