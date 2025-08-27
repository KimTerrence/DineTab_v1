using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineTab_v1.Services
{
    [AttributeUsage(AttributeTargets.All)]

    // Attribute to prevent code stripping during linking
    public class PreserveAttribute : Attribute
    {
        public bool AllMembers { get; set; }
        public bool Conditional { get; set; }
    }
}
