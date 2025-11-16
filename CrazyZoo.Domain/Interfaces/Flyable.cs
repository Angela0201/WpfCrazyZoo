using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrazyZoo.Domain.Interfaces
{
    public interface Flyable
    {
        void Fly();
        bool IsFlying { get; }
    }
}