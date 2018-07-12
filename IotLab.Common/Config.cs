using System;
using System.Globalization;
using System.Net;

namespace IotLab.Common
{
    public static class Config
    {
        public const string DeviceId = "h0001";
        public static class IotHub
        {
            public const string ConnectionStringService = "HostName=AldrigoIot.azure-devices.net;SharedAccessKeyName=service;SharedAccessKey=GP7DbSz7wAoUjRZ5yVlvrJuvVj3NoehnsJmM7hz6G8s=";
        }
    }
}
