using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonValidationProject.Contracts
{
    public interface IMatching
    {
        Tuple<IList<string>, IList<string>> MatchCardToRange(string jsonCardsfileName, string jsonRagesfileName);

    }
}
