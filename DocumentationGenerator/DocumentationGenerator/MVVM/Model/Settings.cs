using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentationGenerator.MVVM.Model
{
    public class Settings : IDisposable
    {
        public static Settings? Instance { get; private set; }

        public Settings()
        {
            if(Instance != null)
            {
                return;
            }

            if(Instance == null && Instance != this)
            {
                Instance = this;
            }
        }

        ~Settings()
        {
            Dispose();
        }

        public void Dispose()
        {
            
        }
    }
}
