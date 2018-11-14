using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspITInfoScreen.DAL;

namespace AspITInfoScreen.Business
{
    public class MessageHandler : DBHandler
    {
        public Message GetMessage(int id)
        {
            Message message = Model.Messages.Where(m => m.Id == id).FirstOrDefault();
            return message;
        }
        public Message GetNewestMessage(Model model)
        {
            return model.Messages.OrderByDescending(m => m.Date).FirstOrDefault();
        }
    }
}
