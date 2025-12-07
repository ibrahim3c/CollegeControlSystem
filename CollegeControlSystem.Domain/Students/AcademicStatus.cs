using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollegeControlSystem.Domain.Students
{
    public enum AcademicStatus
    {
        GoodStanding = 1,

        // "SGPA < 2.00 in any main semester" [cite: 197]
        AcademicWarning = 2,

        // "Four consecutive متتاليه academic warnings" 
        Dismissed = 3
    }
}
