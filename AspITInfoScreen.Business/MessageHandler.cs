using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspITInfoScreen.DAL;
using AspITInfoScreen.DAL.Entities;

namespace AspITInfoScreen.Business
{
    public class MessageHandler : DBHandler
    {
        /// <summary>
        /// Returns message for provided id
        /// </summary>
        /// <param name="id">Id to search for in the database</param>
        /// <returns></returns>
        public Message GetMessage(int id)
        {
            Message message = Model.Messages.Where(m => m.Id == id).FirstOrDefault();
            return message;
        }
        /// <summary>
        /// Returns the newest message based on logged date in the database
        /// </summary>
        /// <returns></returns>
        public Message GetNewestMessage()
        {
            return Model.Messages.OrderByDescending(m => m.Date).FirstOrDefault();
        }
        /// <summary>
        /// Returns the newest message with name of admin attached
        /// </summary>
        /// <returns></returns>
        public ViewAdminMessageJoin GetNewestViewMessage()
        {
            DbAccess dbAccess = new DbAccess();
            return dbAccess.GetMessagesView();
        }
    }
}
