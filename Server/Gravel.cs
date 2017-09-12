using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Gravel
    {

        public delegate void gravelEventHandler(Item item);

        public event gravelEventHandler gravel;

        #region Singleton Region
        private static volatile Gravel instance;

        public static Gravel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Gravel();
                }
                return instance;
            }
        }
        #endregion

        public void gravelNow(Item item)
        {
            OnGravel(item);
        }

        protected virtual void OnGravel(Item item)
        {
            gravel?.Invoke(item);
        }

    }
}
