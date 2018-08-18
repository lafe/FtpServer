using System;

namespace FubarDev.FtpServer
{
    public interface IFtpPasvPortMananger
    {
        bool HasLimitedPorts { get; }
        int PeekPasvPort(TimeSpan timeout);
        void ReleasePasvPort(int port);
    }
}
