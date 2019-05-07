using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AspITInfoScreen.DAL;
using AspITInfoScreen.DAL.Entities;

namespace AspITInfoScreen.Business
{
    public class IPHandler : DBHandler
    {
        /// <summary>
        /// Checks whether the device has an IP and returns it in a string format
        /// </summary>
        /// <returns></returns>
        public static string CheckIP()
        {
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
                throw new Exception("No network with an IPv4 address in the system.");
            }
            throw new Exception("No connection.");
        }
        /// <summary>
        /// Overrides the old IP in the database
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public bool UpdateIP(string ip)
        {
            IpAddress address = Model.IpAddresses.FirstOrDefault();
            address.Ip = ip;
            DbAccess.SetActiveIP(address);
            return true;
        }
        /// <summary>
        /// Creates a new row in the database with the current IP
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public bool CreateNewIp(string ip)
        {
            DbAccess.CreateNewIP(ip);
            return true;
        }
        /// <summary>
        /// Retrieves the first IP from the database. There only be one
        /// </summary>
        /// <returns></returns>
        public IpAddress GetIp()
        {
            return Model.IpAddresses.FirstOrDefault();
        }
        /// <summary>
        /// Checks whether the device has an IP in the database
        /// </summary>
        /// <returns></returns>
        public bool HasIp()
        {
            if(Model.IpAddresses != null && Model.IpAddresses.Count > 0)
            {
                return true;
            } return false;
        }
    }
}
