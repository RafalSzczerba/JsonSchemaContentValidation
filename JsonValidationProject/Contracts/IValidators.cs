using JsonValidationProject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonValidationProject.Contracts
{
    public interface IValidators
    {
        IList<string> JsonTrack2EnterCut(string jobFileName);
        IList<string> JsonContentValidationCards(string jobFileName);
        IList<string> Track2Validation(string jobFileName);
        IList<string> ZipCodeValidation(string jobFileName);
        IList<string> ValidateRangesIntervalAndOverlap(List<RangeOverlapValidatorInput> input);
        IList<string> JsonContentValidationRanges(string jobFileName);
        Tuple<IList<string>, IList<string>> JsonRangeAndCardsValidationWithLogginToFile(string jsonCardsfileName, string jsonRagesfileName, string fileLogName);
    }
}
