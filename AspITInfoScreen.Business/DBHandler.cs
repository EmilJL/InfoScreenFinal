using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspITInfoScreen.DAL;

namespace AspITInfoScreen.Business
{
    public class DBHandler
    {
        private DbAccess dbAccess;
        private static Model model;
        public DBHandler()
        {
            dbAccess = new DbAccess();
            model = new Model();
        }

        protected static Model Model
        {
            get { return model;}
            set { model = value; }
        }
        public DbAccess DbAccess
        {
            get { return dbAccess; }
            set { dbAccess = value; }
        }
        /// <summary>
        /// Creates a new model to include any changes made to the Database
        /// </summary>
        public void UpdateModel()
        {
            Model = new Model();
        }
    }
}
