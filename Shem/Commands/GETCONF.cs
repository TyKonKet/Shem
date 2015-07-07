﻿using System;

namespace Shem.Commands
{
    public class GETCONF : TCCommand
    {
        /// <summary>
        /// Configs availables for GETCONF command.
        /// </summary>
        public enum Configs
        {
            SocksPort,

            SocksPolicy,

            MaxMemInQueues,

            Log,

            RunAsDaemon,

            DataDirectory,

            ControlPort,

            HashedControlPassword,

            CookieAuthentication,

            HiddenServiceDir,

            HiddenServicePort,

            ORPort,

            Address,

            OutboundBindAddress,

            Nickname,

            RelayBandwidthRate,

            RelayBandwidthBurst,

            AccountingMax,

            AccountingStart,

            ContactInfo,

            DirPort,

            DirPortFrontPage,

            MyFamily,

            ExitPolicy,

            BridgeRelay,

            PublishServerDescriptor
        }

        private string configs;

        /// <summary>
        /// Returns requested configs of tor instance.
        /// </summary>
        /// <param name="configs"></param>
        public GETCONF(string configs)
        {
            this.configs = configs;
        }

        /// <summary>
        /// Returns requested configs of tor instance.
        /// </summary>
        /// <param name="configs"></param>
        public GETCONF(params Configs[] configs)
        {
            this.configs = new Func<string>(() =>
            {
                string ks = "";
                foreach (var k in configs)
                {
                    ks += " " + k.ToString();
                }
                return ks;
            })();
        }

        public override string Raw()
        {
            return string.Format("GETCONF{0}\r\n", configs);
        }
    }
}
