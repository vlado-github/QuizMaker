namespace QuizMaker.Domain.Exceptions;

public class RecordNotFoundException : Exception
{
    public RecordNotFoundException(long id, Type recordType) 
        : base($"Record of type {recordType.Name} not found for id = {id}")
    {
    }
}