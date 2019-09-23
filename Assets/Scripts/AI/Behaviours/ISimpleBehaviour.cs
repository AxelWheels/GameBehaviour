using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AI
{
    public interface ISimpleBehaviour
    {
        bool ValidBehaviour();
        void ExecuteAction();
    }
}
