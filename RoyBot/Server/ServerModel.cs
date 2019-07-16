using RoyBot.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RoyBot.Server
{
    public class ServerModel
    {
        [Key]
        public int Key
        {
            get;
            set;
        }

        public Guid ServerGUID
        {
            get;
            set;
        }
         
        [UserEditable]
        public string Description
        {
            get;
            set;
        }

        public ulong Owner
        {
            get;
            set;
        }
        
        public ICollection<ManagementRole> ManagementRoles
        {
            get;
            set;
        }

        public ulong ServerSnowflake
        {
            get;
            set;
        }

        [UserEditable]
        public ulong AdminChannel
        {
            get;
            set;
        }

        

        public ServerModel()
        {

        }
    }

    public class ManagementRole
    {
        [Key]
        public int Key
        {
            get;
            set;
        }

        public ulong Role
        {
            get;
            set;
        }
        
        public ManagementRole()
        {

        }
    }
}
