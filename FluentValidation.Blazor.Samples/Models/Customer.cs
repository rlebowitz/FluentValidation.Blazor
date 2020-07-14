namespace FluentValidation.Blazor.Samples.Models
{
    public class Customer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Address Address1 { get; } = new Address();
        public Address Address2 { get; } = new Address();
    }
}
