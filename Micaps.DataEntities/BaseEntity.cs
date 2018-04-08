using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pmss.Micaps.DataEntities
{
    public abstract class BaseEntity
    {
        public int DiamondType { get; set; }
        public string Description { get; set; }
    }
}
