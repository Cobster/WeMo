using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using UPNPLib;
using WeMoLib.ServiceReference1;

namespace WeMoLib
{
    public class WeMoDevice
    {
        public string Name { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
    }

    class WeMoFinder
    {
        public WeMoDevice Find(string name)
        {
            return Find().FirstOrDefault(x => x.Name == name);
        }

        private IEnumerable<WeMoDevice> Find()
        {
            UPnPDeviceFinder finder = new UPnPDeviceFinder();
            UPnPDevices found = finder.FindByType("urn:Belkin:device:controllee:1", 0);
            
            var devices = found.Cast<UPnPDevice>().Select(device =>
            {
                var uri = new Uri(device.PresentationURL);

                return new WeMoDevice
                {
                    Name = device.FriendlyName,
                    IpAddress = uri.Host,
                    Port = uri.Port
                };
            });

            return devices.ToList();
        }
    }

    public class WeMoSwitch
    {
        public static WeMoSwitch ConnectTo(string deviceName)
        {
            var device = new WeMoFinder().Find(deviceName);
            return device != null ? new WeMoSwitch(device.IpAddress, device.Port) : null;
        }

        private readonly BasicServicePortTypeClient _client;

        public WeMoSwitch(string ip, int port)
        {
            _client = new BasicServicePortTypeClient();
            _client.Endpoint.Address = new EndpointAddress($"http://{ip}:{port}/upnp/control/basicevent1");
        }

        public bool IsOff()
        {
            var state = _client.GetBinaryState(new GetBinaryState());
            return state.BinaryState != "1";
        }

        public void TurnOn()
        {
            if (IsOff())
            {
                SetState("1");
            }
        }

        public void TurnOff()
        {
            if (!IsOff())
            {
                SetState("0");
            }
        }

        private void SetState(string binaryState)
        {
            _client.SetBinaryState(new SetBinaryState { BinaryState = binaryState });
        }
    }
}
