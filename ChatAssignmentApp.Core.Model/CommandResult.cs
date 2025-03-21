namespace ChatAssignmentApp.Core.Model
{
    public class CommandResult<T>
    {
        public T Result { get; set; }
        public bool IsSuccessful { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;

        public CommandResult(T value)
        {
            Result = value;
            IsSuccessful = true;
        }

        public CommandResult(
            bool isSuccessful,
            string errorMessage)
        {
            IsSuccessful = isSuccessful;
            ErrorMessage = errorMessage;
        }
    }
}
