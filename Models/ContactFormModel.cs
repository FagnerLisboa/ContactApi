namespace ContactApi.Models
{
	public class ContactFormModel
	{
		public string Name { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }
}
