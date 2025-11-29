namespace BusinessObject.Enum
{
    public enum CreationSource
    {
        manual,
        ai,
        file_import
    }

    public enum ProcessingStatus
    {
        pending,
        processing,
        completed,
        failed
    }
}
