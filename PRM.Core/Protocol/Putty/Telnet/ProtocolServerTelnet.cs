﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PRM.Core.Model;
using PRM.Core.Protocol.RDP;
using Shawn.Utils;

namespace PRM.Core.Protocol.Putty.Telnet
{
    public class ProtocolServerTelnet : ProtocolServerWithAddrPortBase, IPuttyConnectable
    {
        public ProtocolServerTelnet() : base("Telnet", "Putty.Telnet.V1", "Telnet", false)
        {
        }

        public override ProtocolServerBase CreateFromJsonString(string jsonString)
        {
            try
            {
                var ret = JsonConvert.DeserializeObject<ProtocolServerTelnet>(jsonString);
                return ret;
            }
            catch (Exception e)
            {
                SimpleLogHelper.Debug(e, e.StackTrace);
                return null;
            }
        }

        public override double GetListOrder()
        {
            return 3;
        }


        public string GetPuttyConnString()
        {
            return $@" -load ""{this.GetSessionName()}"" -telnet {Address} -P {Port}";
        }

        [JsonIgnore]
        public ProtocolServerBase ProtocolServerBase => this;
    }
}
