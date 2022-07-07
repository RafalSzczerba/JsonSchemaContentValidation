namespace JsonValidationProject.Contracts
{
    public interface IMatching
    {
        Tuple<IList<string>, IList<string>> MatchCardToRange(string jsonCardsfileName, string jsonRagesfileName);

    }
}
