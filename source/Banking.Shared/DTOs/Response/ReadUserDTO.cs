namespace Banking.Shared.DTOs.Response
{
    public class ReadUserDTO
    {
        public  string? FirstName { get; set; }
        public  string? LastName { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public required ICollection<string>? UserRoles { get; set; }
    }
}