namespace JsonValidationProject.Contracts
{
    public interface IExecution
    {
        void FirstJob(string fileName);
        void SecondJob(string fileName);
        void ThirdJob(string jsonCardsfileName, string jsonRagesfileName, string fileLogName);
    }
}
