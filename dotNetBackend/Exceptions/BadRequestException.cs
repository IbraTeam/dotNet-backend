namespace dotNetBackend.Exceptions
{
    public class BadRequestException : Exception
    {
        public string? Value { get; set; } = null;

        public BadRequestException() { }
        public BadRequestException(string message) : base(message) { }
        public BadRequestException(string message, string Value) : base(message) { this.Value = Value; }
        public BadRequestException(string message, Exception innerException) : base(message, innerException) { }
    }
}
