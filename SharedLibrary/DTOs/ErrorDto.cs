namespace SharedLibrary.DTOs
{
    public class ErrorDto
    {
        public ErrorDto(string error, bool isShow)
        {
            Errors!.Add(error);
            IsShow = isShow;
        }

        public ErrorDto(List<string> errors, bool isShow)
        {
            Errors = errors;
            IsShow = isShow;
        }

        public List<string> Errors { get; private set; } = new();
        public bool IsShow { get; private set; }
    }
}
