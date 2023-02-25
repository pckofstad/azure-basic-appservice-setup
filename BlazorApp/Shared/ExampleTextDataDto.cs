using BlazorApp.Server.DbContext.Models;

namespace BlazorApp.Shared
{
    public class ExampleTextDataDto
    {
        public int? ExampleTextDataId { get; set; }
        public DateTimeOffset Created { get; set; } = DateTimeOffset.Now;
        public string Text { get; set; }

        public ExampleTextDataDto() { }

        public ExampleTextDataDto(ExampleTextData data)
        {
            ExampleTextDataId = data.Id;
            Created = data.Created;
            Text = data.Text;
        }


        public ExampleTextData ToModel()
        {
            return new ExampleTextData()
            {
                Created = Created,
                Text = Text
            };
        }
    }
}
