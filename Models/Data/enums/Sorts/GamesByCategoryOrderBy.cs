using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Data.enums.Sorts;

public enum GamesByCategoryOrderBy
{
    Relevance = 0,
    NewestFirst = 1,
    OldestFirst = 2,
    PriceLowToHigh = 3,
    PriceHighToLow = 4,
    NameAZ = 5,
    NameZA = 6,
}
