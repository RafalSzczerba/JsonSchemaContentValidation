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
        IList<string> IsExpired(string jobFileName);
        IList<string> AuthMethod(string jobFileName);

        Tuple<IList<string>, IList<string>> PANValidation(string jobFileName);
        IList<string> ValidateRangesIntervalAndOverlap(List<RangeOverlapValidatorInput> input);
        IList<string> JsonContentValidationRanges(string jobFileName);    }
}
