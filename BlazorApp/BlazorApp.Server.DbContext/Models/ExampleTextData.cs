namespace BlazorApp.Server.DbContext.Models
{
    public class ExampleTextData
    {
        public int Id { get; set; }
        public DateTimeOffset Created { get; set; } = DateTimeOffset.Now;
        public string Text { get; set; }
    }
}
