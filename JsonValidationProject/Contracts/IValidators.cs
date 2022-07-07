using JsonValidationProject.Model.DTOs;

namespace JsonValidationProject.Contracts
{
    public interface IValidators
    {
        IList<string> Track2NewLineCharacterRemoval(string jobFileName);
        IList<string> JsonCardsContentValidation(string jobFileName);
        IList<string> Track2Validation(string jobFileName);
        IList<string> ZipCodeValidation(string jobFileName);
        IList<string> IsExpired(string jobFileName);
        IList<string> AuthMethod(string jobFileName);
        Tuple<IList<string>, IList<string>> PANValidation(string jobFileName);
        IList<string> ValidateRangesIntervalAndOverlap(List<RangeOverlapValidatorInput> input);
        IList<string> JsonRangesContentValidation(string jobFileName);    
    }
}
